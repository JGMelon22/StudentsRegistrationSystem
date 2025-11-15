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

public class GetAlunoByIdQueryHandlerTests
{
    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly Mock<ILogger<GetAlunoByIdQueryHandler>> _loggerMock;
    private readonly GetAlunoByIdQueryHandler _handler;

    public GetAlunoByIdQueryHandlerTests()
    {
        _alunoRepositoryMock = new Mock<IAlunoRepository>();
        _loggerMock = new Mock<ILogger<GetAlunoByIdQueryHandler>>();

        _handler = new GetAlunoByIdQueryHandler(
            _alunoRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Should_ReturnAlunoResponse_When_AlunoExists()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var query = new GetAlunoByIdQuery(alunoId);
        var aluno = new Aluno("João Silva", "joao@email.com", new DateTime(1990, 1, 1));

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aluno);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Nome.Should().Be(aluno.Nome);
        result.Value.Email.Should().Be(aluno.Email);
        result.Value.DataNascimento.Should().Be(aluno.DataNascimento);

        _alunoRepositoryMock.Verify(
            x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_AlunoNotFound()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var query = new GetAlunoByIdQuery(alunoId);

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Aluno?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(Error.StudentNotFound);

        _alunoRepositoryMock.Verify(
            x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("aluno não encontrado")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_DbUpdateExceptionOccurs()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var query = new GetAlunoByIdQuery(alunoId);
        var dbException = new DbUpdateException("Database error");

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
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
        var alunoId = Guid.NewGuid();
        var query = new GetAlunoByIdQuery(alunoId);
        var unexpectedException = new Exception("Unexpected error");

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
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