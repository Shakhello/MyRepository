using System;
using System.Linq;
using AutoMapper;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class SearchTicketsWithAutoCompleteBehaviour : Behaviour, IActionBehaviour
    {
        public SearchTicketsWithAutoCompleteBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public ActionResult Execute(UI.Action action)
        {
            var searchFieldIds = action
                .Parameters
                .Where(p => p.ParameterType == ParameterType.AutoCompleteSearchFieldId)
                .Select(p => Convert.ToInt32(p.DefaultValue))
                .ToList();

            var displayFieldIds = action
                .Parameters
                .Where(p => p.ParameterType == ParameterType.AutoCompleteDisplayFieldId)
                .Select(p => Convert.ToInt32(p.DefaultValue))
                .ToList();

            var searchFields = DbEditorService.GetFields(searchFieldIds);
            var displayFields = DbEditorService.GetFields(displayFieldIds);

            var templateName = searchFields.First().Template.Name;

            var result = DbDocumentService.GetDocuments(displayFields, searchFields, DAL.Models.OperationType.StartsWith, action.Value);

            return new ActionResult()
            {
                ActionType = action.ActionType,
                Success = true,
                Data = result
            };
        }

        public UI.Action Make(ActionDefinition definition, DAL.Models.DbSearchResponse ticketSet, UI.Event parentNode)
        {
            var action = new UI.Action()
            {
                ActionType = ActionType.SearchTicketsWithAutoComplete,
                ParentNode = parentNode
            };

            action.Parameters = definition
                .Parameters
                .Select(pDef => BehaviourSelector.ParameterBehaviours[Mapper.Map<UI.ParameterType>(pDef.ParameterType)](CurrentUser).Make(pDef, action))
                .ToList();

            return action;
        }
    }
}
