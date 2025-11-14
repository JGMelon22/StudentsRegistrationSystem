using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Alunos.Domains.Mappings;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Alunos.Queries.Handlers;

public class GetAllAlunosHandler : IRequestHandler<GetAllAlunosQuery, IEnumerable<AlunoResponse>>
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly ILogger<GetAllAlunosHandler> _logger;

    public GetAllAlunosHandler(IAlunoRepository alunoRepository, ILogger<GetAllAlunosHandler> logger)
    {
        _alunoRepository = alunoRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AlunoResponse>> Handle(GetAllAlunosQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var alunos = await _alunoRepository.GetAllAsync(cancellationToken);
            _logger.LogInformation("Consulta de todos os alunos realizada com sucesso. Total: {Total}", alunos.Count());
            return alunos.ToResponse();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao consultar todos os alunos.");
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao consultar todos os alunos.");
            return [];
        }
    }
}