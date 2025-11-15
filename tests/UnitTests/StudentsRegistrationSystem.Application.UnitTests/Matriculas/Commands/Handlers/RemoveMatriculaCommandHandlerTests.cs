using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StudentsRegistrationSystem.Application.Matriculas.Commands;
using StudentsRegistrationSystem.Application.Matriculas.Commands.Handlers;
using StudentsRegistrationSystem.Core.Matriculas.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.UnitTests.Matriculas.Commands.Handlers;

public class RemoveMatriculaCommandHandlerTests
{
    private readonly Mock<IMatriculaRepository> _matriculaRepositoryMock;
    private readonly Mock<AppDbContext> _contextMock;
    private readonly Mock<ILogger<RemoveMatriculaCommandHandler>> _loggerMock;
    private readonly RemoveMatriculaCommandHandler _handler;

    public RemoveMatriculaCommandHandlerTests()
    {
        _matriculaRepositoryMock = new Mock<IMatriculaRepository>();
        _contextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _loggerMock = new Mock<ILogger<RemoveMatriculaCommandHandler>>();

        _handler = new RemoveMatriculaCommandHandler(
            _matriculaRepositoryMock.Object,
            _contextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_MatriculaIsRemovedSuccessfully()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var command = new RemoveMatriculaCommand(alunoId, cursoId);
        var matricula = new Matricula(alunoId, cursoId);

        _matriculaRepositoryMock
            .Setup(x => x.GetMatriculaAtivaAsync(alunoId, cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(matricula);

        _matriculaRepositoryMock
            .Setup(x => x.Update(It.IsAny<Matricula>()));

        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        matricula.Ativa.Should().BeFalse();

        _matriculaRepositoryMock.Verify(
            x => x.GetMatriculaAtivaAsync(alunoId, cursoId, It.IsAny<CancellationToken>()),
            Times.Once);

        _matriculaRepositoryMock.Verify(
            x => x.Update(It.Is<Matricula>(m => !m.Ativa)),
            Times.Once);

        _contextMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_MatriculaNotFound()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var command = new RemoveMatriculaCommand(alunoId, cursoId);

        _matriculaRepositoryMock
            .Setup(x => x.GetMatriculaAtivaAsync(alunoId, cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Matricula?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(Error.EnrollmentNotFound);

        _matriculaRepositoryMock.Verify(
            x => x.GetMatriculaAtivaAsync(alunoId, cursoId, It.IsAny<CancellationToken>()),
            Times.Once);

        _matriculaRepositoryMock.Verify(
            x => x.Update(It.IsAny<Matricula>()),
            Times.Never);

        _contextMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Matrícula não encontrada")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_DbUpdateExceptionOccurs()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var command = new RemoveMatriculaCommand(alunoId, cursoId);
        var matricula = new Matricula(alunoId, cursoId);
        var dbException = new DbUpdateException("Database error");

        _matriculaRepositoryMock
            .Setup(x => x.GetMatriculaAtivaAsync(alunoId, cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(matricula);

        _matriculaRepositoryMock
            .Setup(x => x.Update(It.IsAny<Matricula>()));

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
        var cursoId = Guid.NewGuid();
        var command = new RemoveMatriculaCommand(alunoId, cursoId);
        var unexpectedException = new Exception("Unexpected error");

        _matriculaRepositoryMock
            .Setup(x => x.GetMatriculaAtivaAsync(alunoId, cursoId, It.IsAny<CancellationToken>()))
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

    [Fact]
    public async Task Should_CallDesativarMethod_When_MatriculaIsFound()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var command = new RemoveMatriculaCommand(alunoId, cursoId);
        var matricula = new Matricula(alunoId, cursoId);

        _matriculaRepositoryMock
            .Setup(x => x.GetMatriculaAtivaAsync(alunoId, cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(matricula);

        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        matricula.Ativa.Should().BeFalse();
        matricula.UpdatedAt.Should().NotBeNull();
    }
}