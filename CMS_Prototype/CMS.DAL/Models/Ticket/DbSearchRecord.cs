using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Models
{
    public class DbSearchRecord
    {
        public object this[int templateId, string fieldName]
        {
            get
            {
                var field = Values.Keys.FirstOrDefault(k => k.TemplateId == templateId && k.Name.ToUpper() == fieldName.ToUpper());

                return (field != null) ? Values[field] : null;
            }
        }

        public object this[Field field]
        {
            get => Values[field];
            set => Values[field] = value;
        }

        private Dictionary<Field, object> Values { get; set; } = new Dictionary<Field, object>();

        internal DbSearchResponse Response { get; set; }

        internal DbSearchRecord(SqlDataReader reader, List<KeyValuePair<string, Field>> fields, DbSearchResponse response)
        {
            Response = response;

            for (var col = 0; col < reader.FieldCount; col++)
            {
                if (fields.Select(kv => kv.Key).Contains(reader.GetName(col)))
                {
                    var field = fields.FirstOrDefault(kv => kv.Key == reader.GetName(col)).Value;

                    if (!Values.ContainsKey(field))
                        Values.Add(field, reader[col].GetType() != typeof(DBNull) ? reader[col] : null);
                }
            }
        }
    }
}
