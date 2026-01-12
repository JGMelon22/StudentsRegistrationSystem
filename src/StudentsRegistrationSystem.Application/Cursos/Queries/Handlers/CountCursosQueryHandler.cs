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
        var amount = await _cursoRepository.CountAsync();

        _logger.LogInformation("A total of {Amount} courses has been retrieved.", amount);

        return Result<int>.Success(amount);
    }
}