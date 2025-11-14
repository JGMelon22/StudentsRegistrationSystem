using Microsoft.EntityFrameworkCore;
using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;
using StudentsRegistrationSystem.Core.Cursos.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;
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

    public async Task<PagedResponseOffset<Aluno>> GetAlunosByCursoIdAsync(Guid cursoId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = _context.Matriculas
            .Where(m => m.CursoId == cursoId && m.Ativa)
            .Include(m => m.Aluno)
            .Select(m => m.Aluno)
            .AsNoTracking();

        var totalRecords = await query.CountAsync(cancellationToken);

        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponseOffset<Aluno>(data, pageNumber, pageSize, totalRecords);
    }
}