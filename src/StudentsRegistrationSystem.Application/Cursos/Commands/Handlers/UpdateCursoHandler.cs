using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Cursos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Cursos.Commands.Handlers;

public class UpdateCursoHandler : IRequestHandler<UpdateCursoCommand, Result<CursoResponse>>
{
    private readonly ICursoRepository _cursoRepository;
    private readonly AppDbContext _context;
    private readonly ILogger<UpdateCursoHandler> _logger;

    public UpdateCursoHandler(ICursoRepository cursoRepository, AppDbContext context, ILogger<UpdateCursoHandler> logger)
    {
        _cursoRepository = cursoRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<CursoResponse>> Handle(UpdateCursoCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var curso = await _cursoRepository.GetByIdAsync(command.Id, cancellationToken);

            if (curso == null)
            {
                _logger.LogWarning("Curso não encontrado para atualização. CursoId: {CursoId}", command.Id);
                return Result<CursoResponse>.Failure(Error.CourseNotFound);
            }

            curso.Atualizar(command.Request.Nome, command.Request.Descricao);
            _cursoRepository.Update(curso);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<CursoResponse>.Success(curso.ToResponse());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao atualizar curso. CursoId: {CursoId}", command.Id);
            return Result<CursoResponse>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar curso. CursoId: {CursoId}", command.Id);
            return Result<CursoResponse>.Failure(Error.ServerError);
        }
    }
}