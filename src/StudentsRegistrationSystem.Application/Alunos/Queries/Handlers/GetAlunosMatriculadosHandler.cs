using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Alunos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Alunos.Queries.Handlers;

public class GetAlunosMatriculadosHandler : IRequestHandler<GetAlunosMatriculadosQuery, Result<IEnumerable<AlunoResponse>>>
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly ILogger<GetAlunosMatriculadosHandler> _logger;

    public GetAlunosMatriculadosHandler(IAlunoRepository alunoRepository, ILogger<GetAlunosMatriculadosHandler> logger)
    {
        _alunoRepository = alunoRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<AlunoResponse>>> Handle(GetAlunosMatriculadosQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var alunos = await _alunoRepository.GetAlunosMatriculadosAsync(cancellationToken);
            _logger.LogInformation("Consulta de alunos matriculados realizada com sucesso. Total: {Total}", alunos.Count());
            return Result<IEnumerable<AlunoResponse>>.Success(alunos.ToResponse());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao consultar alunos matriculados.");
            return Result<IEnumerable<AlunoResponse>>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao consultar alunos matriculados.");
            return Result<IEnumerable<AlunoResponse>>.Failure(Error.ServerError);
        }
    }
}