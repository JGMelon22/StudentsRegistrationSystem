using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StudentsRegistrationSystem.Application.Matriculas.Commands;
using StudentsRegistrationSystem.Application.Matriculas.Commands.Handlers;
using StudentsRegistrationSystem.Core.Matriculas.Domains.DTOs.Requests;
using StudentsRegistrationSystem.Core.Matriculas.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.UnitTests.Matriculas.Commands.Handlers;

public class CreateMatriculaCommandHandlerTests
{
    private readonly Mock<IMatriculaRepository> _matriculaRepositoryMock;
    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly Mock<ICursoRepository> _cursoRepositoryMock;
    private readonly Mock<AppDbContext> _contextMock;
    private readonly Mock<ILogger<CreateMatriculaCommandHandler>> _loggerMock;
    private readonly CreateMatriculaCommandHandler _handler;

    public CreateMatriculaCommandHandlerTests()
    {
        _matriculaRepositoryMock = new Mock<IMatriculaRepository>();
        _alunoRepositoryMock = new Mock<IAlunoRepository>();
        _cursoRepositoryMock = new Mock<ICursoRepository>();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .Options;
        _contextMock = new Mock<AppDbContext>(options);

        _loggerMock = new Mock<ILogger<CreateMatriculaCommandHandler>>();
        _handler = new CreateMatriculaCommandHandler(
            _matriculaRepositoryMock.Object,
            _alunoRepositoryMock.Object,
            _cursoRepositoryMock.Object,
            _contextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Should_CreateMatricula_When_ValidRequestProvided()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var request = new MatriculaRequest(alunoId, cursoId);

        var command = new CreateMatriculaCommand(request);
        var matricula = new Matricula(alunoId, cursoId);

        _alunoRepositoryMock
            .Setup(x => x.ExistsAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _cursoRepositoryMock
            .Setup(x => x.ExistsAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _matriculaRepositoryMock
            .Setup(x => x.AlunoJaMatriculadoAsync(alunoId, cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _matriculaRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(matricula);

        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        _matriculaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Matricula>(), It.IsAny<CancellationToken>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_AlunoNotFound()
    {
        // Arrange
        var request = new MatriculaRequest(Guid.NewGuid(), Guid.NewGuid());
        var command = new CreateMatriculaCommand(request);

        _alunoRepositoryMock
            .Setup(x => x.ExistsAsync(request.AlunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.StudentNotFound, result.Error);
        _matriculaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Matricula>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_CursoNotFound()
    {
        // Arrange
        var request = new MatriculaRequest(Guid.NewGuid(), Guid.NewGuid());
        var command = new CreateMatriculaCommand(request);

        _alunoRepositoryMock
            .Setup(x => x.ExistsAsync(request.AlunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _cursoRepositoryMock
            .Setup(x => x.ExistsAsync(request.CursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.CourseNotFound, result.Error);
        _matriculaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Matricula>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_AlunoAlreadyEnrolled()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var request = new MatriculaRequest(alunoId, cursoId);

        var command = new CreateMatriculaCommand(request);

        _alunoRepositoryMock
            .Setup(x => x.ExistsAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _cursoRepositoryMock
            .Setup(x => x.ExistsAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _matriculaRepositoryMock
            .Setup(x => x.AlunoJaMatriculadoAsync(alunoId, cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.EnrollmentAlreadyEnrolled, result.Error);
        _matriculaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Matricula>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_ReturnDatabaseError_When_DbUpdateExceptionOccurs()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();
        var request = new MatriculaRequest(alunoId, cursoId);

        var command = new CreateMatriculaCommand(request);

        _alunoRepositoryMock
            .Setup(x => x.ExistsAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _cursoRepositoryMock
            .Setup(x => x.ExistsAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _matriculaRepositoryMock
            .Setup(x => x.AlunoJaMatriculadoAsync(alunoId, cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

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
        var request = new MatriculaRequest(Guid.NewGuid(), Guid.NewGuid());
        var command = new CreateMatriculaCommand(request);

        _alunoRepositoryMock
            .Setup(x => x.ExistsAsync(request.AlunoId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.ServerError, result.Error);
    }
}