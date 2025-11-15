using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StudentsRegistrationSystem.Application.Alunos.Commands;
using StudentsRegistrationSystem.Application.Alunos.Commands.Handlers;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Requests;
using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.UnitTests.Alunos.Commands.Handlers;

public class UpdateAlunoCommandHandlerTests
{
    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly Mock<AppDbContext> _contextMock;
    private readonly Mock<ILogger<UpdateAlunoCommandHandler>> _loggerMock;
    private readonly UpdateAlunoCommandHandler _handler;

    public UpdateAlunoCommandHandlerTests()
    {
        _alunoRepositoryMock = new Mock<IAlunoRepository>();
        _contextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _loggerMock = new Mock<ILogger<UpdateAlunoCommandHandler>>();

        _handler = new UpdateAlunoCommandHandler(
            _alunoRepositoryMock.Object,
            _contextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_AlunoIsUpdatedSuccessfully()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var request = new AlunoRequest("João Silva Atualizado", "joao.novo@email.com", new DateTime(1990, 1, 1));
        var command = new UpdateAlunoCommand(alunoId, request);
        var aluno = new Aluno("João Silva", "joao@email.com", new DateTime(1990, 1, 1));

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aluno);

        _alunoRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email, alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _alunoRepositoryMock
            .Setup(x => x.Update(It.IsAny<Aluno>()));

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
        result.Value.Email.Should().Be(request.Email);

        aluno.Nome.Should().Be(request.Nome);
        aluno.Email.Should().Be(request.Email);
        aluno.UpdatedAt.Should().NotBeNull();

        _alunoRepositoryMock.Verify(
            x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()),
            Times.Once);

        _alunoRepositoryMock.Verify(
            x => x.EmailExistsAsync(request.Email, alunoId, It.IsAny<CancellationToken>()),
            Times.Once);

        _alunoRepositoryMock.Verify(
            x => x.Update(It.Is<Aluno>(a => a.Nome == request.Nome && a.Email == request.Email)),
            Times.Once);

        _contextMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_AlunoNotFound()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var request = new AlunoRequest("João Silva", "joao@email.com", new DateTime(1990, 1, 1));
        var command = new UpdateAlunoCommand(alunoId, request);

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Aluno?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(Error.StudentNotFound);

        _alunoRepositoryMock.Verify(
            x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()),
            Times.Once);

        _alunoRepositoryMock.Verify(
            x => x.Update(It.IsAny<Aluno>()),
            Times.Never);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Tentativa de atualizar aluno não encontrado")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_EmailAlreadyExists()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var request = new AlunoRequest("João Silva", "email.existente@email.com", new DateTime(1990, 1, 1));
        var command = new UpdateAlunoCommand(alunoId, request);
        var aluno = new Aluno("João Silva", "joao@email.com", new DateTime(1990, 1, 1));

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aluno);

        _alunoRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email, alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(Error.StudentAlreadyExists);

        _alunoRepositoryMock.Verify(
            x => x.Update(It.IsAny<Aluno>()),
            Times.Never);

        _contextMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("e-mail já existente")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_AlunoIsUnderage()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var request = new AlunoRequest("João Silva", "joao@email.com", DateTime.Today.AddYears(-17));
        var command = new UpdateAlunoCommand(alunoId, request);
        var aluno = new Aluno("João Silva", "joao@email.com", new DateTime(1990, 1, 1));

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aluno);

        _alunoRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email, alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(Error.StudentUnderage);

        _alunoRepositoryMock.Verify(
            x => x.Update(It.IsAny<Aluno>()),
            Times.Never);

        _contextMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("menor de idade")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_DbUpdateExceptionOccurs()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var request = new AlunoRequest("João Silva", "joao@email.com", new DateTime(1990, 1, 1));
        var command = new UpdateAlunoCommand(alunoId, request);
        var aluno = new Aluno("João Silva", "joao@email.com", new DateTime(1990, 1, 1));
        var dbException = new DbUpdateException("Database error");

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aluno);

        _alunoRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email, alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

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
        var alunoId = Guid.NewGuid();
        var request = new AlunoRequest("João Silva", "joao@email.com", new DateTime(1990, 1, 1));
        var command = new UpdateAlunoCommand(alunoId, request);
        var unexpectedException = new Exception("Unexpected error");

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
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