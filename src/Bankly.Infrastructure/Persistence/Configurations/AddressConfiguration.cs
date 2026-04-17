using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bankly.Domain.Entities;

namespace Bankly.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Street)
            .IsRequired();

        builder.Property(a => a.City)
            .IsRequired();

      

        builder.Property(a => a.ZipCode)
            .IsRequired();
    }
}