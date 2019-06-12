using System.Collections.Generic;
using AutoMapper;
using CMS.UI;
using System.Data;
using System.Linq;
using System;

namespace CMS.Behaviours
{
    internal class DefaultControlBehaviour : Behaviour, IControlBehaviour
    {
        public DefaultControlBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public Control Make(ControlDefinition definition, DAL.Models.DbSearchResponse ticketSet, UI.Instance parentNode, Action<Control> controlAction)
        {
            var control = new Control()
            {
                ControlType = Mapper.Map<UI.ControlType>(definition.ControlType),
                ParentNode = parentNode
            };

            control.Props = new Dictionary<string, object>()
            {
                { "Id", definition.Id },
                { "DisplayName", definition.DisplayName },
                { "Width", definition.Width },
                { "Order", definition.OrderIndex },
                { "Style", Mapper.Map<UI.StyleDefinition>(((DAL.Models.Control)definition.Entity).Style) }
            };

            var dalControl = (DAL.Models.Control)definition.Entity;

            if (dalControl.Field != null)
            {
                control.Props.Add("FieldId", dalControl.Field.Id);
                control.Props.Add("Name", dalControl.Field.Name);
            }

            var ticket = ticketSet.Tickets.FirstOrDefault();

            control.Events = definition
                .Events
                .Select(e => BehaviourSelector.EventBehaviours[Mapper.Map<UI.EventType>(e.EventType)](CurrentUser).Make(e, ticketSet, control))
                .ToList();

            if (ticket != null && dalControl.FieldId.HasValue)
            {
                control.Value = ticket[dalControl.Field];
                control.DocId = Convert.ToInt32(ticket[dalControl.Field.TemplateId, "Id"]);
            }

            return control;
        }

    }
}
