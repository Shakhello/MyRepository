using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoMapper;
using Common.Exceptions;

namespace CMS.Behaviours
{
    internal class ButtonControlBehaviour : Behaviour, IControlBehaviour
    {
        public ButtonControlBehaviour(UI.UserDefinition currentUser) : base(currentUser) { }

        public UI.Control Make(UI.ControlDefinition definition, DAL.Models.DbSearchResponse ticketSet, UI.Instance parentNode, Action<UI.Control> controlAction)
        {
            var button = new UI.Control()
            {
                ControlType = Mapper.Map<UI.ControlType>(definition.ControlType),
                ParentNode = parentNode
            };

            var dalControl = (DAL.Models.Control)definition.Entity;

            var ticket = ticketSet.Tickets.FirstOrDefault();

            if (dalControl.Field != null)
            {
                button.Props.Add("FieldId", dalControl.Field.Id);
                button.Props.Add("Name", dalControl.Field.Name);
                button.DocId = Convert.ToInt32(ticket[dalControl.Field]);
            }

            button.Events = definition
                .Events
                .Select(e => BehaviourSelector.EventBehaviours[Mapper.Map<UI.EventType>(e.EventType)](CurrentUser).Make(e, ticketSet, button))
                .ToList();

            button.Props = new Dictionary<string, object>()
            {
                { "DisplayName", definition.DisplayName },
                { "Width", definition.Width },
                { "Order", definition.OrderIndex },
                { "Style", Mapper.Map<UI.StyleDefinition>(((DAL.Models.Control)definition.Entity).Style) }
            };

            return button;
        }


    }
}
