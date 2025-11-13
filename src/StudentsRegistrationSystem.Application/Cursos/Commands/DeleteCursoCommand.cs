using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Application.Cursos.Commands;

public record DeleteCursoCommand(Guid Id) : IRequest<Result<bool>>;