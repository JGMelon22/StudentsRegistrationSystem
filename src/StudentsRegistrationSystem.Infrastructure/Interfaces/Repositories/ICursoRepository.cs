using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;
using StudentsRegistrationSystem.Core.Cursos.Domains.Entities;

namespace StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

public interface ICursoRepository : IRepository<Curso>
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Aluno>> GetAlunosByCursoIdAsync(Guid cursoId, CancellationToken cancellationToken = default);
}