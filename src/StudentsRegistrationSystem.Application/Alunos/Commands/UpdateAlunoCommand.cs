using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Requests;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Application.Alunos.Commands;

public record UpdateAlunoCommand(Guid Id, AlunoRequest Request) : IRequest<Result<AlunoResponse>>;
