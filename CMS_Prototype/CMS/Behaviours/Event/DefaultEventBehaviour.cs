using System;
using System.Data;
using System.Linq;
using AutoMapper;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class DefaultEventBehaviour : Behaviour, IEventBehaviour
    {
        public DefaultEventBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public EventResult Execute(UI.Event evt)
        {
            var result = new EventResult()
            {
                ActionResults = evt
                    .Actions
                    .Select(a => BehaviourSelector.ActionBehaviours[a.ActionType](CurrentUser).Execute(a))
                    .ToList()
            };

            return result;
        }

        public Event Make(EventDefinition definition, DAL.Models.DbSearchResponse ticketSet, Instance parentNode)
        {
            var evt = new Event()
            {
                EventType = Mapper.Map<UI.EventType>(definition.EventType),
                ParentNode = parentNode
            };

            evt.Actions = definition
                    .Actions
                    .Select(a => BehaviourSelector.ActionBehaviours[Mapper.Map<UI.ActionType>(a.ActionType)](CurrentUser).Make(a, ticketSet, evt))
                    .ToList();

            var ticket = ticketSet.Tickets.FirstOrDefault();

            //TODO
            //if (ticket != null)
            //    evt.DocId = Convert.ToInt32(ticket["Id"]);

            return evt;
        }
    }
}
