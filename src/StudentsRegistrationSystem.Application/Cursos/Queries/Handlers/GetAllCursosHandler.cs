using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Cursos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Cursos.Queries.Handlers;

public class GetAllCursosHandler : IRequestHandler<GetAllCursosQuery, Result<IEnumerable<CursoResponse>>>
{
    private readonly ICursoRepository _cursoRepository;
    private readonly ILogger<GetAllCursosHandler> _logger;

    public GetAllCursosHandler(ICursoRepository cursoRepository, ILogger<GetAllCursosHandler> logger)
    {
        _cursoRepository = cursoRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<CursoResponse>>> Handle(GetAllCursosQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var cursos = await _cursoRepository.GetAllAsync(cancellationToken);
            return Result<IEnumerable<CursoResponse>>.Success(cursos.ToResponse());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error ao buscar todos os cursos.");
            return Result<IEnumerable<CursoResponse>>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar todos os cursos.");
            return Result<IEnumerable<CursoResponse>>.Failure(Error.ServerError);
        }
    }
}