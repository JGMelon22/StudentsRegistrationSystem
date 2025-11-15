using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StudentsRegistrationSystem.Application.Alunos.Queries;
using StudentsRegistrationSystem.Application.Alunos.Queries.Handlers;
using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.UnitTests.Alunos.Queries.Handlers;

public class GetAllAlunosQueryHandlerTests
{
    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly Mock<ILogger<GetAllAlunosQueryHandler>> _loggerMock;
    private readonly GetAllAlunosQueryHandler _handler;

    public GetAllAlunosQueryHandlerTests()
    {
        _alunoRepositoryMock = new Mock<IAlunoRepository>();
        _loggerMock = new Mock<ILogger<GetAllAlunosQueryHandler>>();

        _handler = new GetAllAlunosQueryHandler(
            _alunoRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Should_ReturnPagedAlunosResponse_When_AlunosExist()
    {
        // Arrange
        var query = new GetAllAlunosQuery(1, 10);

        var alunos = new List<Aluno>
        {
            new("João Silva", "joao@email.com", new DateTime(1990, 1, 1)),
            new("Maria Santos", "maria@email.com", new DateTime(1995, 5, 15)),
            new("Pedro Oliveira", "pedro@email.com", new DateTime(1988, 10, 20))
        };

        var pagedAlunos = new PagedResponseOffset<Aluno>(alunos, 1, 10, 3);

        _alunoRepositoryMock
            .Setup(x => x.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedAlunos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Data.Should().HaveCount(3);
        result.Value.PageNumber.Should().Be(1);
        result.Value.PageSize.Should().Be(10);
        result.Value.TotalRecords.Should().Be(3);

        _alunoRepositoryMock.Verify(
            x => x.GetAllAsync(1, 10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnEmptyList_When_NoAlunosExist()
    {
        // Arrange
        var query = new GetAllAlunosQuery(1, 10);

        var emptyPagedAlunos = new PagedResponseOffset<Aluno>(new List<Aluno>(), 1, 10, 0);

        _alunoRepositoryMock
            .Setup(x => x.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
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
    public async Task Should_RespectPaginationParameters_When_Querying()
    {
        // Arrange
        var query = new GetAllAlunosQuery(3, 5);

        var alunos = new List<Aluno>
        {
            new("Aluno 11", "aluno11@email.com", new DateTime(1992, 3, 10)),
            new("Aluno 12", "aluno12@email.com", new DateTime(1993, 7, 22))
        };

        var pagedAlunos = new PagedResponseOffset<Aluno>(alunos, 3, 5, 12);

        _alunoRepositoryMock
            .Setup(x => x.GetAllAsync(3, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedAlunos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value!.PageNumber.Should().Be(3);
        result.Value.PageSize.Should().Be(5);
        result.Value.TotalPages.Should().Be(3);

        _alunoRepositoryMock.Verify(
            x => x.GetAllAsync(3, 5, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_DbUpdateExceptionOccurs()
    {
        // Arrange
        var query = new GetAllAlunosQuery(1, 10);
        var dbException = new DbUpdateException("Database error");

        _alunoRepositoryMock
            .Setup(x => x.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
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
        var query = new GetAllAlunosQuery(1, 10);
        var unexpectedException = new Exception("Unexpected error");

        _alunoRepositoryMock
            .Setup(x => x.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
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