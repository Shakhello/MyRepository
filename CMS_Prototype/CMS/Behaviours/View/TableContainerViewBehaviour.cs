using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CMS.Resources;
using CMS.UI;
using Common.Exceptions;

namespace CMS.Behaviours
{
    internal class TableContainerViewBehaviour : Behaviour, IViewBehaviour
    {
        public TableContainerViewBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public View Make(ViewDefinition definition, DAL.Models.DbSearchResponse ticketSet, View parentNode, Action<UI.View> viewAction)
        {
            var view = new View()
            {
                ViewType = ViewType.TableContainer,
                ParentNode = parentNode,
                Virtual = true
            };

            view.Props = new Dictionary<string, object>()
            {
                { "Id", definition.Id },
                { "Name", definition.Name },
                { "DisplayName", definition.DisplayName },
                { "TemplateId", definition.TemplateId },
                { "GridWidth", definition.GridWidth }
            };

            view.ChildViews = new List<View>()
            {
                new TableHeaderViewBehaviour(CurrentUser).Make(definition, ticketSet, view, null),
                new TableBodyViewBehaviour(CurrentUser).Make(definition, ticketSet, view, null),
                new TableFooterViewBehaviour(CurrentUser).Make(definition, ticketSet, view, null)
            };

            foreach (var control in definition.Controls.Where(c => c.GridIndex < 0))
            {
                view.Controls.Add(BehaviourSelector.ControlBehaviours[Mapper.Map<UI.ControlType>(control.ControlType)](CurrentUser)
                    .Make(control, ticketSet, view, null));
            }

            view.Filters = definition
                    .Filters
                    .Select(fDef => BehaviourSelector.FilterBehaviours[Mapper.Map<UI.FilterType>(fDef.FilterType)](CurrentUser).Make(fDef, view))
                    .ToList();

            if (view.Filters.Where(f=>f.FilterType != FilterType.Hidden).Count() > 0)
            {
                view.Controls.Add(new ButtonSearchControlBehaviour(CurrentUser).Make(null, ticketSet, view, null));
            }

            foreach (var filter in view.Filters)
            {
                var filterDefName = string.Join("", filter.Fields.Select(f => f.GetName()).OrderBy(f => f));
                foreach (var appliedFilter in ticketSet.Request.Filters)
                {
                    var appliedFilterName = string.Join("", appliedFilter.GetFilter().Fields.Select(f => f.Name).OrderBy(f => f));

                    if (filterDefName == appliedFilterName)
                        filter.Value = appliedFilter.GetValue();
                }
            }

            return view;
        }

    }
}
