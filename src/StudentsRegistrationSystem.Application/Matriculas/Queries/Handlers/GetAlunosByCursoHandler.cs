using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Alunos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Matriculas.Domains.Mappings;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Matriculas.Queries.Handlers;

public class GetAlunosByCursoHandler : IRequestHandler<GetAlunosByCursoQuery, IEnumerable<AlunoResponse>>
{
    private readonly ICursoRepository _cursoRepository;
    private readonly ILogger<GetAlunosByCursoHandler> _logger;

    public GetAlunosByCursoHandler(ICursoRepository cursoRepository, ILogger<GetAlunosByCursoHandler> logger)
    {
        _cursoRepository = cursoRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AlunoResponse>> Handle(GetAlunosByCursoQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var alunos = await _cursoRepository.GetAlunosByCursoIdAsync(query.CursoId, cancellationToken);
            return alunos.ToResponse();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error ao buscar alunos do curso. CursoId: {CursoId}", query.CursoId);
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar alunos do curso. CursoId: {CursoId}", query.CursoId);
            return [];
        }
    }
}