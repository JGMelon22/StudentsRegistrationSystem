using Microsoft.EntityFrameworkCore;
using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;
using StudentsRegistrationSystem.Core.Cursos.Domains.Entities;
using StudentsRegistrationSystem.Core.Matriculas.Domains.Entities;
using StudentsRegistrationSystem.Infrastructure.Data.Configurations;

namespace StudentsRegistrationSystem.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Curso> Cursos { get; set; }
    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Matricula> Matriculas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AlunoConfiguration());
        modelBuilder.ApplyConfiguration(new CursoConfiguration());
        modelBuilder.ApplyConfiguration(new MatriculaConfiguration());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}