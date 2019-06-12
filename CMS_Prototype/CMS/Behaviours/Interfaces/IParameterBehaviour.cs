using CMS.UI;

namespace CMS.Behaviours
{
    public interface IParameterBehaviour
    {
        Parameter Make(ParameterDefinition definition, UI.Action parentNode);

    }
}
