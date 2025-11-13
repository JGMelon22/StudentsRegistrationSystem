using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Application.Matriculas.Commands;

public record RemoveMatriculaCommand(Guid AlunoId, Guid CursoId) : IRequest<Result<bool>>;