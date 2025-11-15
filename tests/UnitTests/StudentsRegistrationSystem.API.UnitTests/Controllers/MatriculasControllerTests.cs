using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.API.Controllers;
using StudentsRegistrationSystem.Application.Matriculas.Commands;
using StudentsRegistrationSystem.Application.Matriculas.Queries;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Matriculas.Domains.DTOs.Requests;
using StudentsRegistrationSystem.Core.Matriculas.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.API.UnitTests.Controllers;

public class MatriculasControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly MatriculasController _controller;

    public MatriculasControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new MatriculasController(_mediatorMock.Object);
    }

    #region GetAlunosByCurso Tests

    [Fact]
    public async Task Should_ReturnOkWithPagedList_When_GetAlunosByCursoIsSuccessful()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var query = new GetAlunosByCursoQuery(cursoId, 1, 10);
        var alunos = new List<AlunoResponse>
        {
            new() { Id = Guid.NewGuid(), Nome = "João Silva", Email = "joao@email.com", DataNascimento = new DateTime(2000, 1, 1), CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Nome = "Maria Santos", Email = "maria@email.com", DataNascimento = new DateTime(1999, 5, 15), CreatedAt = DateTime.UtcNow }
        };
        var pagedResponse = new PagedResponseOffset<AlunoResponse>(alunos, 1, 10, 2);
        var successResult = Result<PagedResponseOffset<AlunoResponse>>.Success(pagedResponse);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetAlunosByCursoQuery>(q => q.CursoId == cursoId), CancellationToken.None))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.GetAlunosByCurso(cursoId, query, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(pagedResponse);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAlunosByCursoQuery>(q => q.CursoId == cursoId), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_GetAlunosByCursoFailsWithCourseNotFound()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var query = new GetAlunosByCursoQuery(cursoId, 1, 10);
        var failureResult = Result<PagedResponseOffset<AlunoResponse>>.Failure(Error.CourseNotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetAlunosByCursoQuery>(q => q.CursoId == cursoId), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.GetAlunosByCurso(cursoId, query, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.CourseNotFound);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAlunosByCursoQuery>(q => q.CursoId == cursoId), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_GetAlunosByCursoFailsWithDatabaseError()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var query = new GetAlunosByCursoQuery(cursoId, 1, 10);
        var failureResult = Result<PagedResponseOffset<AlunoResponse>>.Failure(Error.DatabaseError);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetAlunosByCursoQuery>(q => q.CursoId == cursoId), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.GetAlunosByCurso(cursoId, query, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.DatabaseError);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAlunosByCursoQuery>(q => q.CursoId == cursoId), CancellationToken.None), Times.Once);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Should_ReturnCreated_When_CreateMatriculaIsSuccessful()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var request = new MatriculaRequest(alunoId, cursoId);
        var matriculaResponse = new MatriculaResponse
        {
            Id = Guid.NewGuid(),
            AlunoId = alunoId,
            AlunoNome = "João Silva",
            CursoId = cursoId,
            CursoNome = "Matemática Básica",
            DataMatricula = DateTime.UtcNow,
            Ativa = true
        };
        var successResult = Result<MatriculaResponse>.Success(matriculaResponse);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateMatriculaCommand>(), CancellationToken.None))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        var createdResult = result as CreatedResult;
        createdResult!.Location.Should().Be($"{matriculaResponse.Id}");
        createdResult.Value.Should().Be(matriculaResponse);
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateMatriculaCommand>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_CreateMatriculaFailsWithStudentNotFound()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var request = new MatriculaRequest(alunoId, cursoId);
        var failureResult = Result<MatriculaResponse>.Failure(Error.StudentNotFound);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateMatriculaCommand>(), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.StudentNotFound);
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateMatriculaCommand>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_CreateMatriculaFailsWithCourseNotFound()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var request = new MatriculaRequest(alunoId, cursoId);
        var failureResult = Result<MatriculaResponse>.Failure(Error.CourseNotFound);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateMatriculaCommand>(), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.CourseNotFound);
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateMatriculaCommand>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_CreateMatriculaFailsWithAlreadyEnrolled()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var request = new MatriculaRequest(alunoId, cursoId);
        var failureResult = Result<MatriculaResponse>.Failure(Error.EnrollmentAlreadyEnrolled);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateMatriculaCommand>(), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.EnrollmentAlreadyEnrolled);
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateMatriculaCommand>(), CancellationToken.None), Times.Once);
    }

    #endregion

    #region Remove Tests

    [Fact]
    public async Task Should_ReturnNoContent_When_RemoveMatriculaIsSuccessful()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var request = new MatriculaRequest(alunoId, cursoId);
        var successResult = Result<bool>.Success(true);

        _mediatorMock
            .Setup(m => m.Send(It.Is<RemoveMatriculaCommand>(c => c.AlunoId == alunoId && c.CursoId == cursoId), CancellationToken.None))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.Remove(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<RemoveMatriculaCommand>(c => c.AlunoId == alunoId && c.CursoId == cursoId), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_RemoveMatriculaFailsWithEnrollmentNotFound()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var request = new MatriculaRequest(alunoId, cursoId);
        var failureResult = Result<bool>.Failure(Error.EnrollmentNotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<RemoveMatriculaCommand>(c => c.AlunoId == alunoId && c.CursoId == cursoId), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.Remove(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.EnrollmentNotFound);
        _mediatorMock.Verify(m => m.Send(It.Is<RemoveMatriculaCommand>(c => c.AlunoId == alunoId && c.CursoId == cursoId), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_RemoveMatriculaFailsWithDatabaseError()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var request = new MatriculaRequest(alunoId, cursoId);
        var failureResult = Result<bool>.Failure(Error.DatabaseError);

        _mediatorMock
            .Setup(m => m.Send(It.Is<RemoveMatriculaCommand>(c => c.AlunoId == alunoId && c.CursoId == cursoId), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.Remove(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.DatabaseError);
        _mediatorMock.Verify(m => m.Send(It.Is<RemoveMatriculaCommand>(c => c.AlunoId == alunoId && c.CursoId == cursoId), CancellationToken.None), Times.Once);
    }

    #endregion
}