using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CMS.UI;

namespace CMS.Behaviours
{
    internal class SectionSettingsButtonBehaviour : Behaviour, IControlBehaviour
    {
        public SectionSettingsButtonBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public UI.Control Make(ControlDefinition definition, DAL.Models.DbSearchResponse ticketSet, UI.Instance parentNode, Action<UI.Control> controlAction)
        {
            var button = new UI.Control()
            {
                Virtual = true,
                ParentNode = parentNode,
                ControlType = ControlType.SectionSettingsButton
            };

            var evt = new Event()
            {
                Virtual = true,
                EventType = EventType.Click,
                ParentNode = button
            };

            evt.Actions.Add(new GetSectionSettingsBehaviour(CurrentUser).Make(null, null, evt));

            button.Events.Add(evt);

            return button;
        }
    }
}
