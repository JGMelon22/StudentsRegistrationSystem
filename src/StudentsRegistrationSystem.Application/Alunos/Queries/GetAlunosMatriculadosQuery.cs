using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;

namespace StudentsRegistrationSystem.Application.Alunos.Queries;


public record GetAlunosMatriculadosQuery : IRequest<IEnumerable<AlunoResponse>>;