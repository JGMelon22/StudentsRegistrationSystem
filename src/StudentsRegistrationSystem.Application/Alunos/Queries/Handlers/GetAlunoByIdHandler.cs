using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;
using StudentsRegistrationSystem.Core.Alunos.Domains.Mappings;

namespace StudentsRegistrationSystem.Application.Alunos.Queries.Handlers;

public class GetAlunoByIdHandler : IRequestHandler<GetAlunoByIdQuery, Result<AlunoResponse>>
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly ILogger<GetAlunoByIdHandler> _logger;

    public GetAlunoByIdHandler(IAlunoRepository alunoRepository, ILogger<GetAlunoByIdHandler> logger)
    {
        _alunoRepository = alunoRepository;
        _logger = logger;
    }

    public async Task<Result<AlunoResponse>> Handle(GetAlunoByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var aluno = await _alunoRepository.GetByIdAsync(query.Id, cancellationToken);

            if (aluno == null)
            {
                _logger.LogWarning("Tentativa de consultar aluno não encontrado. Id: {AlunoId}", query.Id);
                return Result<AlunoResponse>.Failure(Error.StudentNotFound);
            }

            return Result<AlunoResponse>.Success(aluno.ToResponse());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro de banco de dados ao consultar aluno. Id: {AlunoId}", query.Id);
            return Result<AlunoResponse>.Failure(Error.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao consultar aluno. Id: {AlunoId}", query.Id);
            return Result<AlunoResponse>.Failure(Error.ServerError);
        }
    }
}