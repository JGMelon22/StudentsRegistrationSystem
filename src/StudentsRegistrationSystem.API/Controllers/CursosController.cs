using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Application.Cursos.Commands;
using StudentsRegistrationSystem.Application.Cursos.Queries;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Requests;

namespace StudentsRegistrationSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CursosController : ControllerBase
{
    private readonly IMediator _mediator;

    public CursosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllCursosQuery());

        return result.Match<IActionResult>(
            Ok,
            BadRequest
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCursoByIdQuery(id));

        return result.Match<IActionResult>(
           Ok,
           NotFound
       );
    }

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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CursoRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateCursoCommand(id, request));

        return result.Match<IActionResult>(
            Ok,
            BadRequest
        );
    }

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
