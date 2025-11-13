using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Application.Alunos.Commands;

public record DeleteAlunoCommand(Guid Id) : IRequest<Result<bool>>;
