using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.UI;
using Common.ExtensionMethods;

namespace CMS.Behaviours
{
    internal class TableBodyViewBehaviour : Behaviour, IViewBehaviour
    {
        public TableBodyViewBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public View Make(ViewDefinition definition, DAL.Models.DbSearchResponse ticketSet, View parentNode, Action<UI.View> viewAction)
        {
            var view = new UI.View()
            {
                ViewType = ViewType.TableBody,
                ParentNode = parentNode,
                Virtual = true
            };

            view.Props = new Dictionary<string, object>()
            {
                { "TemplateId", definition.TemplateId }
            };

            if (definition.LinkedFieldId.HasValue)
                view.Props.Add("LinkedFieldId", definition.LinkedFieldId);

            foreach (var ticket in ticketSet.Tickets)
            {
                var newTicketSet = ticketSet.ReduceToOneTicket(ticket);
                var childView = new TableRecordViewBehaviour(CurrentUser).Make(definition, newTicketSet, view, null);
                view.ChildViews.Add(childView);
            }

            return view;
        }

    }
}
