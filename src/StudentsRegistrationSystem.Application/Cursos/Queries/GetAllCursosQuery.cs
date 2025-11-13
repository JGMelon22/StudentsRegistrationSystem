using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;

namespace StudentsRegistrationSystem.Application.Cursos.Queries;

public record GetAllCursosQuery : IRequest<IEnumerable<CursoResponse>>;
