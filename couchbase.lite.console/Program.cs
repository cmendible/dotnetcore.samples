using System;
using System.Collections.Generic;
using System.IO;
using Couchbase.Lite;

// https://developer.couchbase.com/documentation/mobile/2.0/guides/couchbase-lite/index.html?language=csharp#getting-started

namespace couchbase.lite
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = Path.Combine(Path.GetTempPath().Replace("cache", "files"), "CouchbaseLite");
            DatabaseFactory.DeleteDatabase("db", dir);

            var options = DatabaseOptions.Default;
            options.Directory = dir;

            var db = DatabaseFactory.Create("db", options);

            db.InBatch(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    var doc = db.CreateDocument();
                    doc.Properties = new Dictionary<string, object>
                    {
                        ["type"] = "user",
                        ["name"] = $"user {i}"
                    };
                    doc.Save();
                    Console.WriteLine($"saved user document {doc.GetString("name")}");
                }
                return true;
            });

            // Have to wait for Developer Build 004
            // var query = QueryFactory.Select()
            //     .From(DataSourceFactory.Database(db))
            //     .Where(
            //         ExpressionFactory.Property("type").EqualTo("user")
            //         .And(ExpressionFactory.Property("admin").EqualTo(false))
            //     );

            // var rows = query.Run();
            // foreach (var row in rows)
            // {
            //     Console.WriteLine($"doc ID :: ${row.DocumentID}");
            // }
        }
    }
}