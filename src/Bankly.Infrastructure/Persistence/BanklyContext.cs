using Microsoft.EntityFrameworkCore;
using Bankly.Domain.Entities;

namespace Bankly.Infrastructure.Persistence;

public class BanklyContext(DbContextOptions<BanklyContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountType> AccountTypes { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
     
        configurationBuilder.Properties<bool>().HaveConversion<int>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
     
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BanklyContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}