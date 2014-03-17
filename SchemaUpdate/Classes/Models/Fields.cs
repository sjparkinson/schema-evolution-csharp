using SchemaUpdate.Classes.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SchemaUpdate.Classes.Models
{
    public class Field : IEquatable<Field>
    {
        public string Name { get; set; }

        public DbType DbType { get; set; }

        public ColumnProperty ColumnProperty { get; set; }

        // Fields are equal if their names and field types are equal. 
        public bool Equals(Field field)
        {
            // Check whether the fields properties are equal. 
            return this.Name == field.Name && this.DbType == field.DbType;
        }
    }
}
