using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAL.Models;
using CMS.Resources;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class SectionSettingsViewBehaviour : Behaviour, IViewBehaviour
    {
        public SectionSettingsViewBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public UI.View Make(ViewDefinition definition, DbSearchResponse ticketSet, UI.View parentNode, Action<UI.View> viewAction)
        {
            var view = new UI.View()
            {
                ViewType = UI.ViewType.SectionsSettings,
                Virtual = true,
                ParentNode = parentNode
            };

            view.Props.Add("DisplayName", RC.SECTION_SETTINGS_DISP_NAME);

            viewAction(view);

            return view;
        }
    }
}
