using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Cursos.Commands.Handlers;

public class DeleteCursoCommandHandler : IRequestHandler<DeleteCursoCommand, Result<bool>>
{
    private readonly AppDbContext _context;
    private readonly ICursoRepository _cursoRepository;
    private readonly ILogger<DeleteCursoCommandHandler> _logger;

    public DeleteCursoCommandHandler(ICursoRepository cursoRepository, AppDbContext context,
        ILogger<DeleteCursoCommandHandler> logger)
    {
        _cursoRepository = cursoRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteCursoCommand command, CancellationToken cancellationToken)
    {
        var curso = await _cursoRepository.GetByIdAsync(command.Id, cancellationToken);

        if (curso == null)
        {
            _logger.LogWarning("Curso não encontrado para exclusão. CursoId: {CursoId}", command.Id);
            return Result<bool>.Failure(Error.CourseNotFound);
        }

        _cursoRepository.Delete(curso);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}