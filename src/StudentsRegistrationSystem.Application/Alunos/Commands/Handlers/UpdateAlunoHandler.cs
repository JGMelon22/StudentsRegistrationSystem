using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Alunos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Alunos.Commands.Handlers;

public class UpdateAlunoHandler : IRequestHandler<UpdateAlunoCommand, Result<AlunoResponse>>
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly AppDbContext _context;
    private readonly ILogger<UpdateAlunoHandler> _logger;

    public UpdateAlunoHandler(IAlunoRepository alunoRepository, AppDbContext context, ILogger<UpdateAlunoHandler> logger)
    {
        _alunoRepository = alunoRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<AlunoResponse>> Handle(UpdateAlunoCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var aluno = await _alunoRepository.GetByIdAsync(command.Id, cancellationToken);

            if (aluno == null)
            {
                _logger.LogWarning("Tentativa de atualizar aluno não encontrado. Id: {AlunoId}", command.Id);
                return Result<AlunoResponse>.Failure(Error.StudentNotFound);
            }

            var emailExists = await _alunoRepository.EmailExistsAsync(command.Request.Email, command.Id, cancellationToken);
            if (emailExists)
            {
                _logger.LogWarning("Tentativa de atualizar aluno com e-mail já existente. Email: {Email}", command.Request.Email);
                return Result<AlunoResponse>.Failure(Error.StudentAlreadyExists);
            }

            aluno.Atualizar(command.Request.Nome, command.Request.Email, command.Request.DataNascimento);

            if (!aluno.IsMaiorDeIdade())
            {
                _logger.LogWarning("Tentativa de atualizar aluno menor de idade. Email: {Email}", command.Request.Email);
                return Result<AlunoResponse>.Failure(Error.StudentUnderage);
            }

            _alunoRepository.Update(aluno);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<AlunoResponse>.Success(aluno.ToResponse());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao atualizar aluno. Id: {AlunoId}", command.Id);
            return Result<AlunoResponse>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar aluno. Id: {AlunoId}", command.Id);
            return Result<AlunoResponse>.Failure(Error.ServerError);
        }
    }
}