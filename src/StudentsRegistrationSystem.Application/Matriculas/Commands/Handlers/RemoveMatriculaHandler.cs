using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Matriculas.Commands.Handlers;

public class RemoveMatriculaHandler : IRequestHandler<RemoveMatriculaCommand, Result<bool>>
{
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly AppDbContext _context;
    private readonly ILogger<RemoveMatriculaHandler> _logger;

    public RemoveMatriculaHandler(
        IMatriculaRepository matriculaRepository,
        AppDbContext context,
        ILogger<RemoveMatriculaHandler> logger)
    {
        _matriculaRepository = matriculaRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(RemoveMatriculaCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var matricula = await _matriculaRepository.GetMatriculaAtivaAsync(
                command.AlunoId,
                command.CursoId,
                cancellationToken);

            if (matricula == null)
            {
                _logger.LogWarning("Matrícula não encontrada para remoção. AlunoId: {AlunoId}, CursoId: {CursoId}", command.AlunoId, command.CursoId);
                return Result<bool>.Failure(Error.EnrollmentNotFound);
            }

            matricula.Desativar();
            _matriculaRepository.Update(matricula);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao remover matrícula. AlunoId: {AlunoId}, CursoId: {CursoId}", command.AlunoId, command.CursoId);
            return Result<bool>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao remover matrícula. AlunoId: {AlunoId}, CursoId: {CursoId}", command.AlunoId, command.CursoId);
            return Result<bool>.Failure(Error.ServerError);
        }
    }
}