using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentsRegistrationSystem.Core.Cursos.Domains.Entities;

namespace StudentsRegistrationSystem.Infrastructure.Data.Configurations;

public class CursoConfiguration : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> builder)
    {
        builder.ToTable("Cursos");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Descricao)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.HasMany(c => c.Matriculas)
            .WithOne(m => m.Curso)
            .HasForeignKey(m => m.CursoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.Nome);
    }
}
