using PrototypeTwo.Entitys;
using SchemaUpdate.Classes;
using SchemaUpdate.Classes.Providers;
using SchemaUpdate.Classes.Providers.SQLServer;
using System;
using System.Configuration;

namespace PrototypeTwo
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = GetConnectionStringByName("WindowsAzure");

            var schemaUpdate = new SchemaUpdater<SqlServerProvider>(connectionString);

            // Feel free to create new classes and add new links,
            // or edit the current entity classes to see how it handles.

            schemaUpdate.AddLink(typeof(Account), "Accounts");

            schemaUpdate.SubmitChanges();

            Console.ReadLine();
        }

        static string GetConnectionStringByName(string name)
        {
            // Assume failure. 
            string returnValue = null;

            // Look for the name in the connectionStrings section.
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];

            // If found, return the connection string. 
            if (settings != null) returnValue = settings.ConnectionString;

            return returnValue;
        }
    }
}
