using CMS.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Behaviours
{
    public interface IEventBehaviour
    {
        Event Make(EventDefinition definition, DAL.Models.DbSearchResponse ticketSet, Instance parentNode);

        EventResult Execute(UI.Event evt);
    }
}
