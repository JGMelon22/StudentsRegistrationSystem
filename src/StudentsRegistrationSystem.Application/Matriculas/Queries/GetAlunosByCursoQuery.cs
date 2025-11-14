using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Application.Matriculas.Queries;

public record GetAlunosByCursoQuery(Guid CursoId, int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedResponseOffset<AlunoResponse>>>;