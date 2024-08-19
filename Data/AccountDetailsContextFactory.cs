using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WebApp.Data
{
    public class AccountDetailsContextFactory : IDesignTimeDbContextFactory<AccountDetailsContext>
    {
        public AccountDetailsContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AccountDetailsContext>();

            // Použijte SQLite connection string
            optionsBuilder.UseSqlite("Data Source=WebAppDb.db");

            return new AccountDetailsContext(optionsBuilder.Options);
        }
    }
}
