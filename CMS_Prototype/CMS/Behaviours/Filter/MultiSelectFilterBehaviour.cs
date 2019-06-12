using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CMS.UI;
using Common.Exceptions;
using Common.ExtensionMethods;

namespace CMS.Behaviours
{
    internal class MultiSelectFilterBehaviour : Behaviour, IFilterBehaviour
    {
        public MultiSelectFilterBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public Filter Make(FilterDefinition definition, View parentNode)
        {
            var filter = new UI.Filter()
            {
                FilterType = Mapper.Map<UI.FilterType>(definition.FilterType),
                Definition = definition
            };

            var dalFilter = (DAL.Models.Filter)definition.Entity;

            if (dalFilter.Fields.Count != 1 && !dalFilter.Fields.First().FieldType.In(DAL.Models.FieldType.Dictionary))
                throw new CustomValidationException($"Dictionary filter (Id = {definition.Id}) must be linked to one dictionary field.");

            filter.Props.Add("Id", definition.Id);
            filter.Props.Add("Width", definition.Width);
            filter.Props.Add("Order", definition.Order);
            filter.Props.Add("ViewId", definition.ViewId);
            filter.Props.Add("FieldId", dalFilter.Fields.First().Id);

            filter.Props.Add("DisplayName", !string.IsNullOrEmpty(definition.DisplayName) ?
                definition.DisplayName : string.Join(", ", ((DAL.Models.Filter)definition.Entity).Fields.Select(f => f.Name)));

            var options = DbDictionaryCache.GetDictionaryRecords(dalFilter.Fields.First().Dictionary);
            var controlOptions = options.Select(i => new KeyValuePair<object, string>(i.Key, i.Value)).ToList();
            controlOptions.Insert(0, new KeyValuePair<object, string>(null, ""));

            filter.Props.Add("Options", controlOptions);

            filter.Fields = definition
                .Fields
                .Select(fDef => BehaviourSelector.FieldBehaviours[Mapper.Map<UI.FieldType>(fDef.FieldType)](CurrentUser).Make(fDef, filter))
                .ToList();


            return filter;
        }
    }
}
