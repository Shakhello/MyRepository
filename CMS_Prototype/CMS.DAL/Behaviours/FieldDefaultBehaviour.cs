using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAL.Common;
using CMS.DAL.Interfaces;
using CMS.DAL.Models;
using CMS.DAL.Services;

namespace CMS.DAL.Behaviours
{
    internal class FieldDefaultBehaviour : ICRUDBehaviour<Field>
    {
        public void OnCreate(Field entity, CMSContext db, DbContextTransaction transaction)
        {
            var dataTableName = Constants.DATA_TABLE_PREFIX + entity.Template.Name;
            var historyTableName = Constants.HIST_TABLE_PREFIX + entity.Template.Name;

            var dataTableAlterCommand = GetAddColumnsScript(dataTableName, new List<Field>() { entity });
            var historyTableAlterCommand = GetAddColumnsScript(historyTableName, new List<Field>() { entity });
            db.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, dataTableAlterCommand, dataTableName);
            db.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, historyTableAlterCommand, historyTableName);
        }

        public void OnDelete(Field entity, CMSContext db, DbContextTransaction transaction)
        {
            var dataColumnDropCommand = $@"alter table {Constants.DATA_TABLE_PREFIX + entity.Template.Name} drop column [{entity.Name}]";
            var histColumnDropCommand = $@"alter table {Constants.HIST_TABLE_PREFIX + entity.Template.Name} drop column [{entity.Name}]";

            db.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, dataColumnDropCommand);
            db.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, histColumnDropCommand);
        }

        public void OnRead(Field entity, CMSContext db, DbContextTransaction transaction)
        {
            
        }

        public void OnUpdate(Field entity, CMSContext db, DbContextTransaction transaction)
        {
            
        }

        private string GetAddColumnsScript(string tableName, IEnumerable<Field> fields)
        {
            var query = string.Empty;

            var fieldList = new List<string>();

            if (fields != null && fields.Count() > 0)
            {
                fieldList.AddRange(GenerateColumns(fields));

                if (fields.Count() == 1)
                    query = string.Format("alter table {0} add {1};", tableName, string.Join(", ", fieldList));
                else
                    query = string.Format("alter table {0} add ({1});", tableName, string.Join(", ", fieldList));
            }

            return query;
        }

        internal static List<string> GenerateColumns(IEnumerable<Field> fields)
        {
            return fields.Select(f =>
                string.Format("[{0}] {1} {2}", f.Name,
                Constants.CMS_TO_SQL[f.FieldType].ToLower() != "nvarchar" ? Constants.CMS_TO_SQL[f.FieldType]
                : Constants.CMS_TO_SQL[f.FieldType] + "(" + f.Length + ")", "null")).ToList();
        }
    }
}
