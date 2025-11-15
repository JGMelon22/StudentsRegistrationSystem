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

public class CreateAlunoHandlerTests
{
    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly Mock<AppDbContext> _contextMock;
    private readonly Mock<ILogger<CreateAlunoCommandHandler>> _loggerMock;
    private readonly CreateAlunoCommandHandler _handler;

    public CreateAlunoHandlerTests()
    {
        _alunoRepositoryMock = new Mock<IAlunoRepository>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .Options;
        _contextMock = new Mock<AppDbContext>(options);

        _loggerMock = new Mock<ILogger<CreateAlunoCommandHandler>>();
        _handler = new CreateAlunoCommandHandler(
            _alunoRepositoryMock.Object,
            _contextMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Should_CreateAluno_When_ValidRequestProvided()
    {
        // Arrange
        var request = new AlunoRequest("João Silva", "joao.silva@email.com", new DateTime(2000, 1, 1));
        var command = new CreateAlunoCommand(request);

        _alunoRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _alunoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Aluno>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(request.Nome, result.Value.Nome);
        Assert.Equal(request.Email, result.Value.Email);
        _alunoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Aluno>(), It.IsAny<CancellationToken>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_EmailAlreadyExists()
    {
        // Arrange
        var request = new AlunoRequest("João Silva", "joao.silva@email.com", new DateTime(2000, 1, 1));
        var command = new CreateAlunoCommand(request);

        _alunoRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.StudentAlreadyExists, result.Error);
        _alunoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Aluno>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_StudentIsUnderage()
    {
        // Arrange
        var request = new AlunoRequest("João Silva", "joao.silva@email.com", DateTime.Now.AddYears(-17));
        var command = new CreateAlunoCommand(request);

        _alunoRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.StudentUnderage, result.Error);
        _alunoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Aluno>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_ReturnDatabaseError_When_DbUpdateExceptionOccurs()
    {
        // Arrange
        var request = new AlunoRequest("João Silva", "joao.silva@email.com", new DateTime(2000, 1, 1));
        var command = new CreateAlunoCommand(request);

        _alunoRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _alunoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Aluno>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.DatabaseError, result.Error);
    }

    [Fact]
    public async Task Should_ReturnServerError_When_UnexpectedExceptionOccurs()
    {
        // Arrange
        var request = new AlunoRequest("João Silva", "joao.silva@email.com", new DateTime(2000, 1, 1));
        var command = new CreateAlunoCommand(request);

        _alunoRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email, null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.ServerError, result.Error);
    }
}