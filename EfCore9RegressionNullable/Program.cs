using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EfCoreRegression
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Setting up the database...");
            using var context = new ApplicationDbContext();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            // Seed some data
            context.Projects.Add(new Project { MaxTimes = 15 });
            context.Projects.Add(new Project { MaxTimes = 20 });
            await context.SaveChangesAsync();

            Console.WriteLine("Running the query...");
            try
            {
                int? number = 10;
                var result = await context.Projects
                    .Select(p => new
                    {
                        IntValue = Math.Min(p.MaxTimes, number ?? 10) // This triggers the issue
                    })
                    .FirstAsync();

                Console.WriteLine($"Query Result: {result.IntValue}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred:");
                Console.WriteLine(ex);
            }
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Using SQLite for simplicity
            optionsBuilder.UseSqlite("Data Source=projects.db");
        }
    }

    public class Project
    {
        public int Id { get; set; }
        public int MaxTimes { get; set; }
    }
}