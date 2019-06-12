using System.Collections.Generic;
using System.Linq;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class DefaultViewLinkBehaviour : Behaviour, IViewLinkBehaviour
    {
        public DefaultViewLinkBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public ViewLink Make(ViewDefinition definition, Section parentNode)
        {
            var viewLink = new ViewLink()
            {
                ViewLinkType = ViewLinkType.Default,
                ParentNode = parentNode
            };

            viewLink.Props = new Dictionary<string, object>()
            {
                { "Name", definition.Name },
                { "DisplayName", definition.DisplayName }
            };

            viewLink.Events = new List<Event>()
            {
                new Event()
                {
                    EventType = EventType.Click,
                    Virtual = true,
                    Actions = new List<UI.Action>()
                    {
                        new UI.Action()
                        {
                            ActionType = ActionType.OpenSection,
                            Virtual = true,
                            Parameters = new List<Parameter>()
                            {
                                new Parameter()
                                {
                                    Virtual = true,
                                    ParameterType = ParameterType.FilterId,
                                    DefaultValue = definition
                                        .Filters
                                        .FirstOrDefault(f => f.ViewId == definition.Id)?
                                        .Id.ToString()
                                }
                            }
                        }
                    }
                }
            };

            return viewLink;
        }
    }
}
