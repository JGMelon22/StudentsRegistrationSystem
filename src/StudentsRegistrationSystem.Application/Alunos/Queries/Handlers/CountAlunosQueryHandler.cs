using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Alunos.Queries.Handlers;

public class CountAlunosQueryHandler : IRequestHandler<CountAlunosQuery, Result<int>>
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly ILogger<GetAllAlunosQueryHandler> _logger;

    public CountAlunosQueryHandler(IAlunoRepository alunoRepository, ILogger<GetAllAlunosQueryHandler> logger)
    {
        _alunoRepository = alunoRepository;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(CountAlunosQuery request, CancellationToken cancellationToken)
    {
        var amount = await _alunoRepository.CountAsync();

        _logger.LogInformation("A total of {Amount} students has been retrieved.", amount);

        return Result<int>.Success(amount);
    }
}