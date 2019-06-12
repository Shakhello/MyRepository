using CMS.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Behaviours
{
    internal class DefaultSectionBehaviour : Behaviour, ISectionBehaviour
    {
        public DefaultSectionBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public Section Make(SectionDefinition definition)
        {
            var section = new Section()
            {
                SectionType = SectionType.Default,
                Definition = definition
            };


            section.Props = new Dictionary<string, object>()
            {
                { "Name", definition.Name },
                { "DisplayName", definition.DisplayName }
            };

            section.ViewLinks = definition
                .Views
                .Select(viewDef => BehaviourSelector.ViewLinkBehaviours[ViewLinkType.Default](CurrentUser).Make(viewDef, section))
                .ToList();

            section.SettingsButton = new SectionSettingsButtonBehaviour(CurrentUser).Make(null, null, section, null);

            return section;
        }
    }
}
