using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Matriculas.Domains.DTOs.Requests;
using StudentsRegistrationSystem.Core.Matriculas.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.Application.Matriculas.Commands;

public record CreateMatriculaCommand(MatriculaRequest Request) : IRequest<Result<MatriculaResponse>>;