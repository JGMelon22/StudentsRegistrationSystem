using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;
using StudentsRegistrationSystem.Core.Cursos.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

public interface ICursoRepository : IRepository<Curso>
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponseOffset<Aluno>> GetAlunosByCursoIdAsync(Guid cursoId, int PageNumber = 1, int PageSize = 10, CancellationToken cancellationToken = default);
}