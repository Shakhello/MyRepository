using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoMapper;
using CMS.DAL.Interfaces;
using CMS.DAL.Models;
using CMS.DAL.Services;
using CMS.UI;
using Common.Exceptions;

namespace CMS.Behaviours
{
    internal class GotoActionBehaviour : Behaviour, IActionBehaviour
    {
        public GotoActionBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public ActionResult Execute(UI.Action action)
        {
            if (action.DocId == null)
                throw new CustomValidationException("DocId not defined for action's parent control. Please assign Id field to control.");

            var viewIdParameter = action.Parameters.FirstOrDefault(p => p.ParameterType == UI.ParameterType.ViewId);
            var anchorParameter = action.Parameters.FirstOrDefault(p => p.ParameterType == UI.ParameterType.ViewAnchor);


            var dbViewDef = DbEditorService.GetViewDeep(Convert.ToInt32(viewIdParameter.DefaultValue));
            var viewDef = Mapper.Map<ViewDefinition>(dbViewDef);

            var fieldId = Convert.ToInt32(action.Props["FieldId"]);

            var field = DbEditorService.GetField(fieldId);

            var filter = new VirtualFilter()
            {
                Operation = DAL.Models.OperationType.EqualTo,
                ViewId = dbViewDef.Id
            };

            filter.FilterFields = new List<IFilterField>() { new VirtualFilterField() { ChainId = 0, Depth = 0, Filter = filter, Field = field } };

            var filterWithValue = new VirtualFilterWithValue() { Filter = filter, Value = action.DocId };

            var searchParams = DAL.Models.DbSearchRequestParams.GetDefault(dbViewDef);

            var ticketSet = DbDocumentService.GetDocuments(dbViewDef, new List<IFilterWithValue>() { filterWithValue }, searchParams);

            UI.View view;

            if (ticketSet.Tickets.Count == 1)
            {
                view = BehaviourSelector.ViewBehaviours[Mapper.Map<UI.ViewType>(dbViewDef.ViewType)](CurrentUser)
                    .Make(viewDef, ticketSet, null, null);

                var views = GetFlatListOfViews(view);

                if (anchorParameter != null && views.Count > 0)
                {
                    foreach (var v in views)
                    {
                        if (v.Props.ContainsKey("Active"))
                            v.Props["Active"] = (v.Definition?.Id == Convert.ToInt32(anchorParameter.DefaultValue));
                    }

                }
            }
            else
                throw new CustomValidationException("Applying filters in goto action does not result in a single document.");

            var result = new ActionResult()
            {
                Success = true,
                ActionType = action.ActionType,
                Data = view
            };

            return result;
        }

        public UI.Action Make(ActionDefinition definition, DAL.Models.DbSearchResponse ticketSet, UI.Event parentNode)
        {
            var action = new UI.Action()
            {
                ActionType = Mapper.Map<UI.ActionType>(definition.ActionType),
                ParentNode = parentNode
            };

            var control = parentNode.ParentNode;

            action.DocId = control.DocId;

            if (control.Props.ContainsKey("FieldId"))
                action.Props.Add("FieldId", parentNode.ParentNode.Props["FieldId"]);

            action.Parameters = definition
                .Parameters
                .Select(p => BehaviourSelector.ParameterBehaviours[Mapper.Map<UI.ParameterType>(p.ParameterType)](CurrentUser).Make(p, action))
                .ToList();

            return action;
        }



    }
}
