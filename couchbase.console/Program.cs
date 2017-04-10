namespace couchbase.console
{
    using System;
    using Couchbase;

    class Program
    {
        static void Main(string[] args)
        {
            // Connect to cluster. Defaults to localhost
            using (var cluster = new Cluster())
            {
                // Open the beer sample bucket
                using (var bucket = cluster.OpenBucket("beer-sample"))
                {
                    // Create a new beer document
                    var document = new Document<dynamic>
                    {
                        Id = "Polar Ice",
                        Content = new
                        {
                            name = "Polar Ice",
                            brewery_id = "Polar"
                        }
                    };

                    // Insert the beer document
                    var result = bucket.Insert(document);
                    if (result.Success)
                    {
                        Console.WriteLine("Inserted document '{0}'", document.Id);
                    }

                    // Query the beer sample bucket and find the beer we just added.
                    using (var queryResult = bucket.Query<dynamic>("SELECT name FROM `beer-sample` WHERE brewery_id =\"Polar\""))
                    {
                        foreach (var row in queryResult.Rows)
                        {
                            Console.WriteLine(row);
                        }
                    }
                }
            }
        }
    }
}