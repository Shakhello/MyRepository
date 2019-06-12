using System;
using System.Linq;
using AutoMapper;
using CMS.DAL.Models;
using CMS.UI;
using Common.ExtensionMethods;

namespace CMS.Behaviours
{
    internal class LabelControlBehaviour : Behaviour, IControlBehaviour
    {
        public LabelControlBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public UI.Control Make(ControlDefinition definition, DbSearchResponse ticketSet, UI.Instance parentNode, Action<UI.Control> controlAction)
        {
            var control = new UI.Control
            {
                ControlType = Mapper.Map<UI.ControlType>(definition.ControlType),
                ParentNode = parentNode
            };

            var ticket = ticketSet.Tickets.FirstOrDefault();

            var dalControl = (DAL.Models.Control)definition.Entity;

            control.Events = definition
                .Events
                .Select(e => BehaviourSelector.EventBehaviours[Mapper.Map<UI.EventType>(e.EventType)](CurrentUser).Make(e, ticketSet, control))
                .ToList();

            control.Props.Add("DisplayName", definition.DisplayName);
            control.Props.Add("Order", definition.OrderIndex);
            control.Props.Add("Width", definition.Width);
            control.Props.Add("Style", Mapper.Map<UI.StyleDefinition>(((DAL.Models.Control)definition.Entity).Style));

            if (dalControl.Field != null)
            {
                control.Props.Add("FieldId", dalControl.Field.Id);
            }

            if (ticket != null)
            {
                control.DocId = Convert.ToInt32(ticket[dalControl.Field.TemplateId, "Id"]);

                if (dalControl.Field.FieldType.In(DAL.Models.FieldType.Dictionary))
                {
                    control.Props.Add("DictionaryId", dalControl.Field.Dictionary.Id);
                    control.Props.Add("DictionaryName", dalControl.Field.Dictionary.Name);
                    control.Props.Add("DictionaryType", Enum.GetName(typeof(DAL.Models.DictionaryType), dalControl.Field.Dictionary.DictionaryType));

                    var options = DbDictionaryCache.GetDictionaryRecords(dalControl.Field.Dictionary);

                    var values = DbDictionaryCache.GetValues(dalControl.Field.Dictionary, dalControl.Field.Id, control.DocId.Value);
                    control.Value = string.Join(", ", values.Select(v => options[v]));

                }
                else if (dalControl.Field.FieldType == DAL.Models.FieldType.Flag)
                {
                    control.Value = (ticket[dalControl.Field] == null) ? string.Empty :
                        (bool)ticket[dalControl.Field] ? $"{definition.DisplayName}" : string.Empty;
                }
                else if (dalControl.Field.FieldType == DAL.Models.FieldType.DateTime)
                {
                    control.Value = (ticket[dalControl.Field] != null) ?
                        ((DateTime)ticket[dalControl.Field]).ToString("dd.MM.yyyy") : string.Empty;
                }
                else
                {
                    control.Value = ticket[dalControl.Field] ?? string.Empty;
                }
            }

            return control;
        }
    }
}
