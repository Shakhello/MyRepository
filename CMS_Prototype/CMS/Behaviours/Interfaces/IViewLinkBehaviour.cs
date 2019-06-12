using CMS.UI;

namespace CMS.Behaviours
{
    public interface IViewLinkBehaviour
    {
        ViewLink Make(ViewDefinition definition, Section parentNode);
    }
}
