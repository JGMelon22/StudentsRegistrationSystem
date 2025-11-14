using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Cursos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Cursos.Queries.Handlers;

public class GetAllCursosHandler : IRequestHandler<GetAllCursosQuery, Result<PagedResponseOffset<CursoResponse>>>
{
    private readonly ICursoRepository _cursoRepository;
    private readonly ILogger<GetAllCursosHandler> _logger;

    public GetAllCursosHandler(ICursoRepository cursoRepository, ILogger<GetAllCursosHandler> logger)
    {
        _cursoRepository = cursoRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResponseOffset<CursoResponse>>> Handle(GetAllCursosQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var pagedCursos = await _cursoRepository.GetAllAsync(query.PageNumber, query.PageSize, cancellationToken);

            var cursosResponse = pagedCursos.Data.Select(c => c.ToResponse()).ToList();

            var pagedResponse = new PagedResponseOffset<CursoResponse>(
                cursosResponse,
                pagedCursos.PageNumber,
                pagedCursos.PageSize,
                pagedCursos.TotalRecords
            );

            return Result<PagedResponseOffset<CursoResponse>>.Success(pagedResponse);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error ao buscar todos os cursos.");
            return Result<PagedResponseOffset<CursoResponse>>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar todos os cursos.");
            return Result<PagedResponseOffset<CursoResponse>>.Failure(Error.ServerError);
        }
    }
}