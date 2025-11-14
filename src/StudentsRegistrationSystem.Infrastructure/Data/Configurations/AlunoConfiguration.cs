using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;

namespace StudentsRegistrationSystem.Infrastructure.Data.Configurations;

public class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
{
    public void Configure(EntityTypeBuilder<Aluno> builder)
    {
        builder.ToTable("Alunos");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.DataNascimento)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt);

        builder.HasMany(a => a.Matriculas)
            .WithOne(m => m.Aluno)
            .HasForeignKey(m => m.AlunoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.Email)
            .IsUnique();

        builder.HasIndex(a => a.Nome);
    }
}
