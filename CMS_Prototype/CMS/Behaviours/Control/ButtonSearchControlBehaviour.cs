using System;
using System.Collections.Generic;
using CMS.Resources;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class ButtonSearchControlBehaviour : Behaviour, IControlBehaviour
    {
        public ButtonSearchControlBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public Control Make(ControlDefinition definition, DAL.Models.DbSearchResponse ticketSet, UI.Instance parentNode, Action<Control> controlAction)
        {
            var button = new UI.Control()
            {
                Virtual = true,
                ParentNode = parentNode,
                ControlType = ControlType.ButtonSearch
            };

            button.Props = new Dictionary<string, object>()
            {
                { "DisplayName", RC.BTN_SEARCH },
                { "Width", 3 },
                { "Order", 0 },
                //{ "PageSize", ticketSet.PageSize.ToString() },
                //{ "PageNumber", "1" },
                //{ "SortFieldId", "Id"},
                //{ "SortDirection", "asc" },
                { "Style", GetStyle() }
            };

            var evt = new Event() { Virtual = true, EventType = EventType.Click, ParentNode = button };
            evt.Actions.Add(new SearchActionBehaviour(CurrentUser).Make(null, null, evt));

            button.Events.Add(evt);

            return button;
        }

        private string[] GetStyle()
        {
            var def = new StyleDefinition()
            {
                BackgroundColor = "success"
            };

            return def.GetPropertiesAsArray();
        }

    }
}
