using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class AutoCompleteControlBehaviour : Behaviour, IControlBehaviour
    {
        public AutoCompleteControlBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public UI.Control Make(ControlDefinition definition, DAL.Models.DbSearchResponse ticketSet, Instance parentNode, Action<UI.Control> controlAction)
        {
            var control = new Control
            {
                ControlType = ControlType.AutoComplete,
                ParentNode = parentNode
            };

            control.Events = definition
                    .Events
                    .Select(a => BehaviourSelector.EventBehaviours[Mapper.Map<UI.EventType>(a.EventType)](CurrentUser).Make(a, ticketSet, null))
                    .ToList();

            control.Props = new Dictionary<string, object>()
            {
                { "DisplayName", definition.DisplayName },
                { "Width", definition.Width },
                { "Order", definition.OrderIndex },
                { "Id", definition.Id }
            };

            return control;
        }
    }
}
