using Microsoft.EntityFrameworkCore;

namespace EFCoreDBOperationProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        public DbSet<Books> Books { get; set; }
        public DbSet<Language> Languages { get; set; }
    }
}
