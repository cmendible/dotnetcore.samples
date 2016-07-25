namespace ConsoleApplication
{
    using System;
    using System.Security.Claims;
    using System.Security.Principal;
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

            // With this code we are going to simulate an authenticated user.
            ClaimsPrincipal.ClaimsPrincipalSelector = () =>
                {
                    return new ClaimsPrincipal(new ClaimsIdentity(new GenericIdentity("cmendibl3")));
                };

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
