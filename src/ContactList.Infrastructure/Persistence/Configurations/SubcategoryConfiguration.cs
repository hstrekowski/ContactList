using ContactList.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactList.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core mapping for <see cref="Subcategory"/>.
/// The relationship to <see cref="Category"/> is configured on the owning side in
/// <see cref="CategoryConfiguration"/>.
/// </summary>
public sealed class SubcategoryConfiguration : IEntityTypeConfiguration<Subcategory>
{
    public void Configure(EntityTypeBuilder<Subcategory> builder)
    {
        builder.ToTable("Subcategories");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}
