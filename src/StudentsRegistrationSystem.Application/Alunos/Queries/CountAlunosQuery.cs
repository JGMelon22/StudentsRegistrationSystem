using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Application.Alunos.Queries;

public record CountAlunosQuery : IRequest<Result<int>>;