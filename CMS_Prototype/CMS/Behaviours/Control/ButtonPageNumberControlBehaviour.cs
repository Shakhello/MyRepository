using System;
using System.Collections.Generic;
using AutoMapper;
using CMS.UI;

namespace CMS.Behaviours
{

    internal class ButtonPageNumberControlBehaviour : Behaviour, IControlBehaviour
    {
        public ButtonPageNumberControlBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public Control Make(UI.ControlDefinition definition, DAL.Models.DbSearchResponse ticketSet, UI.Instance parentNode, Action<Control> controlAction)
        {
            var button = new Control()
            {
                Virtual = true,
                ParentNode = parentNode,
                ControlType = ControlType.ButtonPageNumberControl
            };

            button.Props = new Dictionary<string, object>()
            {
                { "Width", 1 },
                { "PageSize", ticketSet.PageSize.ToString() },
                { "SortFieldId", ticketSet.SortField.Id.ToString()},
                { "SortDirection", ticketSet.SortDirection },
                { "Style", GetStyle() }
            };

            controlAction(button);

            button.Props.Add("PageNumber", button.Props["DisplayName"]);

            button.Value = button.Props["DisplayName"];

            var evt = new Event() { Virtual = true, EventType = EventType.Click, ParentNode = button };
            evt.Actions.Add(new SearchActionBehaviour(CurrentUser).Make(null, null, evt));

            button.Events.Add(evt);

            return button;
        }

        private string[] GetStyle()
        {
            return null;
        }
    }
}
