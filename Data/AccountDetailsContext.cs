using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.Data;  
using System.Threading.Tasks;

namespace WebApp.Data
{
    public class AccountDetailsContext : DbContext
{
    public AccountDetailsContext(DbContextOptions<AccountDetailsContext> options)
        : base(options)
    {
    }

    public DbSet<AccountDetails> AccountDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountDetails>().ToTable("AccountDetails");
    }
}

}
