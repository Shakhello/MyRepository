using System;
using System.Collections.Generic;
using System.Data;

namespace CMS.Behaviours
{
    public interface IControlBehaviour
    {
        UI.Control Make(UI.ControlDefinition definition, DAL.Models.DbSearchResponse ticketSet, UI.Instance parentNode, Action<UI.Control> controlAction);
    }
}
