using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Application.Cursos.Queries;

public record GetAllCursosQuery : IRequest<Result<IEnumerable<CursoResponse>>>;
