using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Application.Matriculas.Queries;

public record GetAlunosByCursoQuery(Guid CursoId) : IRequest<Result<IEnumerable<AlunoResponse>>>;