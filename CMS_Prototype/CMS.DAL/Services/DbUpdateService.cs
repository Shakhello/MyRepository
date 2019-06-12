using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Common.Config;
using System.Linq;

namespace CMS.DAL.Services
{
    public abstract class DbUpdateService
    {
        private static readonly string _connectionString = Config.KEY("CMSConnectionString");

        protected void ExecuteDbContextTransaction(Action<CMSContext, DbContextTransaction> action)
        {
            using (var db = new CMSContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    action(db, transaction);
                    transaction.Commit();
                }
            }
        }

        protected void UseTransaction(Action<SqlTransaction> action)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    action(transaction);
                    transaction.Commit();
                }
            }
        }

        protected T ExecuteReader<T>(string query, IEnumerable<SqlParameter> parameters, Func<SqlDataReader, T> callback)
        {
            T result;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    if (parameters != null && parameters.Count() > 0)
                        command.Parameters.AddRange(parameters.ToArray());

                    command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        result = callback(reader);
                    }
                }
            }

            return result;
        }

        protected void BulkInsert(DataTable dt, string tableName, Dictionary<string, string> columnMappings = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlBulkCopy bulkCopy = new SqlBulkCopy
                (
                    connection,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null
                );

                if (columnMappings != null)
                    foreach (var mapping in columnMappings)
                        bulkCopy.ColumnMappings.Add(mapping.Key, mapping.Value);

                bulkCopy.DestinationTableName = tableName;
                connection.Open();

                bulkCopy.WriteToServer(dt);
                connection.Close();
            }
        }

        protected void ExecuteNonQuery(string statement, IEnumerable<SqlParameter> parameters, SqlTransaction transaction = null)
        {
            if (transaction == null)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(statement, connection))
                    {
                        if (parameters != null && parameters.Count() > 0)
                            command.Parameters.AddRange(parameters.ToArray());

                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (SqlCommand command = new SqlCommand(statement, transaction.Connection, transaction))
                {
                    if (parameters != null && parameters.Count() > 0)
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddRange(parameters.ToArray());
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        protected T ExecuteScalar<T>(string statement, IEnumerable<SqlParameter> parameters, SqlTransaction transaction = null)
        {
            T result;

            if (transaction == null)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(statement, connection))
                    {
                        if (parameters != null && parameters.Count() > 0)
                        {
                            foreach (var p in parameters)
                            {
                                if (p.Value != null)
                                    command.Parameters.AddWithValue(p.ParameterName, p.Value);
                                else
                                    command.Parameters.Add(p);

                            }
                        }

                        result = (T)command.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (SqlCommand command = new SqlCommand(statement, transaction.Connection, transaction))
                {
                    if (parameters != null && parameters.Count() > 0)
                    {
                        foreach (var p in parameters)
                        {
                            if (p.Value != null)
                                command.Parameters.AddWithValue(p.ParameterName, p.Value);
                            else
                                command.Parameters.Add(p);

                        }
                    }

                    result = (T)command.ExecuteScalar();
                }
            }

            return result;
        }
    }
}
