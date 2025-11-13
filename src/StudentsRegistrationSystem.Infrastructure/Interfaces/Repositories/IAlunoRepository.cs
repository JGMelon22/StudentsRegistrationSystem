using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;

namespace StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

public interface IAlunoRepository : IRepository<Aluno>
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Aluno>> GetAlunosMatriculadosAsync(CancellationToken cancellationToken = default);
}