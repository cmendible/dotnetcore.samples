namespace ConsoleApplication
{
    using System;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            // Enable to app to read json setting files
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();

            // Get the connection string
            string connectionString = configuration.GetConnectionString("Sample");

            // Create a Student instance
            var user = new Student() { Name = "Carlos", LastName = "Mendible" };

            // Add and Save the student in the database
            using (var context = StudentsContextFactory.Create(connectionString))
            {
                context.Add(user);
                context.SaveChanges();
            }

            Console.WriteLine($"Student was saved in the database with id: {user.Id}");
        }
    }
}
