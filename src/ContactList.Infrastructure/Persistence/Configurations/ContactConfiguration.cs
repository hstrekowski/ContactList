using ContactList.Domain.Entities;
using ContactList.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactList.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core mapping for <see cref="Contact"/>.
/// Value objects (<see cref="Email"/>, <see cref="PhoneNumber"/>) are stored as strings;
/// </summary>
public sealed class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .IsRequired()
            .HasMaxLength(254);

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.Property(c => c.PhoneNumber)
            .HasConversion(
                phone => phone.Value,
                value => new PhoneNumber(value))
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.PasswordHash)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.DateOfBirth)
            .HasColumnType("date");

        builder.HasOne(c => c.Category)
            .WithMany()
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Subcategory)
            .WithMany()
            .HasForeignKey(c => c.SubcategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
