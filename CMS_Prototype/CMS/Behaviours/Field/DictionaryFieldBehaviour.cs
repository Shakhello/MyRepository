using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class DictionaryFieldBehaviour : Behaviour, IFieldBehaviour
    {
        public DictionaryFieldBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public UI.Field Make(FieldDefinition definition, Filter parentNode)
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
                    { "Length", definition.Length },
                    { "DALDictionaryType", Enum.GetName(typeof(DAL.Models.DictionaryType), ((DAL.Models.Field)definition.Entity).Dictionary.DictionaryType) }
                }
            };

            return field;
        }
    }
}
