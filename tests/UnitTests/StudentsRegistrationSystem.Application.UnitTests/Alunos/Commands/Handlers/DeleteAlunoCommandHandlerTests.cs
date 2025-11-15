using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StudentsRegistrationSystem.Application.Alunos.Commands;
using StudentsRegistrationSystem.Application.Alunos.Commands.Handlers;
using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.UnitTests.Alunos.Commands.Handlers;

public class DeleteAlunoHandlerTests
{
    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly Mock<AppDbContext> _contextMock;
    private readonly Mock<ILogger<DeleteAlunoCommandHandler>> _loggerMock;
    private readonly DeleteAlunoCommandHandler _handler;

    public DeleteAlunoHandlerTests()
    {
        _alunoRepositoryMock = new Mock<IAlunoRepository>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .Options;
        _contextMock = new Mock<AppDbContext>(options);

        _loggerMock = new Mock<ILogger<DeleteAlunoCommandHandler>>();
        _handler = new DeleteAlunoCommandHandler(
            _alunoRepositoryMock.Object,
            _contextMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Should_DeleteAluno_When_AlunoExists()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var aluno = new Aluno("João Silva", "joao@email.com", new DateTime(2000, 1, 1));
        var command = new DeleteAlunoCommand(alunoId);

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aluno);

        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        _alunoRepositoryMock.Verify(x => x.Delete(aluno), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_AlunoNotFound()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var command = new DeleteAlunoCommand(alunoId);

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Aluno?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.StudentNotFound, result.Error);
        _alunoRepositoryMock.Verify(x => x.Delete(It.IsAny<Aluno>()), Times.Never);
    }

    [Fact]
    public async Task Should_ReturnDatabaseError_When_DbUpdateExceptionOccurs()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var aluno = new Aluno("João Silva", "joao@email.com", new DateTime(2000, 1, 1));
        var command = new DeleteAlunoCommand(alunoId);

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aluno);

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
        var alunoId = Guid.NewGuid();
        var command = new DeleteAlunoCommand(alunoId);

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.ServerError, result.Error);
    }
}