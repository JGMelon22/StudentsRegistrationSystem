using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Application.Matriculas.Commands;
using StudentsRegistrationSystem.Application.Matriculas.Queries;
using StudentsRegistrationSystem.Core.Matriculas.Domains.DTOs.Requests;

namespace StudentsRegistrationSystem.API.Controllers;

/// <summary>
/// Controller responsável por gerenciar as matrículas de alunos em cursos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MatriculasController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="MatriculasController"/>.
    /// </summary>
    /// <param name="mediator">Instância do mediator para comunicação com a camada de aplicação.</param>
    public MatriculasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém todos os alunos matriculados em um curso específico.
    /// </summary>
    /// <param name="cursoId">Identificador único do curso.</param>
    /// <param name="query">
    /// Parâmetros adicionais de consulta, incluindo paginação:
    /// <br/>- <c>PageNumber</c>: número da página (opcional, padrão = 1)
    /// <br/>- <c>PageSize</c>: quantidade de itens por página (opcional, padrão = 10)
    /// </param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista paginada de alunos matriculados no curso.</returns>
    /// <response code="200">Retorna a lista de alunos matriculados no curso.</response>
    /// <response code="400">Se ocorrer um erro ao buscar os alunos.</response>
    [HttpGet("curso/{cursoId:guid}/alunos")]
    public async Task<IActionResult> GetAlunosByCurso(Guid cursoId, [FromQuery] GetAlunosByCursoQuery query, CancellationToken cancellationToken)
    {
        var queryWithCursoId = query with { CursoId = cursoId };
        var result = await _mediator.Send(queryWithCursoId);

        return result.Match<IActionResult>(
            Ok,
            BadRequest
        );
    }

    /// <summary>
    /// Cria uma nova matrícula de um aluno em um curso.
    /// </summary>
    /// <param name="request">Dados da matrícula a ser criada.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>A matrícula criada.</returns>
    /// <response code="201">Retorna a matrícula criada com sucesso.</response>
    /// <response code="400">Se os dados fornecidos forem inválidos.</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MatriculaRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateMatriculaCommand(request));

        return result.Match<IActionResult>(
            matricula => Created($"{matricula.Id}", matricula),
            BadRequest
        );
    }

    /// <summary>
    /// Remove a matrícula de um aluno em um curso.
    /// </summary>
    /// <param name="request">Dados da matrícula a ser removida.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Resposta sem conteúdo se a remoção for bem-sucedida.</returns>
    /// <response code="204">Matrícula removida com sucesso.</response>
    /// <response code="400">Se ocorrer um erro ao remover a matrícula.</response>
    [HttpDelete]
    public async Task<IActionResult> Remove([FromBody] MatriculaRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RemoveMatriculaCommand(request.AlunoId, request.CursoId));

        return result.Match<IActionResult>(
            _ => NoContent(),
            BadRequest
        );
    }
}