using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Cursos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Cursos.Queries.Handlers;

public class GetCursoByIdQueryHandler : IRequestHandler<GetCursoByIdQuery, Result<CursoResponse>>
{
    private readonly ICursoRepository _cursoRepository;
    private readonly ILogger<GetCursoByIdQueryHandler> _logger;

    public GetCursoByIdQueryHandler(ICursoRepository cursoRepository, ILogger<GetCursoByIdQueryHandler> logger)
    {
        _cursoRepository = cursoRepository;
        _logger = logger;
    }

    public async Task<Result<CursoResponse>> Handle(GetCursoByIdQuery query, CancellationToken cancellationToken)
    {
        var curso = await _cursoRepository.GetByIdAsync(query.Id, cancellationToken);

        if (curso == null)
        {
            _logger.LogWarning("Curso não encontrado. CursoId: {CursoId}", query.Id);
            return Result<CursoResponse>.Failure(Error.CourseNotFound);
        }

        return Result<CursoResponse>.Success(curso.ToResponse());
    }
}