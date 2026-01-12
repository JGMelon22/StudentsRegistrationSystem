using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StudentsRegistrationSystem.Application.Cursos.Commands;
using StudentsRegistrationSystem.Application.Cursos.Commands.Handlers;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Requests;
using StudentsRegistrationSystem.Core.Cursos.Domains.Entities;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.UnitTests.Cursors.Commands.Handlers;

public class CreateCursoCommandHandlerTests
{
    private readonly Mock<AppDbContext> _contextMock;
    private readonly Mock<ICursoRepository> _cursoRepositoryMock;
    private readonly CreateCursoCommandHandler _handler;
    private readonly Mock<ILogger<CreateCursoCommandHandler>> _loggerMock;

    public CreateCursoCommandHandlerTests()
    {
        _cursoRepositoryMock = new Mock<ICursoRepository>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .Options;
        _contextMock = new Mock<AppDbContext>(options);

        _loggerMock = new Mock<ILogger<CreateCursoCommandHandler>>();
        _handler = new CreateCursoCommandHandler(_cursoRepositoryMock.Object, _contextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Should_CreateCurso_When_ValidRequestProvided()
    {
        // Arrange
        var request = new CursoRequest
        (
            "Matemática Avançada",
            "Curso de matemática para nível avançado"
        );

        var command = new CreateCursoCommand(request);

        _cursoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Curso>(), It.IsAny<CancellationToken>()))
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
        Assert.Equal(request.Descricao, result.Value.Descricao);
        _cursoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Curso>(), It.IsAny<CancellationToken>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}