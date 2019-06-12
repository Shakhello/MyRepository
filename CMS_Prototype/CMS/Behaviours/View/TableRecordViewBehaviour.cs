using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class TableRecordViewBehaviour : Behaviour, IViewBehaviour
    {
        public TableRecordViewBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public View Make(ViewDefinition definition, DAL.Models.DbSearchResponse ticketSet, View parentNode, Action<UI.View> viewAction)
        {
            var view = new UI.View()
            {
                ViewType = ViewType.TableRecord,
                ParentNode = parentNode,
                Virtual = true
            };

            view.Props.Add("GridIndex", 0);

            foreach (var controlGroup in definition.Controls.Where(c => c.GridIndex >= 0).GroupBy(c => c.GridIndex).OrderBy(c => c.Key))
            {
                view.Props["GridIndex"] = controlGroup.Key;

                var childView = new TableCellViewBehaviour(CurrentUser).Make(definition, ticketSet, view, null);
                view.ChildViews.Add(childView);
            }

            view.Props.Remove("GridIndex");

            return view;
        }
    }
}
