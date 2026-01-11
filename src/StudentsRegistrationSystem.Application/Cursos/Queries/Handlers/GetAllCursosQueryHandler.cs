using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Cursos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Cursos.Queries.Handlers;

public class GetAllCursosQueryHandler : IRequestHandler<GetAllCursosQuery, Result<PagedResponseOffset<CursoResponse>>>
{
    private readonly ICursoRepository _cursoRepository;
    private readonly ILogger<GetAllCursosQueryHandler> _logger;

    public GetAllCursosQueryHandler(ICursoRepository cursoRepository, ILogger<GetAllCursosQueryHandler> logger)
    {
        _cursoRepository = cursoRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResponseOffset<CursoResponse>>> Handle(GetAllCursosQuery query,
        CancellationToken cancellationToken)
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
}