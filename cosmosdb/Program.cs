using System;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace cosmosdb
{
    class Program
    {
        static void Main(string[] args)
        {
            // The endpoint to your cosmosdb instance
            var endpointUrl = "[THE ENPOINT OF YOUR COSMOSDB SERVICE HERE]";

            // The key to you cosmosdb
            var key = "[THE KEY OF YOUR COSMOSDB SERVICE HERE]";

            // The name of the database
            var databaseName = "Students";

            // The name of the collection of json documents
            var databaseCollection = "StudentsCollection";

            // Create a cosmosdb client
            using (var client = new DocumentClient(new Uri(endpointUrl), key))
            {
                // Create the database
                client.CreateDatabaseIfNotExistsAsync(new Database() { Id = databaseName }).GetAwaiter().GetResult();

                // Create the collection
                client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri(databaseName),
                    new DocumentCollection { Id = databaseCollection }).
                    GetAwaiter()
                    .GetResult();

                // Create a Student instance
                var student = new Student() { Id = "Student.1", Name = "Carlos", LastName = "Mendible" };

                // Sava the document to cosmosdb
                client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, databaseCollection), student)
                    .GetAwaiter().GetResult();

                Console.WriteLine($"Student was saved in the database with id: {student.Id}");

                // Query for the student by last name
                var query = client.CreateDocumentQuery<Student>(
                        UriFactory.CreateDocumentCollectionUri(databaseName, databaseCollection))
                        .Where(f => f.LastName == "Mendible")
                        .ToList();

                if (query.Any())
                {
                    Console.WriteLine("Student was found in the cosmosdb database");
                }

            }
        }
    }

    /// <summary>
    /// A simple class representing a Student
    /// </summary>
    public class Student
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }
    }
}
