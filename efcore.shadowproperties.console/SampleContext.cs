namespace ConsoleApplication
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using System.Security.Claims;

    /// <summary>
    /// The entity framework context with a Students DbSet 
    /// </summary>
    public class StudentsContext : DbContext
    {
        public StudentsContext(DbContextOptions<StudentsContext> options)
            : base(options)
        { }

        public DbSet<Student> Students { get; set; }

        /// <summary>
        /// We overrride OnModelCreating to map the audit properties for every entity marked with the 
        /// IAuditable interface.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(IAuditable).IsAssignableFrom(e.ClrType)))
            {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime>("CreatedAt");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime>("UpdatedAt");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property<string>("CreatedBy");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property<string>("UpdatedBy");
            }

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Override SaveChanges so we can call the new AuditEntities method.
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            this.AuditEntities();

            return base.SaveChanges();
        }
        
        /// <summary>
        /// Method that will set the Audit properties for every added or modified Entity marked with the 
        /// IAuditable interface.
        /// </summary>
        private void AuditEntities()
        {

            // Get the authenticated user name 
            string userName = string.Empty;

            var user = ClaimsPrincipal.Current;
            if (user != null)
            {
                var identity = user.Identity;
                if (identity != null)
                {
                    userName = identity.Name;
                }
            }

            // Get current date & time
            DateTime now = DateTime.Now;

            // For every changed entity marked as IAditable set the values for the audit properties
            foreach (EntityEntry<IAuditable> entry in ChangeTracker.Entries<IAuditable>())
            {
                // If the entity was added.
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedBy").CurrentValue = userName;
                    entry.Property("CreatedAt").CurrentValue = now;
                }
                else if (entry.State == EntityState.Modified) // If the entity was updated
                {
                    entry.Property("UpdatedBy").CurrentValue = userName;
                    entry.Property("UpdatedAt").CurrentValue = now;
                }
            }
        }
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
    /// A simple class representing a Student.
    /// We've marked this entity as IAditable so we can save audit info for the entity.
    /// </summary>
    public class Student : IAuditable
    {
        public Student()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }
    }
}