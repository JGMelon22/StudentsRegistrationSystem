using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentsRegistrationSystem.Core.Matriculas.Domains.Entities;

namespace StudentsRegistrationSystem.Infrastructure.Data.Configurations;

public class MatriculaConfiguration : IEntityTypeConfiguration<Matricula>
{
    public void Configure(EntityTypeBuilder<Matricula> builder)
    {
        builder.ToTable("Matriculas");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.AlunoId)
            .IsRequired();

        builder.Property(m => m.CursoId)
            .IsRequired();

        builder.Property(m => m.DataMatricula)
            .IsRequired();

        builder.Property(m => m.Ativa)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt);

        builder.HasIndex(m => new { m.AlunoId, m.CursoId });
        builder.HasIndex(m => m.Ativa);
    }
}