using CMS.DAL.Interfaces;
using CMS.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.UI
{
    public class Field : Instance
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public FieldType FieldType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public override DefinitionType Type => DefinitionType.Field;

        public DAL.Models.DictionaryType? GetDictionaryType()
        {
            return (DAL.Models.DictionaryType)Enum.Parse(typeof(DAL.Models.DictionaryType), (string)Props["DALDictionaryType"]);
        }

        public DAL.Models.FieldType GetFieldType()
        {
            return (DAL.Models.FieldType)Enum.Parse(typeof(DAL.Models.FieldType), (string)Props["DALFieldType"]);
        }

        public int? GetLength()
        {
            return (int?)Props["Length"];
        }

        public string GetName()
        {
            return (string)Props["Name"];
        }

        public string GetSQLType()
        {
            return (string)Props["DALSQLType"];
        }

        public int GetId()
        {
            return Convert.ToInt32(Props["Id"]);
        }

    }
}
