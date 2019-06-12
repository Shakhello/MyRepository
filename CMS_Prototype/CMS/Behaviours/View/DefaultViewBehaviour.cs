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
    internal class DefaultViewBehaviour : Behaviour, IViewBehaviour
    {
        public DefaultViewBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public View Make(ViewDefinition definition, DAL.Models.DbSearchResponse ticketSet, View parentNode, Action<UI.View> viewAction)
        {
            var view = new View()
            {
                ViewType = Mapper.Map<UI.ViewType>(definition.ViewType),
                ParentNode = parentNode
            };

            var ticket = ticketSet.Tickets.FirstOrDefault();

            // TODO
            //view.DocId = (ticket != null) ? (int?)Convert.ToInt32(ticket["Id"]) : null;

            view.Props = new Dictionary<string, object>()
            {
                { "Name", definition.Name },
                { "TemplateId", definition.TemplateId },
                { "OrderIndex", definition.OrderIndex },
                { "DisplayName", definition.DisplayName },
                { "GridWidth", definition.GridWidth },
                { "LinkedFieldId", definition.LinkedFieldId },
                { "Active", false }
            };

            view.Controls = definition
                    .Controls
                    .Select(c => BehaviourSelector.ControlBehaviours[Mapper.Map<UI.ControlType>(c.ControlType)](CurrentUser).Make(c, ticketSet, view, null))
                    .ToList();

            view.Filters = definition
                    .Filters
                    .Select(f => BehaviourSelector.FilterBehaviours[Mapper.Map<UI.FilterType>(f.FilterType)](CurrentUser).Make(f, view))
                    .ToList();

            foreach (var childViewDef in definition.ChildViews)
            {
                View childView;

                if (childViewDef.ViewType == DAL.Models.ViewType.Table)
                {
                    var parentFilters = ticketSet.Request.Filters;

                    var searchParams = DAL.Models.DbSearchRequestParams.GetDefault((DAL.Models.View)childViewDef.Entity);
                    searchParams.SearchWithInnerJoin = true;
                    var linkedTickets = DbDocumentService.GetDocuments((DAL.Models.View)childViewDef.Entity, parentFilters, searchParams);

                    childView = BehaviourSelector.ViewBehaviours[Mapper.Map<UI.ViewType>(childViewDef.ViewType)](CurrentUser)
                        .Make(childViewDef, linkedTickets, null, null);
                }
                else
                    childView = BehaviourSelector.ViewBehaviours[Mapper.Map<UI.ViewType>(childViewDef.ViewType)](CurrentUser)
                        .Make(childViewDef, ticketSet, null, null);

                view.ChildViews.Add(childView);
            }

            if (definition.ChildViews.Any(cv => cv.ViewType == DAL.Models.ViewType.Tab))
            {
                var parentView = new View()
                {
                    ViewType = ViewType.TabContainer,
                    ParentNode = parentNode
                };



                var tabViews = view.ChildViews.Where(v => v.ViewType == ViewType.Tab);

                view.ChildViews = view.ChildViews.Except(tabViews).ToList();

                view.ViewType = ViewType.Tab;
                view.ParentNode = parentView;
                view.Props["Active"] = true;

                parentView.DocId = view.DocId;
                parentView.Props = view.Props;

                parentView.ChildViews.AddRange((new List<View>() { view }).Union(tabViews));

                return parentView;
            }
                else return view;
        }

    }
}
