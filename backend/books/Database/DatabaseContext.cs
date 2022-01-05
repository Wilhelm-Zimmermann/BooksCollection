using Microsoft.EntityFrameworkCore;
using backend.Entities;

namespace backend.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        private string _connectionString = "Host=localhost;Database=books_collection;Username=books;Password=books123";

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
}
