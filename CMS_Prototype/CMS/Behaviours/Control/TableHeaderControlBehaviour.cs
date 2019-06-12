using System;
using System.Collections.Generic;
using CMS.UI;
using Common.ExtensionMethods;

namespace CMS.Behaviours
{
    internal class TableHeaderControlBehaviour : Behaviour, IControlBehaviour
    {
        public TableHeaderControlBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public Control Make(ControlDefinition definition, DAL.Models.DbSearchResponse ticketSet, UI.Instance parentNode, Action<UI.Control> controlAction)
        {
            var header = new Control()
            {
                ControlType = UI.ControlType.TableHeaderControl,
                ParentNode = parentNode,
                Virtual = true
            };

            header.Props = new Dictionary<string, object>()
            {
                { "DisplayName", definition.DisplayName },
                { "Width", definition.Width },
                { "Order", definition.OrderIndex },
                { "Style", GetStyle() }
            };

            var dalControl = (DAL.Models.Control)definition.Entity;

            if (dalControl.FieldId.HasValue && !dalControl.Field.FieldType.In(DAL.Models.FieldType.Dictionary, DAL.Models.FieldType.File))
            {
                var controlFieldId = ((DAL.Models.Control)definition.Entity).Field.Id;

                header.Props.Add("IsCurrentSortControl", ticketSet.SortField.Id == controlFieldId);

                header.Props.Add("PageSize", ticketSet.PageSize.ToString());
                header.Props.Add("PageNumber", ticketSet.PageNumber.ToString());
                header.Props.Add("SortFieldId", ((DAL.Models.Control)definition.Entity).Field?.Id.ToString());
                header.Props.Add("SortDirection", (ticketSet.SortField.Id != controlFieldId) ? "asc" :
                        ticketSet.SortDirection == "asc" ? "desc" : "asc");

                var evt = new Event() { Virtual = true, EventType = EventType.Click, ParentNode = header };
                evt.Actions.Add(new SearchActionBehaviour(CurrentUser).Make(null, null, evt));

                header.Events.Add(evt);

            }

            return header;
        }

        private string[] GetStyle()
        {
            return null;
        }
    }
}
