using SchemaUpdate.Classes.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SchemaUpdate.Classes.Providers.SQLite
{
    public class SqlLiteProvider : Provider
    {
        public SqlLiteProvider()
        {
            this.Transformer = new SqlLiteTransformer();

            EnrollColumnType(DbType.StringFixedLength, "TEXT");
            EnrollColumnType(DbType.String, "TEXT");
            EnrollColumnType(DbType.Boolean, "INTEGER");
            EnrollColumnType(DbType.Binary, "BLOB");
            EnrollColumnType(DbType.DateTime, "DATETIME");
            EnrollColumnType(DbType.Decimal, "NUMERIC");
            EnrollColumnType(DbType.Double, "NUMERIC");
            EnrollColumnType(DbType.Single, "NUMERIC");
            EnrollColumnType(DbType.Currency, "NUMERIC");
            EnrollColumnType(DbType.Int16, "INTEGER");
            EnrollColumnType(DbType.Int32, "INTEGER");
            EnrollColumnType(DbType.Int64, "INTEGER");
            EnrollColumnType(DbType.Byte, "INTEGER");
            EnrollColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
        }

        public override Model GetPersistantModel(string tableName)
        {
            Model model = new Model
            {
                Name = tableName,
                ModelType = ModelType.Table
            };

            string sqlStatment = string.Format("SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}';", tableName);

            List<Field> fields = new List<Field>();

            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sqlStatment, connection))
                using (var sqlReader = command.ExecuteReader())
                {
                    while (sqlReader.Read())
                    {
                        string dataType = string.Empty;

                        if (!sqlReader.IsDBNull(2))
                        {
                            dataType = string.Format("{0}({1})", sqlReader.GetString(1), sqlReader.GetInt32(2));
                        }
                        else
                        {
                            dataType = sqlReader.GetString(1);
                        }

                        fields.Add(new Field
                        {
                            Name = sqlReader.GetString(0),
                            DbType = this.GetColumnDbType(dataType),
                            ColumnProperty = ColumnProperty.Null
                        });
                    }
                }
            }

            model.Fields = fields;

            return model;
        }
    }
}
