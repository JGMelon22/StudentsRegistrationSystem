using Microsoft.EntityFrameworkCore;
using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;
using StudentsRegistrationSystem.Core.Cursos.Domains.Entities;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Infrastructure.Repositories;

public class CursoRepository : Repository<Curso>, ICursoRepository
{
    public CursoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Aluno>> GetAlunosByCursoIdAsync(Guid cursoId, CancellationToken cancellationToken = default)
    {
        return await _context.Matriculas
            .Where(m => m.CursoId == cursoId && m.Ativa)
            .Include(m => m.Aluno)
            .Select(m => m.Aluno)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}