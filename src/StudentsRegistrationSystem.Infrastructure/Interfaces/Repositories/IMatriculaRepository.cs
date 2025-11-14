using StudentsRegistrationSystem.Core.Matriculas.Domains.Entities;

namespace StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

public interface IMatriculaRepository : IRepository<Matricula>
{
    Task<bool> AlunoJaMatriculadoAsync(Guid alunoId, Guid cursoId, CancellationToken cancellationToken = default);
    Task<Matricula?> GetMatriculaAtivaAsync(Guid alunoId, Guid cursoId, CancellationToken cancellationToken = default);
}