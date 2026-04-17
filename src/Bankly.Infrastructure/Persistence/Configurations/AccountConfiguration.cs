using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bankly.Domain.Entities;

namespace Bankly.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.AccountNumber)
            .IsRequired();

        builder.HasIndex(a => a.AccountNumber)
            .IsUnique();

        builder.Property(a => a.Branch)
            .IsRequired();
        
        builder.Property(a => a.Balance)
            .IsRequired()
            .HasPrecision(18, 2); 

     
        builder.HasOne(a => a.User)
            .WithMany(u => u.Accounts)
            .HasForeignKey(a => a.UserId);
        
        builder.HasOne(a => a.AccountType)
            .WithMany(at => at.Accounts)
            .HasForeignKey(a => a.AccountTypeId);
   
        builder.HasMany(a => a.Cards)
            .WithOne(c => c.Account)
            .HasForeignKey(c => c.AccountId);
        
        builder.HasMany(a => a.Transactions)
            .WithOne(t => t.Account)
            .HasForeignKey(t => t.AccountId);
    }
}