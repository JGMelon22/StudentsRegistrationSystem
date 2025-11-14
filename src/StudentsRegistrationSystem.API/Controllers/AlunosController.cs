using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Application.Alunos.Commands;
using StudentsRegistrationSystem.Application.Alunos.Queries;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Requests;

namespace StudentsRegistrationSystem.API.Controllers;

/// <summary>
/// Controller responsável por gerenciar os alunos do sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AlunosController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="AlunosController"/>.
    /// </summary>
    /// <param name="mediator">Instância do mediator para comunicação com a camada de aplicação.</param>
    public AlunosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém todos os alunos cadastrados.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de todos os alunos.</returns>
    /// <response code="200">Retorna a lista de alunos.</response>
    /// <response code="400">Se ocorrer um erro ao buscar os alunos.</response>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllAlunosQuery());

        return result.Match<IActionResult>(
            Ok,
            BadRequest
        );
    }

    /// <summary>
    /// Obtém todos os alunos que possuem ao menos uma matrícula ativa.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de alunos matriculados.</returns>
    /// <response code="200">Retorna a lista de alunos matriculados.</response>
    /// <response code="400">Se ocorrer um erro ao buscar os alunos.</response>
    [HttpGet("matriculados")]
    public async Task<IActionResult> GetMatriculados(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAlunosMatriculadosQuery());

        return result.Match<IActionResult>(
            Ok,
            BadRequest
        );
    }

    /// <summary>
    /// Obtém um aluno específico pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador único do aluno.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O aluno solicitado.</returns>
    /// <response code="200">Retorna o aluno encontrado.</response>
    /// <response code="404">Se o aluno não for encontrado.</response>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAlunoByIdQuery(id));

        return result.Match<IActionResult>(
            Ok,
            NotFound
        );
    }

    /// <summary>
    /// Cria um novo aluno.
    /// </summary>
    /// <param name="request">Dados do aluno a ser criado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O aluno criado.</returns>
    /// <response code="201">Retorna o aluno criado com sucesso.</response>
    /// <response code="400">Se os dados fornecidos forem inválidos.</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AlunoRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateAlunoCommand(request));

        return result.Match<IActionResult>(
            aluno => CreatedAtAction(nameof(GetById), new { id = aluno.Id }, aluno),
            BadRequest
        );
    }

    /// <summary>
    /// Atualiza os dados de um aluno existente.
    /// </summary>
    /// <param name="id">Identificador único do aluno a ser atualizado.</param>
    /// <param name="request">Novos dados do aluno.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O aluno atualizado.</returns>
    /// <response code="200">Retorna o aluno atualizado com sucesso.</response>
    /// <response code="400">Se os dados fornecidos forem inválidos.</response>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AlunoRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateAlunoCommand(id, request));

        return result.Match<IActionResult>(
            Ok,
            BadRequest
        );
    }

    /// <summary>
    /// Remove um aluno do sistema.
    /// </summary>
    /// <param name="id">Identificador único do aluno a ser removido.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Resposta sem conteúdo se a remoção for bem-sucedida.</returns>
    /// <response code="204">Aluno removido com sucesso.</response>
    /// <response code="400">Se ocorrer um erro ao remover o aluno.</response>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteAlunoCommand(id));

        return result.Match<IActionResult>(
            _ => NoContent(),
            BadRequest
        );
    }
}