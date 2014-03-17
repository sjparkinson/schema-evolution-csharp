using SchemaUpdate.Classes.Models;
using SchemaUpdate.Classes.Providers;
using SchemaUpdate.Structs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SchemaUpdate.Classes
{
    public class SchemaUpdater<T> where T : Provider, new()
    {
        private readonly List<EntityTableLink> entityTableLinks = new List<EntityTableLink>();

        private readonly List<string> updateStatments = new List<string>();

        private readonly T provider = new T();

        private string connectionString;

        public SchemaUpdater(string connectionString)
        {
            this.connectionString = connectionString;

            this.provider.SetConnectionString(connectionString);
        }

        public void SubmitChanges()
        {
            Console.WriteLine("Beginning update...\n");

            // Work out the stuff.
            this.entityTableLinks.ForEach(e => GenerateUpdateStatments(e));

            // Run the list of update statments created.
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                foreach (string statment in this.updateStatments)
                {
                    using (SqlCommand command = new SqlCommand(statment, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }

            Console.WriteLine("\nUpdate complete.");
        }

        public SchemaUpdater<T> AddLink(Type entityType, string tableName)
        {
            Model persistantModel = this.provider.GetPersistantModel(tableName);

            this.AddLink(new EntityTableLink(new Model(entityType), persistantModel));

            return this;
        }

        private SchemaUpdater<T> AddLink(EntityTableLink link)
        {
            this.entityTableLinks.Add(link);

            return this;
        }

        private void GenerateUpdateStatments(EntityTableLink link)
        {
            string tableName = link.PersistantModel.Name;

            ITransformer transformer = this.provider.Transformer;

            // check table exists, create if not
            if (!transformer.TableExists(tableName))
            {
                this.updateStatments.Add(transformer.AddTableStatment(tableName, link.ConceptualModel));
            }
            else
            {
                // check tables columns, add missing
                var missingColumns = link.ConceptualModel.Diff(link.PersistantModel);

                foreach (var column in missingColumns)
                {
                    this.updateStatments.Add(transformer.AddColumnStatment(tableName, column.Name, column.DbType));
                }
            }
        }
    }
}
