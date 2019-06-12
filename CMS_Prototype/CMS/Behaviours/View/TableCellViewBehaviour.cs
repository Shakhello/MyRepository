using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoMapper;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class TableCellViewBehaviour : Behaviour, IViewBehaviour
    {
        public TableCellViewBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public View Make(ViewDefinition definition, DAL.Models.DbSearchResponse ticketSet, View parentNode, Action<UI.View> viewAction)
        {
            var view = new UI.View()
            {
                ViewType = ViewType.TableCell,
                ParentNode = parentNode,
                Virtual = true
            };

            var gridIndex = Convert.ToInt32(parentNode.Props["GridIndex"]);

            view.Props = new Dictionary<string, object>()
            {
                { "TemplateId", definition.TemplateId },
                { "GridIndex", gridIndex }
            };

            view.Controls = definition
                .Controls.Where(c => c.GridIndex >= 0 && c.GridIndex == gridIndex).OrderBy(c => c.OrderIndex)
                .Select(c => BehaviourSelector.ControlBehaviours[Mapper.Map<UI.ControlType>(c.ControlType)](CurrentUser).Make(c, ticketSet, view, null))
                .ToList();

            return view;
        }
    }
}
