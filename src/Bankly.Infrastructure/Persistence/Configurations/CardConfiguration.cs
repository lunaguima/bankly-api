using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bankly.Domain.Entities;

namespace Bankly.Infrastructure.Persistence.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {

        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.CardNumber)
            .IsRequired()
            .HasMaxLength(16); 
      
        builder.Property(c => c.Cvv)
            .IsRequired()
            .HasMaxLength(4);

      
        builder.Property(c => c.ExpirationDate)
            .IsRequired();

       
        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasColumnType("CHAR(1)")
            .HasDefaultValue('Y'); 

       
        builder.HasOne(c => c.Account)
            .WithMany(a => a.Cards)
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}