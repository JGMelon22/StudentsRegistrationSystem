using Microsoft.Extensions.Logging;
using Moq;
using StudentsRegistrationSystem.Application.Alunos.Queries;
using StudentsRegistrationSystem.Application.Alunos.Queries.Handlers;
using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.UnitTests.Alunos.Queries.Handlers;

public class GetAlunosMatriculadosHandlerTests
{
    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly GetAlunosMatriculadosQueryHandler _handler;
    private readonly Mock<ILogger<GetAlunosMatriculadosQueryHandler>> _loggerMock;

    public GetAlunosMatriculadosHandlerTests()
    {
        _alunoRepositoryMock = new Mock<IAlunoRepository>();
        _loggerMock = new Mock<ILogger<GetAlunosMatriculadosQueryHandler>>();
        _handler = new GetAlunosMatriculadosQueryHandler(_alunoRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Should_ReturnPagedAlunos_When_AlunosMatriculadosExist()
    {
        // Arrange
        var alunos = new List<Aluno>
        {
            new("João Silva", "joao@email.com", new DateTime(2000, 1, 1)),
            new("Maria Santos", "maria@email.com", new DateTime(1999, 5, 15))
        };

        var pagedResponse = new PagedResponseOffset<Aluno>(alunos, 1, 10, 2);
        var query = new GetAlunosMatriculadosQuery();

        _alunoRepositoryMock
            .Setup(x => x.GetAlunosMatriculadosAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.TotalRecords);
        Assert.Equal(2, result.Value.Data.Count);
        Assert.Equal("João Silva", result.Value.Data[0].Nome);
        Assert.Equal("Maria Santos", result.Value.Data[1].Nome);
    }

    [Fact]
    public async Task Should_ReturnEmptyList_When_NoAlunosMatriculados()
    {
        // Arrange
        var pagedResponse = new PagedResponseOffset<Aluno>(new List<Aluno>(), 1, 10, 0);
        var query = new GetAlunosMatriculadosQuery();

        _alunoRepositoryMock
            .Setup(x => x.GetAlunosMatriculadosAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(0, result.Value.TotalRecords);
        Assert.Empty(result.Value.Data);
    }
}