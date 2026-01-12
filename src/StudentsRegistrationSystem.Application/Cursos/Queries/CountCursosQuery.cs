using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Application.Cursos.Queries;

public record CountCursosQuery : IRequest<Result<int>>;