using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Cursos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Cursos.Commands.Handlers;

public class CreateCursoCommandHandler : IRequestHandler<CreateCursoCommand, Result<CursoResponse>>
{
    private readonly AppDbContext _context;
    private readonly ICursoRepository _cursoRepository;
    private readonly ILogger<CreateCursoCommandHandler> _logger;

    public CreateCursoCommandHandler(ICursoRepository cursoRepository, AppDbContext context,
        ILogger<CreateCursoCommandHandler> logger)
    {
        _cursoRepository = cursoRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<CursoResponse>> Handle(CreateCursoCommand command, CancellationToken cancellationToken)
    {
        var curso = command.Request.ToDomain();

        await _cursoRepository.AddAsync(curso, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CursoResponse>.Success(curso.ToResponse());
    }
}