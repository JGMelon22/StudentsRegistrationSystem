using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StudentsRegistrationSystem.Application.Matriculas.Queries;
using StudentsRegistrationSystem.Application.Matriculas.Queries.Handlers;
using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.UnitTests.Matriculas.Queries.Handlers;

public class GetAlunosByCursoQueryHandlerTests
{
    private readonly Mock<ICursoRepository> _cursoRepositoryMock;
    private readonly Mock<ILogger<GetAlunosByCursoQueryHandler>> _loggerMock;
    private readonly GetAlunosByCursoQueryHandler _handler;

    public GetAlunosByCursoQueryHandlerTests()
    {
        _cursoRepositoryMock = new Mock<ICursoRepository>();
        _loggerMock = new Mock<ILogger<GetAlunosByCursoQueryHandler>>();

        _handler = new GetAlunosByCursoQueryHandler(
            _cursoRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Should_ReturnPagedAlunosResponse_When_CursoExists()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var query = new GetAlunosByCursoQuery(cursoId, 1, 10);

        var alunos = new List<Aluno>
        {
            new("João Silva", "joao@email.com", new DateTime(1990, 1, 1)),
            new("Maria Santos", "maria@email.com", new DateTime(1995, 5, 15))
        };

        var pagedAlunos = new PagedResponseOffset<Aluno>(alunos, 1, 10, 2);

        _cursoRepositoryMock
            .Setup(x => x.ExistsAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _cursoRepositoryMock
            .Setup(x => x.GetAlunosByCursoIdAsync(cursoId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedAlunos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Data.Should().HaveCount(2);
        result.Value.PageNumber.Should().Be(1);
        result.Value.PageSize.Should().Be(10);
        result.Value.TotalRecords.Should().Be(2);

        _cursoRepositoryMock.Verify(
            x => x.ExistsAsync(cursoId, It.IsAny<CancellationToken>()),
            Times.Once);

        _cursoRepositoryMock.Verify(
            x => x.GetAlunosByCursoIdAsync(cursoId, 1, 10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_CursoNotFound()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var query = new GetAlunosByCursoQuery(cursoId, 1, 10);

        _cursoRepositoryMock
            .Setup(x => x.ExistsAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(Error.CourseNotFound);

        _cursoRepositoryMock.Verify(
            x => x.ExistsAsync(cursoId, It.IsAny<CancellationToken>()),
            Times.Once);

        _cursoRepositoryMock.Verify(
            x => x.GetAlunosByCursoIdAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
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
    public async Task Should_ReturnEmptyList_When_CursoHasNoAlunos()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var query = new GetAlunosByCursoQuery(cursoId, 1, 10);

        var emptyPagedAlunos = new PagedResponseOffset<Aluno>(new List<Aluno>(), 1, 10, 0);

        _cursoRepositoryMock
            .Setup(x => x.ExistsAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _cursoRepositoryMock
            .Setup(x => x.GetAlunosByCursoIdAsync(cursoId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyPagedAlunos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value!.Data.Should().BeEmpty();
        result.Value.TotalRecords.Should().Be(0);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_DbUpdateExceptionOccurs()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var query = new GetAlunosByCursoQuery(cursoId, 1, 10);
        var dbException = new DbUpdateException("Database error");

        _cursoRepositoryMock
            .Setup(x => x.ExistsAsync(cursoId, It.IsAny<CancellationToken>()))
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
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Database error")),
                It.Is<Exception>(ex => ex == dbException),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_UnexpectedExceptionOccurs()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var query = new GetAlunosByCursoQuery(cursoId, 1, 10);
        var unexpectedException = new Exception("Unexpected error");

        _cursoRepositoryMock
            .Setup(x => x.ExistsAsync(cursoId, It.IsAny<CancellationToken>()))
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