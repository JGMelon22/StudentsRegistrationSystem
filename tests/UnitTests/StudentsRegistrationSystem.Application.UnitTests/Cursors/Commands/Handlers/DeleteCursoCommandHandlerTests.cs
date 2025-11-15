using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StudentsRegistrationSystem.Application.Cursos.Commands;
using StudentsRegistrationSystem.Application.Cursos.Commands.Handlers;
using StudentsRegistrationSystem.Core.Cursos.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.UnitTests.Cursors.Commands.Handlers;

public class DeleteCursoCommandHandlerTests
{
    private readonly Mock<ICursoRepository> _cursoRepositoryMock;
    private readonly Mock<AppDbContext> _contextMock;
    private readonly Mock<ILogger<DeleteCursoCommandHandler>> _loggerMock;
    private readonly DeleteCursoCommandHandler _handler;

    public DeleteCursoCommandHandlerTests()
    {
        _cursoRepositoryMock = new Mock<ICursoRepository>();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .Options;
        _contextMock = new Mock<AppDbContext>(options);

        _loggerMock = new Mock<ILogger<DeleteCursoCommandHandler>>();
        _handler = new DeleteCursoCommandHandler(_cursoRepositoryMock.Object, _contextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Should_DeleteCurso_When_CursoExists()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var curso = new Curso("Matemática", "Curso de matemática básica");
        var command = new DeleteCursoCommand(cursoId);

        _cursoRepositoryMock
            .Setup(x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(curso);

        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        _cursoRepositoryMock.Verify(x => x.Delete(curso), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnFailure_When_CursoNotFound()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var command = new DeleteCursoCommand(cursoId);

        _cursoRepositoryMock
            .Setup(x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Curso?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.CourseNotFound, result.Error);
        _cursoRepositoryMock.Verify(x => x.Delete(It.IsAny<Curso>()), Times.Never);
    }

    [Fact]
    public async Task Should_ReturnDatabaseError_When_DbUpdateExceptionOccurs()
    {
        // Arrange
        var cursoId = Guid.NewGuid();
        var curso = new Curso("Matemática", "Curso de matemática básica");
        var command = new DeleteCursoCommand(cursoId);

        _cursoRepositoryMock
            .Setup(x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(curso);

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
        var cursoId = Guid.NewGuid();
        var command = new DeleteCursoCommand(cursoId);

        _cursoRepositoryMock
            .Setup(x => x.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.ServerError, result.Error);
    }
}