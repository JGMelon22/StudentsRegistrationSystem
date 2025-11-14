using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Application.Matriculas.Commands;
using StudentsRegistrationSystem.Application.Matriculas.Queries;
using StudentsRegistrationSystem.Core.Matriculas.Domains.DTOs.Requests;

namespace StudentsRegistrationSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatriculasController : ControllerBase
{
    private readonly IMediator _mediator;

    public MatriculasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("curso/{cursoId:guid}/alunos")]
    public async Task<IActionResult> GetAlunosByCurso(Guid cursoId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAlunosByCursoQuery(cursoId));

        return result.Match<IActionResult>(
            Ok,
            BadRequest
        );
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MatriculaRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateMatriculaCommand(request));

        return result.Match<IActionResult>(
            matricula => Created($"/api/matriculas/{matricula.Id}", matricula),
            BadRequest
        );
    }

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