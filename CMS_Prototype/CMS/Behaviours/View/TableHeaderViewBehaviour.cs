using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class TableHeaderViewBehaviour : Behaviour, IViewBehaviour
    {
        public TableHeaderViewBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public View Make(ViewDefinition definition, DAL.Models.DbSearchResponse ticketSet, View parentNode, Action<UI.View> viewAction)
        {
            var view = new UI.View()
            {
                ViewType = ViewType.TableHeader,
                ParentNode = parentNode,
                Virtual = true
            };

            view.Controls = definition
                .Controls.Where(c => c.GridIndex >= 0).GroupBy(c => c.GridIndex).OrderBy(c => c.Key)
                .Select(c => new TableHeaderControlBehaviour(CurrentUser).Make(c.First(), ticketSet, view, null))
                .ToList();

            return view;
        }
    }
}
