using System;
using System.Collections.Generic;
using System.IO;
using Couchbase.Lite;
using Couchbase.Lite.Query;

namespace couchbase.lite
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get current directory to store the database.
            var dir = Directory.GetCurrentDirectory();

            // Delete the database so we can run the sample without issues.
            DatabaseFactory.DeleteDatabase("db", dir);

            // Create the default options.
            var options = DatabaseOptions.Default;
            options.Directory = dir;

            // Create the database
            var db = DatabaseFactory.Create("db", options);

            // Create a document the Id == "Polar Ice" an set properties. 
            var document = db.GetDocument("Polar Ice");
            document.Properties = new Dictionary<string, object>
            {
                ["name"] = "Polar Ice",
                ["brewery_id"] = "Polar"
            };

            // Save the document
            document.Save();

            // Query for the document abd write results to the console.
            var query = QueryFactory.Select()
                .From(DataSourceFactory.Database(db))
                .Where(ExpressionFactory.Property("brewery_id").EqualTo("Polar"));

            var rows = query.Run();
            foreach (var row in rows)
            {
                Console.WriteLine($"Fetched doc with id :: {row.DocumentID}");
            }
        }
    }
}