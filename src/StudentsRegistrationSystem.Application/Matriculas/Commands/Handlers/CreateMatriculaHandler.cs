using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Matriculas.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Matriculas.Domains.Mappings;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Matriculas.Commands.Handlers;

public class CreateMatriculaHandler : IRequestHandler<CreateMatriculaCommand, Result<MatriculaResponse>>
{
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IAlunoRepository _alunoRepository;
    private readonly ICursoRepository _cursoRepository;
    private readonly AppDbContext _context;
    private readonly ILogger<CreateMatriculaHandler> _logger;

    public CreateMatriculaHandler(
        IMatriculaRepository matriculaRepository,
        IAlunoRepository alunoRepository,
        ICursoRepository cursoRepository,
        AppDbContext context,
        ILogger<CreateMatriculaHandler> logger)
    {
        _matriculaRepository = matriculaRepository;
        _alunoRepository = alunoRepository;
        _cursoRepository = cursoRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<MatriculaResponse>> Handle(CreateMatriculaCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var alunoExists = await _alunoRepository.ExistsAsync(command.Request.AlunoId, cancellationToken);
            if (!alunoExists)
            {
                _logger.LogWarning("Aluno não encontrado. AlunoId: {AlunoId}", command.Request.AlunoId);
                return Result<MatriculaResponse>.Failure(Error.StudentNotFound);
            }

            var cursoExists = await _cursoRepository.ExistsAsync(command.Request.CursoId, cancellationToken);
            if (!cursoExists)
            {
                _logger.LogWarning("Curso não encontrado. CursoId: {CursoId}", command.Request.CursoId);
                return Result<MatriculaResponse>.Failure(Error.CourseNotFound);
            }

            var jaMatriculado = await _matriculaRepository.AlunoJaMatriculadoAsync(
                command.Request.AlunoId,
                command.Request.CursoId,
                cancellationToken);

            if (jaMatriculado)
            {
                _logger.LogWarning("Aluno já matriculado. AlunoId: {AlunoId}, CursoId: {CursoId}", command.Request.AlunoId, command.Request.CursoId);
                return Result<MatriculaResponse>.Failure(Error.EnrollmentAlreadyEnrolled);
            }

            var matricula = command.Request.ToDomain();

            await _matriculaRepository.AddAsync(matricula, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var matriculaCompleta = await _matriculaRepository.GetByIdAsync(matricula.Id, cancellationToken);

            if (matriculaCompleta == null)
            {
                _logger.LogError("Falha ao recarregar matrícula após criação. MatriculaId: {MatriculaId}", matricula.Id);
                return Result<MatriculaResponse>.Failure(Error.EnrollmentNotFound);
            }

            return Result<MatriculaResponse>.Success(matriculaCompleta.ToResponse());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error...");
            return Result<MatriculaResponse>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error...");
            return Result<MatriculaResponse>.Failure(Error.ServerError);
        }
    }
}