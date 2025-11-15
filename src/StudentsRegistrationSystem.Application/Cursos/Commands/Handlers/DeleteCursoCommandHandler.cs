using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Cursos.Commands.Handlers;

public class DeleteCursoCommandHandler : IRequestHandler<DeleteCursoCommand, Result<bool>>
{
    private readonly ICursoRepository _cursoRepository;
    private readonly AppDbContext _context;
    private readonly ILogger<DeleteCursoCommandHandler> _logger;

    public DeleteCursoCommandHandler(ICursoRepository cursoRepository, AppDbContext context, ILogger<DeleteCursoCommandHandler> logger)
    {
        _cursoRepository = cursoRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteCursoCommand command, CancellationToken cancellationToken)
    {
        try
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
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao excluir curso. CursoId: {CursoId}", command.Id);
            return Result<bool>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao excluir curso. CursoId: {CursoId}", command.Id);
            return Result<bool>.Failure(Error.ServerError);
        }
    }
}