using CMS.DAL.Common;
using CMS.DAL.Interfaces;
using CMS.DAL.Models;
using Common.Exceptions;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CMS.DAL.Services
{
    public class DbDocumentService : DbUpdateService
    {
        public DbSearchResponse GetDocuments(View view, List<IFilterWithValue> filterValues, DbSearchRequestParams sortingAndPagingParams)
        {
            if (sortingAndPagingParams == null)
            {
                throw new ArgumentNullException("sortingAndPagingParams");
            }

            var searchRequest = new DbSearchRequest(view, filterValues, sortingAndPagingParams);

            var query = searchRequest.ToString();

            var result = ExecuteReader(query, null, (reader) =>
            {
                return new DbSearchResponse(reader, searchRequest);
            });

            return result;
        }

        public List<KeyValuePair<int, string>> GetDocuments(IEnumerable<Field> displayFields, IEnumerable<Field> searchFields, OperationType operationType, object value)
        {
            var searchRequest = new DbSearchRequest(displayFields, searchFields, operationType, value);

            var query = searchRequest.ToString();

            return ExecuteReader(query, null, (reader) =>
            {
                var result = new List<KeyValuePair<int, string>>();

                while (reader.Read())
                {
                    var docId = (int)reader[Constants.FIELD_ID];
                    var val = string.Join(", ", displayFields.Select(df => reader[df.Name].ToString()));

                    result.Add(new KeyValuePair<int, string>(docId, val));
                }

                return result;
            });
        }

        public void UpdateDocumentToDeleted(string templateName, int docId)
        {            
            UpdateDocument(templateName, docId, new Dictionary<string, object>() {
                { Constants.FIELD_IS_DELETED, true }
            });
        }

        public int CreateDocument(Dictionary<Field, object> values)
        {
            var id = 0;

            ExecuteDbContextTransaction((db, transaction) =>
            {
                var fields = values.Keys.ToList();

                var templateId = fields.First().TemplateId;

                var template = db.Templates.FirstOrDefault(t => t.Id == templateId);

                var dataTableName = Constants.DATA_TABLE_PREFIX + template.Name;
                var histTableName = Constants.HIST_TABLE_PREFIX + template.Name;

                var refFields = fields.Where(f => f.FieldType == FieldType.Reference);
                var dicFields = fields.Where(f => f.FieldType == FieldType.Dictionary);
                var fileFields = fields.Where(f => f.FieldType == FieldType.File);

                var dataFields = fields.Except(refFields).Except(dicFields).Except(fileFields);

                var namesValues = dataFields
                    .Select(f => new KeyValuePair<string, object>(f.Name, values[f]))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                var parameters = GetParameters(namesValues);

                var fieldNames = string.Join(", ", parameters.Select(p => $"[{p.ParameterName.Substring(1)}]"));
                var fieldValues = string.Join(", ", parameters.Select(p => $"{p.ParameterName}"));

                var commandA = $@"insert into {dataTableName}({fieldNames}) output inserted.Id values ({fieldValues})";
                var commandB = $"insert into {dataTableName} output inserted.Id default values";

                var trans = (SqlTransaction)transaction.UnderlyingTransaction;

                id = ExecuteScalar<int>(parameters.Count > 0 ? commandA : commandB, parameters, trans);

                // Формируем связи для ссылочных полей и вставялем их в БД
                foreach (var refField in refFields)
                {
                    int linkDocId = Convert.ToInt32(values[refField]);

                    int fieldId = refField.Id;

                    Field field = db.Fields.Include(f => f.LinkedField).FirstOrDefault(f => f.Id == fieldId);

                    if (field == null || field.FieldType != FieldType.Reference)
                    {
                        throw new CustomValidationException($"Field with id = {fieldId} does not exist or is not a reference field.");
                    }

                    Field twin = field.LinkedField;

                    db.TicketLinks.Add(new TicketLink() { FieldId = fieldId, DocId1 = id, DocId2 = linkDocId });
                    db.TicketLinks.Add(new TicketLink() { FieldId = twin.Id, DocId1 = linkDocId, DocId2 = id });
                }

                // Добавляем историю
                ExecuteHistory(template.Name, id, namesValues, 0, trans);

                db.SaveChanges();
            });

            return id;
        }

        private void ProcessReferenceFields(IEnumerable<Field> fields)
        {

        }

        public void UpdateDocument(string templateName, int docId, Dictionary<string, object> values)
        {
            var dataTableName = Constants.DATA_TABLE_PREFIX + templateName;

            var parameters = GetParameters(values);
            bool hasIdColumn = parameters.Any(p => p.ParameterName.ToUpper() == "ID");

            UseTransaction((transaction) =>
            {
                var fieldNamesAndValues = string.Join(", ", parameters.Select(p => $"[{p.ParameterName.Substring(1)}] = {p.ParameterName}"));
                var dataQuery = $"update {dataTableName} set {fieldNamesAndValues} where {Constants.FIELD_ID} = @Id";

                if (!hasIdColumn)
                    parameters.Add(new SqlParameter("@Id", docId));

                ExecuteNonQuery(dataQuery, parameters, transaction);

                // История
                UpdateHistory(templateName, docId, transaction);
            });

        }

        private void UpdateHistory(string templateName, int docId, SqlTransaction transaction)
        {
            //TODO: Уменьшить кол-во обращения в базу
            string dataTableName = Constants.DATA_TABLE_PREFIX + templateName;
            string histTableName = Constants.HIST_TABLE_PREFIX + templateName;

            string curTicketQuery = $"select top 1 * from {dataTableName} where {Constants.FIELD_ID} = {docId}";

            Dictionary<string, object> currentTicket = GetDocument(templateName, docId);

            // Узнаем текущую версию документа в истории
            string curVersionQuery = $"select top 1 VersionId from {histTableName} where DocId = {docId} order by VersionId desc";
            int versionId = ExecuteReader(curVersionQuery, null, (reader) => reader.Read() ? Convert.ToInt32(reader["VersionId"]) : 0);

            ExecuteHistory(templateName, docId, currentTicket, versionId, transaction);
        }

        private void ExecuteHistory(string templateName, int docId, Dictionary<string, object> currentTicket, int versionId, SqlTransaction transaction)
        {
            string histTableName = Constants.HIST_TABLE_PREFIX + templateName;

            // Определеяем дату изменения для истории
            currentTicket[Constants.FIELD_CHANGE_DATE] = DateTime.Now;

            // Формируем список параметров для истории с добавлением ссылки на документ и инкриментированной версии документа истории
            List<SqlParameter> parameters = GetParameters(currentTicket);
            parameters.Add(new SqlParameter("@docId", docId));
            parameters.Add(new SqlParameter("@versionId", versionId + 1));

            // Формируем SQL запрос и запускем его
            string fieldNames = string.Join(", ", parameters.Select(p => $"[{p.ParameterName.Substring(1)}]"));
            string fieldValues = string.Join(", ", parameters.Select(p => $"{p.ParameterName}"));
            string histQuery = $@"insert into {histTableName}({fieldNames}) values ({fieldValues})";
            ExecuteNonQuery(histQuery, parameters, transaction);
        }


        #region Non-GUI Api

        public Dictionary<string, object> GetDocument(string templateName, int docId)
        {
            var dataTableName = Constants.DATA_TABLE_PREFIX + templateName;

            var query = $"select * from {dataTableName} where {Constants.FIELD_ID} = @Id";

            return ExecuteReader(query, new List<SqlParameter>() { { new SqlParameter("@Id", docId) } }, (reader) =>
            {
                var values = new Dictionary<string, object>();

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        values.Add(reader.GetName(i), reader[i]);
                    }
                }

                return values;
            });
        }

        public List<Dictionary<string, object>> GetDocumentsByParameter(string templateName, string param, object value)
        {
            var dataTableName = Constants.DATA_TABLE_PREFIX + templateName;

            var query = $"select * from {dataTableName} where {param} = @p";

            return ExecuteReader(query, new List<SqlParameter>() { { new SqlParameter("@p", value) } }, (reader) =>
            {
                var result = new List<Dictionary<string, object>>();

                while (reader.Read())
                {
                    var values = new Dictionary<string, object>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        values.Add(reader.GetName(i), reader[i]);
                    }

                    result.Add(values);
                }

                return result;
            });
        }

        public void DeleteDocument(Template template, int docId)
        {
            var dataTableName = Constants.DATA_TABLE_PREFIX + template.Name;

            var command = $"delete from {dataTableName} where {Constants.FIELD_ID} = @id";

            ExecuteNonQuery(command, new List<SqlParameter>() { { new SqlParameter("@Id", docId) } });
        }

        public void CreateLink(int fieldId, int docId1, int docId2)
        {
            ExecuteDbContextTransaction((db, transaction) =>
            {
                var field = db.Fields.Include(f => f.LinkedField).FirstOrDefault(f => f.Id == fieldId);

                if (field == null || field.FieldType != FieldType.Reference)
                    throw new CustomValidationException($"Field with id = {fieldId} does not exist or is not a reference field.");

                var twin = field.LinkedField;

                db.TicketLinks.Add(new TicketLink() { FieldId = fieldId, DocId1 = docId1, DocId2 = docId2 });
                db.TicketLinks.Add(new TicketLink() { FieldId = twin.Id, DocId1 = docId2, DocId2 = docId1 });

                db.SaveChanges();
            });
        }

        public int? GetLinkedDocumentId(int fieldId, int docId1)
        {
            using (var db = new CMSContext())
            {
                return db.TicketLinks.FirstOrDefault(tl => tl.FieldId == fieldId && tl.DocId1 == docId1)?.DocId2;
            }
        }

        #endregion


        private void CheckForSystemFields(Dictionary<string, object> values)
        {
            if (!values.ContainsKey(Constants.FIELD_CREATE_DATE))
                values.Add(Constants.FIELD_CREATE_DATE, DateTime.Now);
            if (!values.ContainsKey(Constants.FIELD_CREATED_BY))
                values.Add(Constants.FIELD_CREATED_BY, "External API call");
            if (!values.ContainsKey(Constants.FIELD_IS_DRAFT))
                values.Add(Constants.FIELD_IS_DRAFT, true);
            if (!values.ContainsKey(Constants.FIELD_IS_DELETED))
                values.Add(Constants.FIELD_IS_DELETED, false);
            if (!values.ContainsKey(Constants.FIELD_IS_ARCHIVED))
                values.Add(Constants.FIELD_IS_ARCHIVED, false);
        }

        private List<SqlParameter> GetParameters(Dictionary<string, object> values)
        {
            var parameters = new List<SqlParameter>();

            if (values == null || values.Count == 0)
            {
                return parameters;
            }

            foreach (var kv in values.Where(v => v.Key != Constants.FIELD_ID))
            {
                if (kv.Value != null && kv.Value is string && (kv.Value as string).Length > 4000)
                {
                    parameters.Add(new SqlParameter($"@{kv.Key}", SqlDbType.NVarChar, -1)
                    {
                        Value = kv.Value
                    });
                }
                else
                {
                    parameters.Add(new SqlParameter($"@{kv.Key}", kv.Value ?? DBNull.Value));
                }
            }
            return parameters;
        }

    }
}
