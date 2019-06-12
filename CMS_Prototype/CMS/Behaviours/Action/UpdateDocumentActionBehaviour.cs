using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoMapper;
using CMS.UI;
using Common.ExtensionMethods;
using Newtonsoft.Json;

namespace CMS.Behaviours
{
    internal class UpdateDocumentActionBehaviour : Behaviour, IActionBehaviour
    {
        public UpdateDocumentActionBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public UI.Action Make(ActionDefinition definition, DAL.Models.DbSearchResponse ticketSet, Event parentNode)
        {
            var action = new UI.Action()
            {
                ActionType = Mapper.Map<UI.ActionType>(definition.ActionType),
                ParentNode = parentNode
            };

            action.DocId = parentNode.ParentNode.DocId;

            action.Parameters = definition
                .Parameters
                .Select(pDef => BehaviourSelector.ParameterBehaviours[Mapper.Map<UI.ParameterType>(pDef.ParameterType)](CurrentUser).Make(pDef, action))
                .ToList();

            return action;
        }

        public ActionResult Execute(UI.Action action)
        {
            var view = action.ViewData;

            var templateDef = DbEditorService.GetTemplate(Convert.ToInt32(view.Props["TemplateId"]));

            var controls = new List<UI.Control>();
            GetFlatListOfControls(action.ViewData, ref controls);

            var dictionaryControls = controls.Where(c => c.ControlType.In(ControlType.MultiSelect, ControlType.Select));
            var labelControls = controls.Where(c => c.ControlType.In(ControlType.Label));
            var referenceControls = controls.Where(c => c.ControlType.In(ControlType.Table));

            var dataControls = controls
                .Except(dictionaryControls)
                .Except(referenceControls)
                .Except(labelControls);

            var dataTableKeyValues = dataControls.Select(c => new KeyValuePair<string, object>((string)c.Props["Name"], c.Value)).ToList();
            Dictionary<string, object> values = dataTableKeyValues.ToDictionary(kv => kv.Key, kv => kv.Value);
            DbDocumentService.UpdateDocument(templateDef.Name, action.DocId.Value, values);

            ProcessDictionaryControls(dictionaryControls);

            return new ActionResult()
            {
                ActionType = action.ActionType,
                Success = true
            };
        }

        private void GetFlatListOfControls(UI.View view, ref List<UI.Control> controls)
        {
            if (view.Controls != null)
            {
                foreach (var control in view.Controls.Where(c => c.Props.ContainsKey("FieldId") && c.Props["FieldId"] != null))
                {
                    controls.Add(control);
                }
            }

            if (view.ChildViews != null)
            {
                // Использовать только незалинкованные View
                var childViews = view.ChildViews.Where(cv => !cv.Props.ContainsKey("LinkedFieldId") || cv.Props["LinkedFieldId"] == null);
                foreach (var childView in childViews)
                {
                    GetFlatListOfControls(childView, ref controls);
                }
            }
        }

        private void ProcessDictionaryControls(IEnumerable<UI.Control> controls)
        {
            foreach (var control in controls)
            {
                var fieldId = Convert.ToInt32(control.Props["FieldId"]);
                var docId = control.DocId;
                var dictId = control.Props["DictionaryId"];

                var dictType = GetEnumValue<DAL.Models.DictionaryType>((string)control.Props["DictionaryType"]);

                if (dictType == DAL.Models.DictionaryType.Int)
                {
                    var values = JsonConvert.DeserializeObject<int[]>(control.Value.ToString());
                    DbDictionaryCache.SetTicketDictionaryValues(fieldId, docId.Value, values);
                }
                else
                {
                    var values = JsonConvert.DeserializeObject<string[]>(control.Value.ToString());
                    DbDictionaryCache.SetTicketDictionaryValues(fieldId, docId.Value, values);
                }
            }
        }

    }
}
