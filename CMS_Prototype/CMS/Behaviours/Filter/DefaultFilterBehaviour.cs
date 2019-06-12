using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class DefaultFilterBehaviour : Behaviour, IFilterBehaviour
    {
        public DefaultFilterBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public Filter Make(FilterDefinition definition, View parentNode)
        {
            var filter = new UI.Filter()
            {
                FilterType = Mapper.Map<UI.FilterType>(definition.FilterType),
                Definition = definition
            };

            filter.Props = new Dictionary<string, object>()
            {
                { "Id", definition.Id },
                { "ViewId", definition.ViewId },
                { "Width", definition.Width },
                { "Order", definition.Order },
                { "DisplayName", !string.IsNullOrEmpty(definition.DisplayName) ? 
                    definition.DisplayName : string.Join(", ", ((DAL.Models.Filter)definition.Entity).Fields.Select(f => f.Name)) }
            };

            filter.Value = definition.DefaultValue;

            filter.Fields = definition
                .Fields
                .Select(fDef => BehaviourSelector.FieldBehaviours[Mapper.Map<UI.FieldType>(fDef.FieldType)](CurrentUser).Make(fDef, filter))
                .ToList();

            return filter;
        }
    }
}
