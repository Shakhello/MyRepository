using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class MultiSelectControlBehaviour : Behaviour, IControlBehaviour
    {
        public MultiSelectControlBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public Control Make(ControlDefinition definition, DAL.Models.DbSearchResponse ticketSet, UI.Instance parentNode, Action<Control> controlAction)
        {
            var control = new UI.Control
            {
                ControlType = Mapper.Map<UI.ControlType>(definition.ControlType),
                ParentNode = parentNode
            };

            var ticket = ticketSet.Tickets.FirstOrDefault();

            var dalControl = (DAL.Models.Control)definition.Entity;

            if (ticket != null)
                control.DocId = Convert.ToInt32(ticket[dalControl.Field.TemplateId, "Id"]);

            control.Props.Add("DisplayName", definition.DisplayName);
            control.Props.Add("FieldId", dalControl.Field.Id);
            control.Props.Add("DictionaryId", dalControl.Field.Dictionary.Id);
            control.Props.Add("DictionaryName", dalControl.Field.Dictionary.Name);
            control.Props.Add("DictionaryType", Enum.GetName(typeof(DAL.Models.DictionaryType), dalControl.Field.Dictionary.DictionaryType));
            control.Props.Add("Order", definition.OrderIndex);
            control.Props.Add("Width", definition.Width);
            control.Props.Add("Style", Mapper.Map<UI.StyleDefinition>(((DAL.Models.Control)definition.Entity).Style));

            var options = DbDictionaryCache.GetDictionaryRecords(dalControl.Field.Dictionary);
            var controlOptions = options.Select(i => new KeyValuePair<object, string>(i.Key, i.Value)).ToList();
            control.Props.Add("Options", controlOptions);
            control.Value = DbDictionaryCache.GetValues(dalControl.Field.Dictionary, dalControl.Field.Id, control.DocId.Value);

            return control;
        }

    }
}
