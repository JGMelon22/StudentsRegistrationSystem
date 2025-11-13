using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;

namespace StudentsRegistrationSystem.Application.Matriculas.Queries;

public record GetAlunosByCursoQuery(Guid CursoId) : IRequest<IEnumerable<AlunoResponse>>;