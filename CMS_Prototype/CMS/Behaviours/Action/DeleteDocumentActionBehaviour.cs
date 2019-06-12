using System;
using System.Data;
using System.Linq;
using AutoMapper;
using CMS.UI;
using Common.Exceptions;

namespace CMS.Behaviours
{
    internal class DeleteDocumentActionBehaviour : Behaviour, IActionBehaviour
    {
        public DeleteDocumentActionBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public UI.Action Make(ActionDefinition definition, DAL.Models.DbSearchResponse ticketSet, Event parentNode)
        {
            UI.Action action = new UI.Action()
            {
                ActionType = Mapper.Map<UI.ActionType>(definition.ActionType),
                ParentNode = parentNode
            };

            action.DocId = parentNode.ParentNode.DocId;

            action.Parameters = definition
                .Parameters
                .Select(pDef => BehaviourSelector.ParameterBehaviours[Mapper.Map<UI.ParameterType>(pDef.ParameterType)](CurrentUser).Make(pDef, action))
                .ToList();

            return action;
        }

        public ActionResult Execute(UI.Action action)
        {
            Parameter templateParam = action.Parameters.FirstOrDefault(p => p.ParameterType == ParameterType.TemplateId);

            if(templateParam == null)
            {
                throw new CustomValidationException($"There is no TemplateId parameter for action DeleteDocument.");
            }

            int templateId = Convert.ToInt32(templateParam.DefaultValue);

            DAL.Models.Template templateDef = DbEditorService.GetTemplate(templateId);

            DbDocumentService.UpdateDocumentToDeleted(templateDef.Name, action.DocId.Value);

            return new ActionResult()
            {
                ActionType = action.ActionType,
                Success = true
            };
        }
    }
}
