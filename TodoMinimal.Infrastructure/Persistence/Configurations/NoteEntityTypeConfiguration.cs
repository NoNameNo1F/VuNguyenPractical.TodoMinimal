using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TodoMinimal.Domain.Notes;

namespace TodoMinimal.Infrastructure.Persistence.Configurations
{
    public class NoteEntityTypeConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.ToTable("Notes", "note");

            builder.HasKey(static n => n.Id);

            // builder.Property(static e => e.Id)
            //     .HasConversion(
            //     new ValueConverter<NoteId, int>(
            //         id => id.Value,
            //         value => new NoteId(value)
            //     ))
            //     .ValueGeneratedOnAdd();

            builder.Property(static n => n.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            builder.Property(static n => n.Content)
                .HasColumnName("Content")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(static n => n.CreatedAt)
                .HasColumnName("CreatedAt");

            builder.Property(static n => n.ModifiedAt)
                .HasColumnName("ModifiedAt");
        }
    }
}