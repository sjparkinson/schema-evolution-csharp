using SchemaUpdate.Classes.Models;
using System.Data;

namespace SchemaUpdate.Classes.Providers
{
    public interface ITransformer
    {
        void SetConnectionString(string connectionString);

        bool TableExists(string tableName);

        string AddTableStatment(string tableName, Model conceptualModel);

        string AddColumnStatment(string tableName, string columnName, DbType dbtype);

        string AddColumnStatment(string tableName, string columnName, DbType dbType, ColumnProperty columnProperty);
    }
}
