using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Application.Alunos.Queries;

public record GetAlunosMatriculadosQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedResponseOffset<AlunoResponse>>>;