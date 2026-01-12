using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using StudentsRegistrationSystem.Application.Cursos.Queries;
using StudentsRegistrationSystem.Application.Cursos.Queries.Handlers;
using StudentsRegistrationSystem.Core.Cursos.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.UnitTests.Cursors.Queries.Handlers;

public class GetAllCursosQueryHandlerTests
{
    private readonly Mock<ICursoRepository> _cursoRepositoryMock;
    private readonly GetAllCursosQueryHandler _handler;
    private readonly Mock<ILogger<GetAllCursosQueryHandler>> _loggerMock;

    public GetAllCursosQueryHandlerTests()
    {
        _cursoRepositoryMock = new Mock<ICursoRepository>();
        _loggerMock = new Mock<ILogger<GetAllCursosQueryHandler>>();

        _handler = new GetAllCursosQueryHandler(
            _cursoRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Should_ReturnPagedCursosResponse_When_CursosExist()
    {
        // Arrange
        var query = new GetAllCursosQuery();

        var cursos = new List<Curso>
        {
            new("Curso de C#", "Descrição do curso de C#"),
            new("Curso de Java", "Descrição do curso de Java"),
            new("Curso de Python", "Descrição do curso de Python")
        };

        var pagedCursos = new PagedResponseOffset<Curso>(cursos, 1, 10, 3);

        _cursoRepositoryMock
            .Setup(x => x.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedCursos);

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

        _cursoRepositoryMock.Verify(
            x => x.GetAllAsync(1, 10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ReturnEmptyList_When_NoCursosExist()
    {
        // Arrange
        var query = new GetAllCursosQuery();

        var emptyPagedCursos = new PagedResponseOffset<Curso>(new List<Curso>(), 1, 10, 0);

        _cursoRepositoryMock
            .Setup(x => x.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyPagedCursos);

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
        var query = new GetAllCursosQuery(2, 5);

        var cursos = new List<Curso>
        {
            new("Curso 6", "Descrição 6"),
            new("Curso 7", "Descrição 7")
        };

        var pagedCursos = new PagedResponseOffset<Curso>(cursos, 2, 5, 12);

        _cursoRepositoryMock
            .Setup(x => x.GetAllAsync(2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedCursos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value!.PageNumber.Should().Be(2);
        result.Value.PageSize.Should().Be(5);
        result.Value.TotalPages.Should().Be(3);

        _cursoRepositoryMock.Verify(
            x => x.GetAllAsync(2, 5, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}