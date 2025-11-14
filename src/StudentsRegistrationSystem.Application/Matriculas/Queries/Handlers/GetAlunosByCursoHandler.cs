using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Alunos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Matriculas.Domains.Mappings;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Matriculas.Queries.Handlers;

public class GetAlunosByCursoHandler : IRequestHandler<GetAlunosByCursoQuery, Result<IEnumerable<AlunoResponse>>>
{
    private readonly ICursoRepository _cursoRepository;
    private readonly ILogger<GetAlunosByCursoHandler> _logger;

    public GetAlunosByCursoHandler(ICursoRepository cursoRepository, ILogger<GetAlunosByCursoHandler> logger)
    {
        _cursoRepository = cursoRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<AlunoResponse>>> Handle(GetAlunosByCursoQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var cursoExists = await _cursoRepository.ExistsAsync(query.CursoId, cancellationToken);

            if (!cursoExists)
            {
                _logger.LogWarning("Curso não encontrado ao buscar alunos. CursoId: {CursoId}", query.CursoId);
                return Result<IEnumerable<AlunoResponse>>.Failure(Error.CourseNotFound);
            }

            var alunos = await _cursoRepository.GetAlunosByCursoIdAsync(query.CursoId, cancellationToken);
            return Result<IEnumerable<AlunoResponse>>.Success(alunos.ToResponse());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error ao buscar alunos do curso. CursoId: {CursoId}", query.CursoId);
            return Result<IEnumerable<AlunoResponse>>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar alunos do curso. CursoId: {CursoId}", query.CursoId);
            return Result<IEnumerable<AlunoResponse>>.Failure(Error.ServerError);
        }
    }
}