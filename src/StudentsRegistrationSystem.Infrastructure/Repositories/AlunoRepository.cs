using Microsoft.EntityFrameworkCore;
using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Infrastructure.Repositories;

public class AlunoRepository : Repository<Aluno>, IAlunoRepository
{
    public AlunoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(a => a.Email == email);

        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<PagedResponseOffset<Aluno>> GetAlunosMatriculadosAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        int skip = (pageNumber - 1) * pageSize;

        var query = _context.Matriculas
            .Where(m => m.Ativa)
            .Select(m => m.Aluno)
            .Distinct()
            .AsNoTracking();

        int totalRecords = await query.CountAsync(cancellationToken);

        List<Aluno> data = await query
            .OrderBy(a => a.Nome)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponseOffset<Aluno>(data, pageNumber, pageSize, totalRecords);
    }
}