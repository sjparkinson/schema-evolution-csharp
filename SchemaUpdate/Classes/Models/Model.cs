using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Data;
using SchemaUpdate.Classes.Providers;

namespace SchemaUpdate.Classes.Models
{
    public enum ModelType
    {
        Entity,

        Table
    }

    public class Model
    {
        public string Name { get; set; }

        public ModelType ModelType { get; set; }

        public List<Field> Fields { get; set; }

        public Model() { }

        public Model(Type entityType)
        {
            this.Name = entityType.Name;
            this.ModelType = Models.ModelType.Entity;

            this.Fields = this.GetEntityMembers(entityType);
        }

        public IEnumerable<Field> Diff(Model input)
        {
            return this.Fields.Where(f => input.Fields.Contains(f) == false);
        }

        private List<Field> GetEntityMembers(Type entityType)
        {
            var modelMembers = new List<Field>();

            foreach (var property in entityType.GetProperties())
            {
                modelMembers.Add(new Field
                    {
                        Name = property.Name,
                        ColumnProperty = Providers.ColumnProperty.Null,
                        DbType = Provider.TypeMap[property.PropertyType]
                    });
            }

            return modelMembers;
        }
    }
}
