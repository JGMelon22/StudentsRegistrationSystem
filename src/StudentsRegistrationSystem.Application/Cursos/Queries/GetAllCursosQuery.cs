using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Application.Cursos.Queries;

public record GetAllCursosQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedResponseOffset<CursoResponse>>>;
