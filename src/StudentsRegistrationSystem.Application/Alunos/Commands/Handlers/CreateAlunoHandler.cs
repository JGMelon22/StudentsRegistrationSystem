using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Alunos.Domains.Mappings;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.Alunos.Commands.Handlers;

public class CreateAlunoHandler : IRequestHandler<CreateAlunoCommand, Result<AlunoResponse>>
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly AppDbContext _context;
    private readonly ILogger<CreateAlunoHandler> _logger;

    public CreateAlunoHandler(IAlunoRepository alunoRepository, AppDbContext context, ILogger<CreateAlunoHandler> logger)
    {
        _alunoRepository = alunoRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<AlunoResponse>> Handle(CreateAlunoCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var emailExists = await _alunoRepository.EmailExistsAsync(command.Request.Email, null, cancellationToken);
            if (emailExists)
            {
                _logger.LogWarning("Tentativa de criar aluno com e-mail já existente. Email: {Email}", command.Request.Email);
                return Result<AlunoResponse>.Failure(Error.StudentAlreadyExists);
            }

            var aluno = command.Request.ToDomain();

            if (!aluno.IsMaiorDeIdade())
            {
                _logger.LogWarning("Tentativa de criar aluno menor de idade. Email: {Email}", command.Request.Email);
                return Result<AlunoResponse>.Failure(Error.StudentUnderage);
            }

            await _alunoRepository.AddAsync(aluno, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<AlunoResponse>.Success(aluno.ToResponse());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao criar aluno. Email: {Email}", command.Request.Email);
            return Result<AlunoResponse>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar aluno. Email: {Email}", command.Request.Email);
            return Result<AlunoResponse>.Failure(Error.ServerError);
        }
    }
}