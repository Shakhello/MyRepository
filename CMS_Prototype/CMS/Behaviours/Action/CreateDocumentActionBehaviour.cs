using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoMapper;
using CMS.DAL.Services;
using CMS.UI;
using Common.Exceptions;

namespace CMS.Behaviours
{
    internal class CreateDocumentActionBehaviour : Behaviour, IActionBehaviour
    {

        public CreateDocumentActionBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public ActionResult Execute(UI.Action action)
        {
            // Создаваемый шаблон
            var tempIdParam = action.Parameters.FirstOrDefault(p => p.ParameterType == UI.ParameterType.TemplateId);

            if (tempIdParam == null)
            {
                throw new CustomValidationException($"TemplateId parameter missing for action {action.Props["Id"]}.");
            }

            // Получаем модель шаблона по которой будем создавать документ
            var templateDef = DbEditorService.GetTemplate(Convert.ToInt32(tempIdParam.DefaultValue));

            // Создаем значения по умолчанию, которыми будет заполнен новый документ
            var values = templateDef.Fields
                .Select(f => new KeyValuePair<DAL.Models.Field, object>(f, null))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            // Проверяем есть ли параметры копирования значений в новый шаблон
            FillValues(action, templateDef, values);

            // Создаем документ шаблона и получаем его ID
            var docId = DbDocumentService.CreateDocument(values);

            // Страницу на которую мы перейдем и в которой будет созданный документ по шаблону
            var viewIdParam = action.Parameters.FirstOrDefault(p => p.ParameterType == UI.ParameterType.ViewId);

            // Проверка параметров
            if (viewIdParam != null)
            {
                // Проверяем есть ли на странице фильтр с помощью которого мы попадем на нее
                var viewDefDal = DbEditorService.GetViewDeep(Convert.ToInt32(viewIdParam.DefaultValue));
                var filterDefDal = viewDefDal.Filters.FirstOrDefault();
                if (filterDefDal == null)
                {
                    throw new CustomValidationException($"View {viewDefDal.Id} must have a filter defined.");
                }
                // Создаем фильтр поля ID с помощью которого мы откроем страницу которая привязана к созданному документу 
                // Заполняем его значением созданного документа
                var filterDef = Mapper.Map<UI.FilterDefinition>(filterDefDal);
                var filter = BehaviourSelector.FilterBehaviours[Mapper.Map<UI.FilterType>(filterDef.FilterType)](CurrentUser).Make(filterDef, null);
                filter.Value = docId;

                // Ищем только что созданный документ по фильтру и открываем страницу с привязкой к этому документу
                var searchParams = DAL.Models.DbSearchRequestParams.GetDefault(viewDefDal);
                var ticketSet = DbDocumentService.GetDocuments(viewDefDal, new List<DAL.Interfaces.IFilterWithValue>() { filter }, searchParams);
                var viewDef = Mapper.Map<UI.ViewDefinition>(viewDefDal);
                var view = BehaviourSelector.ViewBehaviours[Mapper.Map<UI.ViewType>(viewDef.ViewType)](CurrentUser).Make(viewDef, ticketSet, null, null);

                // Возвращаем страницу в качестве результата
                return new ActionResult()
                {
                    ActionType = action.ActionType,
                    Data = view,
                    Success = true
                };
            }
            else
            {
                return new ActionResult()
                {
                    ActionType = action.ActionType,
                    Success = true
                };
            }
        }

        public UI.Action Make(ActionDefinition definition, DAL.Models.DbSearchResponse ticketSet, UI.Event parentNode)
        {
            var action = new UI.Action()
            {
                ActionType = Mapper.Map<UI.ActionType>(definition.ActionType),
                ParentNode = parentNode
            };

            action.Props.Add("Id", definition.Id);
            action.DocId = parentNode.ParentNode.DocId;

            action.Parameters = definition
                .Parameters
                .Select(pDef => BehaviourSelector.ParameterBehaviours[Mapper.Map<UI.ParameterType>(pDef.ParameterType)](CurrentUser).Make(pDef, action))
                .ToList();

            return action;
        }

        private static void FillValues(UI.Action action, DAL.Models.Template templateDef, Dictionary<DAL.Models.Field, object> values)
        {
            List<Parameter> copyParameters = action.Parameters
                            .FindAll(p => p.ParameterType == UI.ParameterType.CopyValueFrom || p.ParameterType == UI.ParameterType.CopyValueTo);
            if (copyParameters.Count > 0)
            {
                if (!copyParameters.All(p => p.Props.ContainsKey("Name") && !string.IsNullOrEmpty((string)p.Props["Name"])))
                {
                    throw new CustomValidationException($"All CopyParameters have to contain a Name property. ActionId={action.Definition.Id}");
                }

                if (!copyParameters.Where(p => p.ParameterType == ParameterType.CopyValueTo).All(p => p.Props.ContainsKey("FieldId")))
                {
                    throw new CustomValidationException($"All CopyParameters.Destination have to contain a FieldId property. ActionId={action.Definition.Id}");
                }

                // Создаем набор параметров из списка двух типов параметров
                List<Incoming.CopyFieldParameter> prms = copyParameters.GroupBy(p => (string)p.Props["Name"]).Select(g =>
                {
                    if (g.Count() != 2)
                    {
                        throw new CustomValidationException($"There is no pair for parameter: {g.Key}");
                    }
                    Parameter paramFrom = g.FirstOrDefault(p => p.ParameterType == ParameterType.CopyValueFrom);
                    Parameter paramTo = g.FirstOrDefault(p => p.ParameterType == ParameterType.CopyValueTo);

                    paramFrom.Props.Where(p => p.Value == null).ToList().ForEach(p => paramFrom.Props.Remove(p.Key));
                    paramTo.Props.Where(p => p.Value == null).ToList().ForEach(p => paramTo.Props.Remove(p.Key));

                    int fieldFromId = paramFrom.Props.ContainsKey("FieldId") ? Convert.ToInt32(paramFrom.Props["FieldId"]) : 0;
                    int fieldToId = paramTo.Props.ContainsKey("FieldId") ? Convert.ToInt32(paramTo.Props["FieldId"]) : 0;

                    Incoming.CopyFieldParameter paramCopy = new Incoming.CopyFieldParameter()
                    {
                        DefaultValue = paramFrom.DefaultValue,
                        FieldFromId = fieldFromId,
                        FieldToId = fieldToId
                    };
                    return paramCopy;
                }).ToList();

                // Заполняем значениями по умолчанию
                foreach (var param in prms.Where(p => p.FieldToId > 0 && p.DefaultValue != null))
                {
                    DAL.Models.Field templateDefField = templateDef.Fields.FirstOrDefault(f => f.Id == param.FieldToId);
                    if (templateDefField == null)
                    {
                        throw new CustomValidationException($"Destination template missing field id={param.FieldToId}.");
                    }
                    values.Add(templateDefField, param.DefaultValue);
                }

                // Заполняем значениями документа из которого был создан новый документ
                // Определяем шаблон источника
                int fieldId = prms.FirstOrDefault(p => p.FieldFromId > 0)?.FieldFromId ?? 0;
                if (fieldId > 0)
                {
                    DbEditorService editor = new DbEditorService();
                    DAL.Models.Field field = editor.GetField(fieldId);
                    DAL.Models.Template templateDefFrom = DbEditorService.GetTemplate(field.TemplateId);
                    int parentDocId = Convert.ToInt32(action.DocId);
                    Dictionary<string, object> documentFrom = DbDocumentService.GetDocument(templateDefFrom.Name, parentDocId);
                    foreach (var param in prms.Where(p => p.FieldFromId > 0))
                    {
                        DAL.Models.Field fieldFrom = templateDefFrom.Fields.FirstOrDefault(f => f.Id == param.FieldFromId);
                        DAL.Models.Field fieldTo = templateDef.Fields.FirstOrDefault(f => f.Id == param.FieldToId);
                        values.Add(fieldTo, documentFrom[fieldFrom.Name]);
                    }
                }
            }
        }
    }
}
