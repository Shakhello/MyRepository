using CMS.UI;

namespace CMS.Behaviours
{
    public interface IFieldBehaviour
    {
        Field Make(FieldDefinition definition, Filter parentNode);
    }
}
