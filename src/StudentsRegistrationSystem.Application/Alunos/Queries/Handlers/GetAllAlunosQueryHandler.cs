using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Alunos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Alunos.Queries.Handlers;

public class GetAllAlunosQueryHandler : IRequestHandler<GetAllAlunosQuery, Result<PagedResponseOffset<AlunoResponse>>>
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly ILogger<GetAllAlunosQueryHandler> _logger;

    public GetAllAlunosQueryHandler(IAlunoRepository alunoRepository, ILogger<GetAllAlunosQueryHandler> logger)
    {
        _alunoRepository = alunoRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResponseOffset<AlunoResponse>>> Handle(GetAllAlunosQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var pagedAlunos = await _alunoRepository.GetAllAsync(query.PageNumber, query.PageSize, cancellationToken);

            var alunosResponse = pagedAlunos.Data.Select(a => a.ToResponse()).ToList();

            var pagedResponse = new PagedResponseOffset<AlunoResponse>(
                alunosResponse,
                pagedAlunos.PageNumber,
                pagedAlunos.PageSize,
                pagedAlunos.TotalRecords
            );

            _logger.LogInformation("Consulta paginada de alunos realizada com sucesso. Página: {PageNumber}, Tamanho: {PageSize}, Total: {Total}",
                pagedAlunos.PageNumber, pagedAlunos.PageSize, pagedAlunos.TotalRecords);

            return Result<PagedResponseOffset<AlunoResponse>>.Success(pagedResponse);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao consultar alunos.");
            return Result<PagedResponseOffset<AlunoResponse>>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao consultar alunos.");
            return Result<PagedResponseOffset<AlunoResponse>>.Failure(Error.ServerError);
        }
    }
}