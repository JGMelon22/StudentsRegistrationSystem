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
    private readonly IMatriculaRepository _matriculaRepository;

    public DeleteAlunoCommandHandler(
        IAlunoRepository alunoRepository,
        IMatriculaRepository matriculaRepository,
        AppDbContext context,
        ILogger<DeleteAlunoCommandHandler> logger)
    {
        _alunoRepository = alunoRepository;
        _matriculaRepository = matriculaRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteAlunoCommand command, CancellationToken cancellationToken)
    {
        var aluno = await _alunoRepository.GetByIdAsync(command.Id, cancellationToken);

        if (aluno == null)
        {
            _logger.LogWarning("Tentativa de deletar aluno não encontrado. Id: {AlunoId}", command.Id);
            return Result<bool>.Failure(Error.StudentNotFound);
        }

        var existeMatriculaAtiva = await _matriculaRepository.ExisteMatriculaAtivaPorAlunoId(aluno.Id);

        if (existeMatriculaAtiva)
        {
            _logger.LogWarning("Tentativa de deletar aluno com amtricula ativa. Id: {AlunoId}", command.Id);
            return Result<bool>.Failure(Error.ActiveRegistration);
        }

        var matriculasDesativadas = await _matriculaRepository.ObterMatriculasDesativadasPorAlunoId(aluno.Id);

        _context.Matriculas.RemoveRange(matriculasDesativadas);

        _alunoRepository.Delete(aluno);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}