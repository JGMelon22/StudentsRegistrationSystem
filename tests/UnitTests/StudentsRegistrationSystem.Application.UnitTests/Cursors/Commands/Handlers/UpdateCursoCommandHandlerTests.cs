using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StudentsRegistrationSystem.Application.Cursos.Commands;
using StudentsRegistrationSystem.Application.Cursos.Commands.Handlers;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Requests;
using StudentsRegistrationSystem.Core.Cursos.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.UnitTests.Cursors.Commands.Handlers;

public class UpdateCursoCommandHandlerTests
{
    private readonly Mock<ICursoRepository> _cursoRepositoryMock;
    private readonly Mock<AppDbContext> _contextMock;
    private readonly Mock<ILogger<UpdateCursoCommandHandler>> _loggerMock;
    private readonly UpdateCursoCommandHandler _handler;

    public UpdateCursoCommandHandlerTests()
    {
        _cursoRepositoryMock = new Mock<ICursoRepository>();
        _contextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _loggerMock = new Mock<ILogger<UpdateCursoCommandHandler>>();

        _handler = new UpdateCursoCommandHandler(
            _cursoRepositoryMock.Object,
            _contextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_CursoIsUpdatedSuccessfully()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var request = new CursoRequest("Curso Atualizado", "Descrição atualizada do curso");
        var command = new UpdateCursoCommand(cursoId, request);
        var curso = new Curso("Curso Original", "Descrição original");

        _cursoRepositoryMock
            .Setup(x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(curso);

        _cursoRepositoryMock
            .Setup(x => x.Update(It.IsAny<Curso>()));

        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Nome.Should().Be(request.Nome);
        result.Value.Descricao.Should().Be(request.Descricao);

        curso.Nome.Should().Be(request.Nome);
        curso.Descricao.Should().Be(request.Descricao);
        curso.UpdatedAt.Should().NotBeNull();

        _cursoRepositoryMock.Verify(
            x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()),
            Times.Once);

        _cursoRepositoryMock.Verify(
            x => x.Update(It.Is<Curso>(c => c.Nome == request.Nome && c.Descricao == request.Descricao)),
            Times.Once);

        _contextMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_CursoNotFound()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var request = new CursoRequest("Curso Atualizado", "Descrição atualizada");
        var command = new UpdateCursoCommand(cursoId, request);

        _cursoRepositoryMock
            .Setup(x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Curso?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(Error.CourseNotFound);

        _cursoRepositoryMock.Verify(
            x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()),
            Times.Once);

        _cursoRepositoryMock.Verify(
            x => x.Update(It.IsAny<Curso>()),
            Times.Never);

        _contextMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Curso não encontrado")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_DbUpdateExceptionOccurs()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var request = new CursoRequest("Curso Atualizado", "Descrição atualizada");
        var command = new UpdateCursoCommand(cursoId, request);
        var curso = new Curso("Curso Original", "Descrição original");
        var dbException = new DbUpdateException("Database error");

        _cursoRepositoryMock
            .Setup(x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(curso);

        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(dbException);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(Error.DatabaseError);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Erro de banco de dados")),
                It.Is<Exception>(ex => ex == dbException),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_UnexpectedExceptionOccurs()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var request = new CursoRequest("Curso Atualizado", "Descrição atualizada");
        var command = new UpdateCursoCommand(cursoId, request);
        var unexpectedException = new Exception("Unexpected error");

        _cursoRepositoryMock
            .Setup(x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(unexpectedException);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(Error.ServerError);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Erro inesperado")),
                It.Is<Exception>(ex => ex == unexpectedException),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}