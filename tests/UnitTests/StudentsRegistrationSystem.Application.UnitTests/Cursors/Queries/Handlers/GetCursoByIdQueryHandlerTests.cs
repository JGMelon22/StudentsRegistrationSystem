using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StudentsRegistrationSystem.Application.Cursos.Queries;
using StudentsRegistrationSystem.Application.Cursos.Queries.Handlers;
using StudentsRegistrationSystem.Core.Cursos.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.UnitTests.Cursors.Queries.Handlers;

public class GetCursoByIdQueryHandlerTests
{
    private readonly Mock<ICursoRepository> _cursoRepositoryMock;
    private readonly Mock<ILogger<GetCursoByIdQueryHandler>> _loggerMock;
    private readonly GetCursoByIdQueryHandler _handler;

    public GetCursoByIdQueryHandlerTests()
    {
        _cursoRepositoryMock = new Mock<ICursoRepository>();
        _loggerMock = new Mock<ILogger<GetCursoByIdQueryHandler>>();

        _handler = new GetCursoByIdQueryHandler(
            _cursoRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Should_ReturnCursoResponse_When_CursoExists()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var query = new GetCursoByIdQuery(cursoId);
        var curso = new Curso("Curso de C#", "Descrição do curso de C#");

        _cursoRepositoryMock
            .Setup(x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(curso);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Nome.Should().Be(curso.Nome);
        result.Value.Descricao.Should().Be(curso.Descricao);

        _cursoRepositoryMock.Verify(
            x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_CursoNotFound()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var query = new GetCursoByIdQuery(cursoId);

        _cursoRepositoryMock
            .Setup(x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Curso?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(Error.CourseNotFound);

        _cursoRepositoryMock.Verify(
            x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()),
            Times.Once);

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
        var query = new GetCursoByIdQuery(cursoId);
        var dbException = new DbUpdateException("Database error");

        _cursoRepositoryMock
            .Setup(x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(dbException);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

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
        var query = new GetCursoByIdQuery(cursoId);
        var unexpectedException = new Exception("Unexpected error");

        _cursoRepositoryMock
            .Setup(x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(unexpectedException);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

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