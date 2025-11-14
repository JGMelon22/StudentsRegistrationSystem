using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Alunos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Matriculas.Queries.Handlers;

public class GetAlunosByCursoHandler : IRequestHandler<GetAlunosByCursoQuery, Result<PagedResponseOffset<AlunoResponse>>>
{
    private readonly ICursoRepository _cursoRepository;
    private readonly ILogger<GetAlunosByCursoHandler> _logger;

    public GetAlunosByCursoHandler(ICursoRepository cursoRepository, ILogger<GetAlunosByCursoHandler> logger)
    {
        _cursoRepository = cursoRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResponseOffset<AlunoResponse>>> Handle(GetAlunosByCursoQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var cursoExists = await _cursoRepository.ExistsAsync(query.CursoId, cancellationToken);

            if (!cursoExists)
            {
                _logger.LogWarning("Curso não encontrado ao buscar alunos. CursoId: {CursoId}", query.CursoId);
                return Result<PagedResponseOffset<AlunoResponse>>.Failure(Error.CourseNotFound);
            }

            var pagedAlunos = await _cursoRepository.GetAlunosByCursoIdAsync(
                query.CursoId,
                query.PageNumber,
                query.PageSize,
                cancellationToken);

            var alunosResponse = pagedAlunos.Data.Select(a => a.ToResponse()).ToList();

            var pagedResponse = new PagedResponseOffset<AlunoResponse>(
                alunosResponse,
                pagedAlunos.PageNumber,
                pagedAlunos.PageSize,
                pagedAlunos.TotalRecords
            );

            _logger.LogInformation("Consulta paginada de alunos por curso realizada com sucesso. CursoId: {CursoId}, Página: {PageNumber}, Tamanho: {PageSize}, Total: {Total}",
                query.CursoId, pagedAlunos.PageNumber, pagedAlunos.PageSize, pagedAlunos.TotalRecords);

            return Result<PagedResponseOffset<AlunoResponse>>.Success(pagedResponse);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error ao buscar alunos do curso. CursoId: {CursoId}", query.CursoId);
            return Result<PagedResponseOffset<AlunoResponse>>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar alunos do curso. CursoId: {CursoId}", query.CursoId);
            return Result<PagedResponseOffset<AlunoResponse>>.Failure(Error.ServerError);
        }
    }
}