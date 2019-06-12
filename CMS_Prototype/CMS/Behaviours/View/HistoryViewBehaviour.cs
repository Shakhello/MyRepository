using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class HistoryViewBehaviour : Behaviour, IViewBehaviour
    {
        public HistoryViewBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public UI.View Make(ViewDefinition definition, CMS.DAL.Models.DbSearchResponse ticketSet, View parentNode, Action<UI.View> viewAction)
        {
            var view = new View()
            {
                ViewType = Mapper.Map<UI.ViewType>(definition.ViewType),
                ParentNode = parentNode
            };

            var ticket = ticketSet.Tickets.FirstOrDefault();

            // TODO
            //view.DocId = (ticket != null) ? (int?)Convert.ToInt32(ticket["Id"]) : null;

            view.Props = new Dictionary<string, object>()
            {
                { "Name", definition.Name },
                { "TemplateId", definition.TemplateId },
                { "OrderIndex", definition.OrderIndex },
                { "GridWidth", definition.GridWidth },
                { "LinkedFieldId", definition.LinkedFieldId }
            };

            return view;
        }
    }
}
