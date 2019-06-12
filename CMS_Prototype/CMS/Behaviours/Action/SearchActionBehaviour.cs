using AutoMapper;
using CMS.DAL.Interfaces;

using CMS.DAL.Services;
using CMS.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CMS.Behaviours
{
    internal class SearchActionBehaviour : Behaviour, IActionBehaviour
    {
        public SearchActionBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public ActionResult Execute(UI.Action action)
        {
            var filters = action.ViewData.Filters;

            var viewId = Convert.ToInt32(filters.First().Props["ViewId"]);

            var dalViewDef = DbEditorService.GetViewDeep(viewId);

            var viewBehaviour = BehaviourSelector.ViewBehaviours[Mapper.Map<UI.ViewType>(dalViewDef.ViewType)](CurrentUser);

            var pageNumParam = action.Parameters.FirstOrDefault(p => p.ParameterType == ParameterType.PageNumber);
            var pageSizeParam = action.Parameters.FirstOrDefault(p => p.ParameterType == ParameterType.PageSize);
            var sortFieldParam = action.Parameters.FirstOrDefault(p => p.ParameterType == ParameterType.SortFieldId);
            var sortDirectionParam = action.Parameters.FirstOrDefault(p => p.ParameterType == ParameterType.SortDirection);

            bool hasPageAndSortInfo = true;

            if (string.IsNullOrEmpty(pageNumParam.DefaultValue) ||
                string.IsNullOrEmpty(pageSizeParam.DefaultValue) ||
                string.IsNullOrEmpty(sortFieldParam.DefaultValue) ||
                string.IsNullOrEmpty(sortDirectionParam.DefaultValue))
                hasPageAndSortInfo = false;

            DAL.Models.DbSearchRequestParams sortAndPageParams = DAL.Models.DbSearchRequestParams.GetDefault(dalViewDef);

            if (hasPageAndSortInfo)
            {
                var pageNum = Convert.ToInt32(pageNumParam.DefaultValue);
                var pageSize = Convert.ToInt32(pageSizeParam.DefaultValue);
                var sortFieldId = Convert.ToInt32(sortFieldParam.DefaultValue);
                var sortDir = sortDirectionParam.DefaultValue;

                var sortField = DbEditorService.GetField(sortFieldId);

                sortAndPageParams = new DAL.Models.DbSearchRequestParams(pageNum, pageSize, sortField, sortDir);
            }


            var ticketSet =  DbDocumentService.GetDocuments(dalViewDef, filters.Select(f => (IFilterWithValue)f).ToList(), sortAndPageParams);

            var viewDef = Mapper.Map<UI.ViewDefinition>(dalViewDef);
            var view = BehaviourSelector.ViewBehaviours[Mapper.Map<UI.ViewType>(viewDef.ViewType)](CurrentUser).Make(viewDef, ticketSet, null, null);

            return new ActionResult()
            {
                Success = true,
                ActionType = action.ActionType,
                Data = view
            };
        }

        public UI.Action Make(ActionDefinition definition, DAL.Models.DbSearchResponse ticketSet, Event parentNode)
        {
            var action = new UI.Action()
            {
                Virtual = true,
                ParentNode = parentNode,
                ActionType = ActionType.Search
            };

            var parentControl = action.ParentNode.ParentNode;

            action.Parameters = new List<Parameter>()
            {
                new UI.Parameter()
                {
                    ParameterType = ParameterType.PageNumber,
                    ParentNode = action,
                    Virtual = true,
                    DefaultValue = parentControl.Props.ContainsKey("PageNumber") ? (string)parentControl.Props["PageNumber"] : string.Empty
                },
                new UI.Parameter()
                {
                    ParameterType = ParameterType.PageSize,
                    ParentNode = action,
                    Virtual = true,
                    DefaultValue = parentControl.Props.ContainsKey("PageSize") ? (string)parentControl.Props["PageSize"] : string.Empty
                },
                new UI.Parameter()
                {
                    ParameterType = ParameterType.SortFieldId,
                    ParentNode = action,
                    Virtual = true,
                    DefaultValue = parentControl.Props.ContainsKey("SortFieldId") ? (string)parentControl.Props["SortFieldId"] : string.Empty
                },
                new UI.Parameter()
                {
                    ParameterType = ParameterType.SortDirection,
                    ParentNode = action,
                    Virtual = true,
                    DefaultValue = parentControl.Props.ContainsKey("SortDirection") ? (string)parentControl.Props["SortDirection"] : string.Empty
                }
            };

            return action;
        }
    }
}
