using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class DefaultParameterBehaviour : Behaviour, IParameterBehaviour
    {
        public DefaultParameterBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public Parameter Make(ParameterDefinition definition, UI.Action parentNode)
        {
            var p = new Parameter()
            {
                ParameterType = Mapper.Map<UI.ParameterType>(definition.ParameterType),
                ParentNode = parentNode
            };

            p.DefaultValue = definition.DefaultValue;

            p.Props = new Dictionary<string, object>()
            {
                { "Name", definition.Name },
                { "FieldId", definition.FieldId },
                { "ActionId", definition.ActionId },
                { "ControlId", definition.ControlId }
            };
            

            return p;
        }
    }
}
