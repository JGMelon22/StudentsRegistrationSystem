using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.API.Controllers;
using StudentsRegistrationSystem.Application.Alunos.Commands;
using StudentsRegistrationSystem.Application.Alunos.Queries;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Requests;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Shared;

namespace StudentsRegistrationSystem.API.UnitTests.Controllers;

public class AlunosControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AlunosController _controller;

    public AlunosControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AlunosController(_mediatorMock.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task Should_ReturnOkWithPagedList_When_GetAllIsSuccessful()
    {
        // Arrange
        var query = new GetAllAlunosQuery(1, 10);
        var alunos = new List<AlunoResponse>
        {
            new() { Id = Guid.NewGuid(), Nome = "João Silva", Email = "joao@email.com", DataNascimento = new DateTime(2000, 1, 1), CreatedAt = DateTime.UtcNow }
        };
        var pagedResponse = new PagedResponseOffset<AlunoResponse>(alunos, 1, 10, 1);
        var successResult = Result<PagedResponseOffset<AlunoResponse>>.Success(pagedResponse);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllAlunosQuery>(), CancellationToken.None))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.GetAll(query, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(pagedResponse);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllAlunosQuery>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_GetAllFails()
    {
        // Arrange
        var query = new GetAllAlunosQuery(1, 10);
        var failureResult = Result<PagedResponseOffset<AlunoResponse>>.Failure(Error.DatabaseError);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllAlunosQuery>(), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.GetAll(query, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.DatabaseError);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllAlunosQuery>(), CancellationToken.None), Times.Once);
    }

    #endregion

    #region GetMatriculados Tests

    [Fact]
    public async Task Should_ReturnOkWithPagedList_When_GetMatriculadosIsSuccessful()
    {
        // Arrange
        var query = new GetAlunosMatriculadosQuery(1, 10);
        var alunos = new List<AlunoResponse>
        {
            new() { Id = Guid.NewGuid(), Nome = "Maria Santos", Email = "maria@email.com", DataNascimento = new DateTime(1999, 5, 15), CreatedAt = DateTime.UtcNow }
        };
        var pagedResponse = new PagedResponseOffset<AlunoResponse>(alunos, 1, 10, 1);
        var successResult = Result<PagedResponseOffset<AlunoResponse>>.Success(pagedResponse);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAlunosMatriculadosQuery>(), CancellationToken.None))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.GetMatriculados(query, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(pagedResponse);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAlunosMatriculadosQuery>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_GetMatriculadosFails()
    {
        // Arrange
        var query = new GetAlunosMatriculadosQuery(1, 10);
        var failureResult = Result<PagedResponseOffset<AlunoResponse>>.Failure(Error.DatabaseError);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAlunosMatriculadosQuery>(), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.GetMatriculados(query, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.DatabaseError);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAlunosMatriculadosQuery>(), CancellationToken.None), Times.Once);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task Should_ReturnOkWithAluno_When_GetByIdFindsAluno()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var aluno = new AlunoResponse { Id = alunoId, Nome = "Pedro Costa", Email = "pedro@email.com", DataNascimento = new DateTime(2001, 3, 20), CreatedAt = DateTime.UtcNow };
        var successResult = Result<AlunoResponse>.Success(aluno);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetAlunoByIdQuery>(q => q.Id == alunoId), CancellationToken.None))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.GetById(alunoId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(aluno);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAlunoByIdQuery>(q => q.Id == alunoId), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_GetByIdDoesNotFindAluno()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var failureResult = Result<AlunoResponse>.Failure(Error.StudentNotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetAlunoByIdQuery>(q => q.Id == alunoId), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.GetById(alunoId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().Be(Error.StudentNotFound);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAlunoByIdQuery>(q => q.Id == alunoId), CancellationToken.None), Times.Once);

    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Should_ReturnCreatedAtAction_When_CreateIsSuccessful()
    {
        // Arrange
        var request = new AlunoRequest("Ana Lima", "ana@email.com", new DateTime(2002, 7, 10));
        var createdAluno = new AlunoResponse { Id = Guid.NewGuid(), Nome = request.Nome, Email = request.Email, DataNascimento = request.DataNascimento, CreatedAt = DateTime.UtcNow };
        var successResult = Result<AlunoResponse>.Success(createdAluno);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateAlunoCommand>(), CancellationToken.None))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(AlunosController.GetById));
        createdResult.RouteValues!["id"].Should().Be(createdAluno.Id);
        createdResult.Value.Should().Be(createdAluno);
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateAlunoCommand>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_CreateFails()
    {
        // Arrange
        var request = new AlunoRequest("Invalid", "invalid", DateTime.MinValue);
        var failureResult = Result<AlunoResponse>.Failure(Error.StudentInvalidEmail);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateAlunoCommand>(), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.StudentInvalidEmail);
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateAlunoCommand>(), CancellationToken.None), Times.Once);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Should_ReturnOkWithUpdatedAluno_When_UpdateIsSuccessful()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var request = new AlunoRequest("Carlos Updated", "carlos.updated@email.com", new DateTime(2000, 12, 25));
        var updatedAluno = new AlunoResponse { Id = alunoId, Nome = request.Nome, Email = request.Email, DataNascimento = request.DataNascimento, CreatedAt = DateTime.UtcNow };
        var successResult = Result<AlunoResponse>.Success(updatedAluno);

        _mediatorMock
            .Setup(m => m.Send(It.Is<UpdateAlunoCommand>(c => c.Id == alunoId), CancellationToken.None))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.Update(alunoId, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(updatedAluno);
        _mediatorMock.Verify(m => m.Send(It.Is<UpdateAlunoCommand>(c => c.Id == alunoId), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_UpdateFails()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var request = new AlunoRequest("Invalid", "invalid", DateTime.MinValue);
        var failureResult = Result<AlunoResponse>.Failure(Error.StudentNotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<UpdateAlunoCommand>(c => c.Id == alunoId), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.Update(alunoId, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.StudentNotFound);
        _mediatorMock.Verify(m => m.Send(It.Is<UpdateAlunoCommand>(c => c.Id == alunoId), CancellationToken.None), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Should_ReturnNoContent_When_DeleteIsSuccessful()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var successResult = Result<bool>.Success(true);

        _mediatorMock
            .Setup(m => m.Send(It.Is<DeleteAlunoCommand>(c => c.Id == alunoId), CancellationToken.None))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.Delete(alunoId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteAlunoCommand>(c => c.Id == alunoId), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_DeleteFails()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var failureResult = Result<bool>.Failure(Error.StudentNotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<DeleteAlunoCommand>(c => c.Id == alunoId), CancellationToken.None))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.Delete(alunoId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(Error.StudentNotFound);
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteAlunoCommand>(c => c.Id == alunoId), CancellationToken.None), Times.Once);
    }

    #endregion
}