using CMS.DAL.Common;

using CMS.DAL.Models;
using Common.Exceptions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace CMS.DAL.Services
{
    internal class DbDictionaryService : DbUpdateService
    {
        internal List<DictionaryLink> GetValuesForField(Dictionary dict, int fieldId)
        {
            using (var db = new CMSContext())
            {
                return db.DictionaryLinks.Where(link => link.FieldId == fieldId).ToList();
            }
        }

        internal void SetTicketDictionaryValues<T>(int fieldId, int docId, IEnumerable<T> values)
        {
            using (var db = new CMSContext())
            {
                var links = db.DictionaryLinks
                    .Where(link => link.DocId == docId && link.FieldId == fieldId);

                if (links.Count() > 0)
                    db.DictionaryLinks.RemoveRange(links);

                db.DictionaryLinks.AddRange(values.Select(v => new DictionaryLink()
                {
                    FieldId = fieldId,
                    DocId = docId,
                    DictionaryKeyString = (typeof(T) == typeof(string)) ? (string)(object)v : null,
                    DictionaryKeyInt = (typeof(T) == typeof(int) || typeof(T) == typeof(int?)) ? (int?)(object)v : null
                }));

                db.SaveChanges();
            }
        }

        internal Dictionary<object, string> GetDictionaryRecords(Dictionary dict)
        {
            var dictTableName = Constants.DICT_TABLE_PREFIX + dict.Name;

            var query = $"select [Id], [Description] from {dictTableName}";

            var list = new Dictionary<object, string>();
            return ExecuteReader(query, null, (reader) =>
            {
                while (reader.Read())
                    list.Add(reader["Id"], reader["Description"].ToString());

                return list;
            });
        }

        internal void AddDictionaryRecord<T>(Dictionary dict, T key, string value)
        {
            var dictTableName = Constants.DICT_TABLE_PREFIX + dict.Name;

            if (!typeof(T).IsValueType && EqualityComparer<T>.Default.Equals(key, default(T)))
                throw new CustomValidationException("Cannot insert empty key.");

            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@Id", key));
            sqlParams.Add(new SqlParameter("@Description", value));

            var command = $"insert into {dictTableName}(Id, Description) values(@Id, @Description)";

            ExecuteNonQuery(command, sqlParams);
        }

        internal void UpdateDictionaryRecord<T>(Dictionary dict, T key, string value)
        {
            var dictTableName = Constants.DICT_TABLE_PREFIX + dict.Name;

            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@Id", key));
            sqlParams.Add(new SqlParameter("@Description", value));

            var command = $"update {dictTableName} set Description = @Description where Id = @Id";

            ExecuteNonQuery(command, sqlParams);
        }

        internal void DeleteDictionaryRecord<T>(Dictionary dict, T key)
        {
            var dictTableName = Constants.DICT_TABLE_PREFIX + dict.Name;

            var command = $"delete from {dictTableName} where Id = @id";

            ExecuteNonQuery(command, new List<SqlParameter>() { { new SqlParameter("@Id", key) } });
        }


    }
}
