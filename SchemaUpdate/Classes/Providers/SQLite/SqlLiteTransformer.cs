using SchemaUpdate.Classes.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SchemaUpdate.Classes.Providers.SQLite
{
    public class SqlLiteTransformer : ITransformer
    {
        private string connectionString;

        private SqlLiteProvider provider;

        public void SetConnectionString(string connectionString)
        {
            if (!string.IsNullOrEmpty(this.connectionString))
            {
                throw new InvalidOperationException("Connection string has already been set.");
            }
            else
            {
                this.connectionString = connectionString;

                this.provider = new SqlLiteProvider();
            }
        }

        public bool TableExists(string tableName)
        {
            string sqlStatment = string.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}';", tableName);

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(sqlStatment, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    return reader.Read();
                }
            }
        }

        public string AddTableStatment(string tableName, Model conceptualModel)
        {
            Console.WriteLine(string.Format("Adding new table [{0}].", tableName));

            var columns = conceptualModel.Fields.Select(f => string.Format("{0} {1} {2}", f.Name, this.provider.GetColumnTypeSQL(f.DbType), this.provider.GetPropertySQL(f.ColumnProperty)));
            
            return string.Format("CREATE TABLE {0} ({1});", tableName, string.Join(", ", columns));
        }

        public string AddColumnStatment(string tableName, string columnName, DbType dbtype)
        {
            return this.AddColumnStatment(tableName, columnName, dbtype, ColumnProperty.Null);
        }

        public string AddColumnStatment(string tableName, string columnName, DbType dbType, ColumnProperty columnProperty)
        {
            Console.WriteLine(string.Format("Adding new column [{0}] to [{1}].", columnName, tableName));

            string sqlStatment = "ALTER TABLE {0} ADD {1} {2} {3};";

            return string.Format(sqlStatment, tableName, columnName, this.provider.GetColumnTypeSQL(dbType), this.provider.GetPropertySQL(columnProperty));
        }
    }
}
