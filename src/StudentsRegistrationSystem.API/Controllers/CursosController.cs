using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Application.Cursos.Commands;
using StudentsRegistrationSystem.Application.Cursos.Queries;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Requests;

namespace StudentsRegistrationSystem.API.Controllers;

/// <summary>
/// Controller responsável por gerenciar os cursos do sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CursosController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="CursosController"/>.
    /// </summary>
    /// <param name="mediator">Instância do mediator para comunicação com a camada de aplicação.</param>
    public CursosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém todos os cursos cadastrados.
    /// </summary>
    /// <param name="query">
    /// Parâmetros adicionais de consulta, incluindo paginação:
    /// <br/>- <c>PageNumber</c>: número da página (opcional, padrão = 1)
    /// <br/>- <c>PageSize</c>: quantidade de itens por página (opcional, padrão = 10)
    /// </param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista paginada de cursos.</returns>
    /// <response code="200">Retorna a lista de cursos.</response>
    /// <response code="400">Se ocorrer um erro ao buscar os cursos.</response>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllCursosQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query);

        return result.Match<IActionResult>(
            Ok,
            BadRequest
        );
    }

    /// <summary>
    /// Obtém um curso específico pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador único do curso.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O curso solicitado.</returns>
    /// <response code="200">Retorna o curso encontrado.</response>
    /// <response code="404">Se o curso não for encontrado.</response>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCursoByIdQuery(id));

        return result.Match<IActionResult>(
           Ok,
           NotFound
       );
    }

    /// <summary>
    /// Cria um novo curso.
    /// </summary>
    /// <param name="request">Dados do curso a ser criado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O curso criado.</returns>
    /// <response code="201">Retorna o curso criado com sucesso.</response>
    /// <response code="400">Se os dados fornecidos forem inválidos.</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CursoRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _mediator.Send(new CreateCursoCommand(request));

        return result.Match<IActionResult>(
            curso => CreatedAtAction(nameof(GetById), new { id = curso.Id }, curso),
            BadRequest
        );
    }

    /// <summary>
    /// Atualiza os dados de um curso existente.
    /// </summary>
    /// <param name="id">Identificador único do curso a ser atualizado.</param>
    /// <param name="request">Novos dados do curso.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O curso atualizado.</returns>
    /// <response code="200">Retorna o curso atualizado com sucesso.</response>
    /// <response code="400">Se os dados fornecidos forem inválidos.</response>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CursoRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateCursoCommand(id, request));

        return result.Match<IActionResult>(
            Ok,
            BadRequest
        );
    }

    /// <summary>
    /// Remove um curso do sistema.
    /// </summary>
    /// <param name="id">Identificador único do curso a ser removido.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Resposta sem conteúdo se a remoção for bem-sucedida.</returns>
    /// <response code="204">Curso removido com sucesso.</response>
    /// <response code="400">Se ocorrer um erro ao remover o curso.</response>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCursoCommand(id));

        return result.Match<IActionResult>(
            _ => NoContent(),
            BadRequest
        );
    }
}