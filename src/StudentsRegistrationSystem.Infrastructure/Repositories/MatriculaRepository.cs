using Microsoft.EntityFrameworkCore;
using StudentsRegistrationSystem.Core.Matriculas.Domains.Entities;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Infrastructure.Repositories;

public class MatriculaRepository : Repository<Matricula>, IMatriculaRepository
{
    public MatriculaRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<Matricula?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Aluno)
            .Include(m => m.Curso)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<bool> AlunoJaMatriculadoAsync(Guid alunoId, Guid cursoId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            m => m.AlunoId == alunoId && m.CursoId == cursoId && m.Ativa,
            cancellationToken);
    }

    public async Task<Matricula?> GetMatriculaAtivaAsync(Guid alunoId, Guid cursoId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Aluno)
            .Include(m => m.Curso)
            .FirstOrDefaultAsync(
                m => m.AlunoId == alunoId && m.CursoId == cursoId && m.Ativa,
                cancellationToken);
    }
}