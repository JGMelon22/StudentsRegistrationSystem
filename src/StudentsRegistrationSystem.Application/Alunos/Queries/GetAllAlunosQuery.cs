using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;

namespace StudentsRegistrationSystem.Application.Alunos.Queries;

public record GetAllAlunosQuery : IRequest<IEnumerable<AlunoResponse>>; 