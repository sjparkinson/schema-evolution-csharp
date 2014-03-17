using SchemaUpdate.Classes.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SchemaUpdate.Classes.Providers
{
    public enum ColumnProperty
    {
        Null,

        NotNull,

        Unique,

        PrimaryKey
    }

    public abstract class Provider
    {
        public static readonly Dictionary<Type, DbType> TypeMap = new Dictionary<Type, DbType>
        {
            { typeof(string), DbType.String },
            { typeof(bool), DbType.Boolean },
            { typeof(short), DbType.Int16 },
            { typeof(int), DbType.Int32 },
            { typeof(float), DbType.Single },
            { typeof(double), DbType.Double },
            { typeof(byte), DbType.Byte },
            { typeof(DateTime), DbType.DateTime },
            { typeof(Guid), DbType.Guid }
        };

        public virtual ITransformer Transformer { get; set; }

        private readonly Dictionary<ColumnProperty, string> propertyMap = new Dictionary<ColumnProperty, string>();

        private readonly Dictionary<DbType, string> columnTypeMap = new Dictionary<DbType, string>();

        protected string connectionString;

        protected Provider()
        {
            EnrollColumnProperty(ColumnProperty.Null, "NULL");
            EnrollColumnProperty(ColumnProperty.NotNull, "NOT NULL");
            EnrollColumnProperty(ColumnProperty.Unique, "UNIQUE");
            EnrollColumnProperty(ColumnProperty.PrimaryKey, "PRIMARY KEY");
        }

        public void SetConnectionString(string connectionString)
        {
            if (!string.IsNullOrEmpty(this.connectionString))
            {
                throw new InvalidOperationException("Connection string has already been set.");
            }
            else
            {
                this.connectionString = connectionString;
                this.Transformer.SetConnectionString(connectionString);
            }
        }
        
        public void EnrollColumnType(DbType type, string sql)
        {
            if (!columnTypeMap.ContainsKey(type))
            {
                columnTypeMap.Add(type, sql);
            }
            else
            {
                columnTypeMap[type] = sql;
            }
        }

        public void EnrollColumnProperty(ColumnProperty property, string sql)
        {
            if (!propertyMap.ContainsKey(property))
            {
                propertyMap.Add(property, sql);
            }
            else
            {
                propertyMap[property] = sql;
            }
        }

        public DbType GetColumnDbType(string sqlType)
        {
            if (!columnTypeMap.ContainsValue(sqlType.ToUpper()))
            {
                throw new KeyNotFoundException(string.Format("No Db type found for type: {0}.", sqlType));
            }

            return columnTypeMap.Single(c => c.Value == sqlType.ToUpper()).Key;
        }

        public string GetColumnTypeSQL(DbType type)
        {
            if (!columnTypeMap.ContainsKey(type))
            {
                throw new KeyNotFoundException(string.Format("No SQL found for DB type: {0}.", type));
            }

            return columnTypeMap[type];
        }

        public string GetPropertySQL(ColumnProperty property)
        {
            if (propertyMap.ContainsKey(property))
            {
                return propertyMap[property];
            }

            return string.Empty;
        }

        public virtual Model GetPersistantModel(string tableName)
        {
            throw new NotImplementedException();
        }
    }
}
