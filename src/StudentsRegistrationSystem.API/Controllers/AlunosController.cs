using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Application.Alunos.Commands;
using StudentsRegistrationSystem.Application.Alunos.Queries;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Requests;

namespace StudentsRegistrationSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlunosController : ControllerBase
{
    private readonly IMediator _mediator;

    public AlunosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllAlunosQuery());

        return result.Match<IActionResult>(
            Ok,
            BadRequest
        );
    }

    [HttpGet("matriculados")]
    public async Task<IActionResult> GetMatriculados(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAlunosMatriculadosQuery());

        return result.Match<IActionResult>(
            Ok,
            BadRequest
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAlunoByIdQuery(id));

        return result.Match<IActionResult>(
            Ok,
            NotFound
        );
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AlunoRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateAlunoCommand(request));

        return result.Match<IActionResult>(
            aluno => CreatedAtAction(nameof(GetById), new { id = aluno.Id }, aluno),
            BadRequest
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AlunoRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateAlunoCommand(id, request));

        return result.Match<IActionResult>(
            Ok,
            BadRequest
        );
    }

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