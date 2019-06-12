using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoMapper;
using CMS.DAL.Interfaces;
using CMS.DAL.Services;
using CMS.UI;
using Common.Exceptions;

namespace CMS.Behaviours
{
    internal class OpenSectionActionBehaviour : Behaviour, IActionBehaviour
    {
        public OpenSectionActionBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public ActionResult Execute(UI.Action action)
        {
            var filterIdParameter = action.Parameters.FirstOrDefault(p => p.ParameterType == ParameterType.FilterId);
            var dbFilterDef = DbEditorService.GetFilter(Convert.ToInt32(filterIdParameter.DefaultValue));

            if (dbFilterDef == null)
                throw new CustomValidationException("Filter is not defined for this action.");

            var filterDef = Mapper.Map<UI.FilterDefinition>(dbFilterDef);

            var filter = BehaviourSelector.FilterBehaviours[Mapper.Map<UI.FilterType>(filterDef.FilterType)](CurrentUser).Make(filterDef, null);

            var dbViewDef = DbEditorService.GetViewDeep(filterDef.ViewId);

            var filterValues = new Dictionary<DAL.Models.Filter, object>() { { dbFilterDef, null } };

            var searchParams = DAL.Models.DbSearchRequestParams.GetDefault(dbViewDef);
            var tickets = DbDocumentService.GetDocuments(dbViewDef, new List<IFilterWithValue>() { filter }, searchParams);

            var viewDef = Mapper.Map<UI.ViewDefinition>(dbViewDef);
            var view = BehaviourSelector.ViewBehaviours[Mapper.Map<UI.ViewType>(viewDef.ViewType)](CurrentUser).Make(viewDef, tickets, null, null);

            var result = new ActionResult()
            {
                Success = true,
                ActionType = action.ActionType,
                Data = view
            };

            return result;
        }

        public UI.Action Make(ActionDefinition definition, DAL.Models.DbSearchResponse ticketSet, Event parentNode)
        {
            var action = new UI.Action()
            {
                ActionType = Mapper.Map<UI.ActionType>(definition.ActionType),
                ParentNode = parentNode
            };

            action.Parameters = definition
                .Parameters
                .Select(p => BehaviourSelector.ParameterBehaviours[Mapper.Map<UI.ParameterType>(p.ParameterType)](CurrentUser).Make(p, action)).ToList();


            return action;
        }
}
}
