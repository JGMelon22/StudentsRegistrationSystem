using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

public interface IAlunoRepository : IRepository<Aluno>
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<PagedResponseOffset<Aluno>> GetAlunosMatriculadosAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}