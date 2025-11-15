using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Alunos.Commands.Handlers;

public class DeleteAlunoCommandHandler : IRequestHandler<DeleteAlunoCommand, Result<bool>>
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly AppDbContext _context;
    private readonly ILogger<DeleteAlunoCommandHandler> _logger;

    public DeleteAlunoCommandHandler(
        IAlunoRepository alunoRepository,
        AppDbContext context,
        ILogger<DeleteAlunoCommandHandler> logger)
    {
        _alunoRepository = alunoRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteAlunoCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var aluno = await _alunoRepository.GetByIdAsync(command.Id, cancellationToken);

            if (aluno == null)
            {
                _logger.LogWarning("Tentativa de deletar aluno não encontrado. Id: {AlunoId}", command.Id);
                return Result<bool>.Failure(Error.StudentNotFound);
            }

            _alunoRepository.Delete(aluno);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao deletar aluno. Id: {AlunoId}", command.Id);
            return Result<bool>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao deletar aluno. Id: {AlunoId}", command.Id);
            return Result<bool>.Failure(Error.ServerError);
        }
    }
}