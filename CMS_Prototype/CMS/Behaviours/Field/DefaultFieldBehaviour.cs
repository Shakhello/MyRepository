using System;
using System.Collections.Generic;
using AutoMapper;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class DefaultFieldBehaviour : Behaviour, IFieldBehaviour
    {
        public DefaultFieldBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public Field Make(FieldDefinition definition, Filter parentNode)
        {
            var field = new Field()
            {
                FieldType = Mapper.Map<UI.FieldType>(definition.FieldType),
                ParentNode = parentNode,
                Props = new Dictionary<string, object>()
                {
                    { "Id", definition.Id },
                    { "Name", definition.Name },
                    { "DALFieldType", Enum.GetName(typeof(DAL.Models.FieldType), definition.FieldType) },
                    //{ "DALSQLType", ((DAL.Models.Field)definition.Entity).GetSQLType() },
                    { "Length", definition.Length }
                }
            };

            return field;
        }

    }
}
