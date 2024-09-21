using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp.Endpoints;
using WebApp.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AccountDetails> AccountDetails { get; set; }
    public DbSet<PasswordDetails> PasswordDetails { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AccountDetails>().ToTable("AccountDetails");
        modelBuilder.Entity<PasswordDetails>().ToTable("PasswordDetails");


        modelBuilder.Entity<PasswordDetails>()
            .HasOne(p => p.AccountDetails)
            .WithMany() 
            .HasForeignKey(p => p.AccountDetailsId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
