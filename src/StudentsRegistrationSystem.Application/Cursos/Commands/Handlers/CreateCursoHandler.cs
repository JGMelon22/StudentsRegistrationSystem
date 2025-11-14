using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Cursos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Cursos.Commands.Handlers;

public class CreateCursoHandler : IRequestHandler<CreateCursoCommand, Result<CursoResponse>>
{
    private readonly ICursoRepository _cursoRepository;
    private readonly AppDbContext _context;
    private readonly ILogger<CreateCursoHandler> _logger;

    public CreateCursoHandler(ICursoRepository cursoRepository, AppDbContext context, ILogger<CreateCursoHandler> logger)
    {
        _cursoRepository = cursoRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<CursoResponse>> Handle(CreateCursoCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var curso = command.Request.ToDomain();

            await _cursoRepository.AddAsync(curso, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<CursoResponse>.Success(curso.ToResponse());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao criar curso. Nome: {Nome}", command.Request.Nome);
            return Result<CursoResponse>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar curso. Nome: {Nome}", command.Request.Nome);
            return Result<CursoResponse>.Failure(Error.ServerError);
        }
    }
}