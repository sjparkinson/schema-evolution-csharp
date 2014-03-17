using SchemaUpdate.Classes.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SchemaUpdate.Classes.Providers.SQLServer
{
    public class SqlServerProvider : Provider
    {
        public SqlServerProvider()
        {
            this.Transformer = new SqlServerTransformer();

            EnrollColumnType(DbType.StringFixedLength, "CHAR(255)");
            EnrollColumnType(DbType.String, "VARCHAR(255)");
            EnrollColumnType(DbType.Binary, "VARBINARY(8000)");
            EnrollColumnType(DbType.Boolean, "BIT");
            EnrollColumnType(DbType.Byte, "TINYINT");
            EnrollColumnType(DbType.Currency, "MONEY");
            EnrollColumnType(DbType.DateTime, "DATETIME");
            EnrollColumnType(DbType.Decimal, "DECIMAL(19,5)");
            EnrollColumnType(DbType.Double, "DOUBLE PRECISION");
            EnrollColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
            EnrollColumnType(DbType.Int16, "SMALLINT");
            EnrollColumnType(DbType.Int32, "INT");
            EnrollColumnType(DbType.Int64, "BIGINT");
            EnrollColumnType(DbType.Single, "REAL");
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
