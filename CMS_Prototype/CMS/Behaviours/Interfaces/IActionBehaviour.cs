using CMS.UI;

namespace CMS.Behaviours
{
    public interface IActionBehaviour
    {
        UI.Action Make(ActionDefinition definition, DAL.Models.DbSearchResponse ticketSet, Event parentNode);

        ActionResult Execute(UI.Action action);
    }
}
