using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.API.Controllers;
using StudentsRegistrationSystem.Application.Cursos.Commands;
using StudentsRegistrationSystem.Application.Cursos.Queries;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Requests;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.API.UnitTests.Controllers;

public class CursosControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CursosController _controller;

    public CursosControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new CursosController(_mediatorMock.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task Should_ReturnOkWithPagedList_When_GetAllIsSuccessful()
    {
        // Arrange
        var query = new GetAllCursosQuery(1, 10);
        var cursos = new List<CursoResponse>
        {
            new() { Id = Guid.NewGuid(), Nome = "Matemática Básica", Descricao = "Curso de matemática para iniciantes", CreatedAt = DateTime.UtcNow }
        };
        var pagedResponse = new PagedResponseOffset<CursoResponse>(cursos, 1, 10, 1);
        var successResult = Result<PagedResponseOffset<CursoResponse>>.Success(pagedResponse);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllCursosQuery>(), CancellationToken.None))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.GetAll(query, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(pagedResponse);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllCursosQuery>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_GetAllFails()
    {
        // Arrange
        var query = new GetAllCursosQuery(1, 10);
        var failureResult = Result<PagedResponseOffset<CursoResponse>>.Failure(Error.DatabaseError);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllCursosQuery>(), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.GetAll(query, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.DatabaseError);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllCursosQuery>(), CancellationToken.None), Times.Once);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task Should_ReturnOkWithCurso_When_GetByIdFindsCurso()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var curso = new CursoResponse { Id = cursoId, Nome = "Física Avançada", Descricao = "Curso avançado de física quântica", CreatedAt = DateTime.UtcNow };
        var successResult = Result<CursoResponse>.Success(curso);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetCursoByIdQuery>(q => q.Id == cursoId), CancellationToken.None))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.GetById(cursoId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(curso);
        _mediatorMock.Verify(m => m.Send(It.Is<GetCursoByIdQuery>(q => q.Id == cursoId), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_GetByIdDoesNotFindCurso()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var failureResult = Result<CursoResponse>.Failure(Error.CourseNotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetCursoByIdQuery>(q => q.Id == cursoId), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.GetById(cursoId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().Be(Error.CourseNotFound);
        _mediatorMock.Verify(m => m.Send(It.Is<GetCursoByIdQuery>(q => q.Id == cursoId), CancellationToken.None), Times.Once);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Should_ReturnCreatedAtAction_When_CreateIsSuccessful()
    {
        // Arrange
        var request = new CursoRequest("Química Orgânica", "Introdução à química orgânica e suas aplicações");
        var createdCurso = new CursoResponse { Id = Guid.NewGuid(), Nome = request.Nome, Descricao = request.Descricao, CreatedAt = DateTime.UtcNow };
        var successResult = Result<CursoResponse>.Success(createdCurso);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateCursoCommand>(), CancellationToken.None))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(CursosController.GetById));
        createdResult.RouteValues!["id"].Should().Be(createdCurso.Id);
        createdResult.Value.Should().Be(createdCurso);
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateCursoCommand>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_CreateFails()
    {
        // Arrange
        var request = new CursoRequest("Curso Inválido", "Descrição curta");
        var failureResult = Result<CursoResponse>.Failure(Error.DatabaseError);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateCursoCommand>(), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.DatabaseError);
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateCursoCommand>(), CancellationToken.None), Times.Once);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Should_ReturnOkWithUpdatedCurso_When_UpdateIsSuccessful()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var request = new CursoRequest("Biologia Atualizada", "Curso de biologia com conteúdo atualizado sobre genética");
        var updatedCurso = new CursoResponse { Id = cursoId, Nome = request.Nome, Descricao = request.Descricao, CreatedAt = DateTime.UtcNow };
        var successResult = Result<CursoResponse>.Success(updatedCurso);

        _mediatorMock
            .Setup(m => m.Send(It.Is<UpdateCursoCommand>(c => c.Id == cursoId), CancellationToken.None))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.Update(cursoId, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(updatedCurso);
        _mediatorMock.Verify(m => m.Send(It.Is<UpdateCursoCommand>(c => c.Id == cursoId), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_UpdateFails()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var request = new CursoRequest("Curso Inexistente", "Tentativa de atualizar curso que não existe");
        var failureResult = Result<CursoResponse>.Failure(Error.CourseNotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<UpdateCursoCommand>(c => c.Id == cursoId), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.Update(cursoId, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.CourseNotFound);
        _mediatorMock.Verify(m => m.Send(It.Is<UpdateCursoCommand>(c => c.Id == cursoId), CancellationToken.None), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Should_ReturnNoContent_When_DeleteIsSuccessful()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var successResult = Result<bool>.Success(true);

        _mediatorMock
            .Setup(m => m.Send(It.Is<DeleteCursoCommand>(c => c.Id == cursoId), CancellationToken.None))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.Delete(cursoId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteCursoCommand>(c => c.Id == cursoId), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_DeleteFails()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var failureResult = Result<bool>.Failure(Error.CourseNotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<DeleteCursoCommand>(c => c.Id == cursoId), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.Delete(cursoId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.CourseNotFound);
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteCursoCommand>(c => c.Id == cursoId), CancellationToken.None), Times.Once);
    }

    #endregion
}