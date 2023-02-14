using API_Movies.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace API_Movies.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Category> Category { get; set; }

        public DbSet<Movie> Movie { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<AppUser> AppUser { get; set; }
    }
}
