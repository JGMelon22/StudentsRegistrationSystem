using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Alunos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Alunos.Queries.Handlers;

public class GetAllAlunosHandler : IRequestHandler<GetAllAlunosQuery, Result<IEnumerable<AlunoResponse>>>
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly ILogger<GetAllAlunosHandler> _logger;

    public GetAllAlunosHandler(IAlunoRepository alunoRepository, ILogger<GetAllAlunosHandler> logger)
    {
        _alunoRepository = alunoRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<AlunoResponse>>> Handle(GetAllAlunosQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var alunos = await _alunoRepository.GetAllAsync(cancellationToken);
            _logger.LogInformation("Consulta de todos os alunos realizada com sucesso. Total: {Total}", alunos.Count());

            return Result<IEnumerable<AlunoResponse>>.Success(alunos.ToResponse());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao consultar todos os alunos.");
            return Result<IEnumerable<AlunoResponse>>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao consultar todos os alunos.");
            return Result<IEnumerable<AlunoResponse>>.Failure(Error.ServerError);
        }
    }
}