using CMS.DAL.Models;
using System;


namespace CMS.Behaviours
{
    public interface IViewBehaviour
    {
        UI.View Make(UI.ViewDefinition definition, DbSearchResponse ticketSet, UI.View parentNode, Action<UI.View> viewAction);
    }
}
