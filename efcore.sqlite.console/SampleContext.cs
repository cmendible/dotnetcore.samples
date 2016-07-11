namespace ConsoleApplication
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The entity framework context with a Students DbSet 
    /// </summary>
    public class StudentsContext : DbContext
    {
        public StudentsContext(DbContextOptions<StudentsContext> options)
            : base(options)
        { }

        public DbSet<Student> Students { get; set; }
    }

    /// <summary>
    /// A factory to create an instance of the StudentsContext 
    /// </summary>
    public static class StudentsContextFactory
    {
        public static StudentsContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StudentsContext>();
            optionsBuilder.UseSqlite(connectionString);

            var context = new StudentsContext(optionsBuilder.Options);
            context.Database.EnsureCreated();

            return context;
        }
    }

/// <summary>
    /// /// A simple class representing a Student
    /// </summary>
    public class Student
    {
        public Student()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }
    }
}