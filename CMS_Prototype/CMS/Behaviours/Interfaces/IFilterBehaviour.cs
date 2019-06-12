using CMS.UI;

namespace CMS.Behaviours
{
    public interface IFilterBehaviour
    {
        Filter Make(FilterDefinition definition, View parentNode);
    }
}
