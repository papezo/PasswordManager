using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Data
{
    public class AccountDetailsContext : DbContext
    {
        public AccountDetailsContext(DbContextOptions<AccountDetailsContext> options)
            : base(options)
        {
        }

        // DbSet for AccountDetails
        public DbSet<AccountDetails> AccountDetails { get; set; }

        // DbSet for PasswordDetails
        public DbSet<PasswordDetails> PasswordDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountDetails>().ToTable("AccountDetails");
            modelBuilder.Entity<PasswordDetails>().ToTable("PasswordDetails");
        }
    }
}
