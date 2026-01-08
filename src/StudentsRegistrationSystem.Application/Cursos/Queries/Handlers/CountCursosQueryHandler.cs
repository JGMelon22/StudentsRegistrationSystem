using System.Data.Common;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Cursos.Queries.Handlers;

public class CountCursosQueryHandler : IRequestHandler<CountCursosQuery, Result<int>>
{
    private readonly ICursoRepository _cursoRepository;
    private readonly ILogger<GetAllCursosQueryHandler> _logger;

    public CountCursosQueryHandler(ICursoRepository cursoRepository, ILogger<GetAllCursosQueryHandler> logger)
    {
        _cursoRepository = cursoRepository;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(CountCursosQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var amount = await _cursoRepository.CountAsync();

            return Result<int>.Success(amount);
        }
        catch (DbException ex)
        {
            _logger.LogError(ex, "Database error ao contar a quantidade de cursos.");
            return Result<int>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao contar a quantidade de cursos.");
            return Result<int>.Failure(Error.ServerError);
        }
    }
}