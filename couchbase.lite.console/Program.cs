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
            Database.Delete("db", dir);

            // Create the default options.
            var options = new DatabaseConfiguration
            {
                Directory = dir
            };

            // Create the database and a document with Id == "Polar Ice"
            using (var db = new Database("db", options))
            using(var document = new MutableDocument("Polar Ice"))
            {

                // Set properties on document
                document.SetString("name", "Polar Ice")
                    .SetString("brewery_id", "Polar");

                // Save the document
                db.Save(document);

                // Query for the document abd write results to the console.
                using (var query = QueryBuilder.Select(SelectResult.Expression(Meta.ID), SelectResult.All())
                    .From(DataSource.Database(db))
                    .Where(Expression.Property("brewery_id").EqualTo(Expression.String("Polar"))))
                {

                    var rows = query.Execute();
                    foreach (var row in rows)
                    {
                        Console.WriteLine($"Fetched doc with id :: {row.GetString(0)}");
                    }
                }
            }
        }
    }
}