using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Application.Alunos.Queries;

public record GetAlunosMatriculadosQuery : IRequest<Result<IEnumerable<AlunoResponse>>>;