using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StudentsRegistrationSystem.Application.Alunos.Commands;
using StudentsRegistrationSystem.Application.Alunos.Commands.Handlers;
using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;
using StudentsRegistrationSystem.Core.Matriculas.Domains.Entities;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Data;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;

namespace StudentsRegistrationSystem.Application.UnitTests.Alunos.Commands.Handlers;

public class DeleteAlunoHandlerTests
{
    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly Mock<AppDbContext> _contextMock;
    private readonly Mock<DbSet<Matricula>> _matriculasDbSetMock;
    private readonly Mock<ILogger<DeleteAlunoCommandHandler>> _loggerMock;
    private readonly DeleteAlunoCommandHandler _handler;

    public DeleteAlunoHandlerTests()
    {
        _alunoRepositoryMock = new Mock<IAlunoRepository>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .Options;
        _contextMock = new Mock<AppDbContext>(options);

        _matriculasDbSetMock = new Mock<DbSet<Matricula>>();
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

        // Criar lista de matrículas desativadas para simular o cenário
        var matricula1 = new Matricula(alunoId, Guid.NewGuid());
        matricula1.Desativar();
        var matricula2 = new Matricula(alunoId, Guid.NewGuid());
        matricula2.Desativar();

        var matriculasDesativadas = new List<Matricula>
        {
            matricula1,
            matricula2
        }.AsQueryable();

        // Configurar o DbSet mockado para retornar as matrículas
        _matriculasDbSetMock.As<IQueryable<Matricula>>()
            .Setup(m => m.Provider)
            .Returns(matriculasDesativadas.Provider);
        _matriculasDbSetMock.As<IQueryable<Matricula>>()
            .Setup(m => m.Expression)
            .Returns(matriculasDesativadas.Expression);
        _matriculasDbSetMock.As<IQueryable<Matricula>>()
            .Setup(m => m.ElementType)
            .Returns(matriculasDesativadas.ElementType);
        _matriculasDbSetMock.As<IQueryable<Matricula>>()
            .Setup(m => m.GetEnumerator())
            .Returns(matriculasDesativadas.GetEnumerator());

        // Configurar a propriedade Matriculas do contexto
        _contextMock.Setup(x => x.Matriculas).Returns(_matriculasDbSetMock.Object);

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
        _matriculasDbSetMock.Verify(x => x.RemoveRange(It.IsAny<IEnumerable<Matricula>>()), Times.Once);
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
    public async Task Should_ReturnActiveRegistrationError_When_StudentHasActiveEnrollments()
    {
        // Arrange
        var aluno = new Aluno("João Silva", "joao@email.com", new DateTime(2000, 1, 1));
        var alunoId = aluno.Id; 
        var command = new DeleteAlunoCommand(alunoId);

        var matriculaAtiva = new Matricula(aluno.Id, Guid.NewGuid());

        var matriculasAtivas = new List<Matricula>
        {
            matriculaAtiva
        }.AsQueryable();

        // Configurar o DbSet mockado para retornar a matrícula ativa
        _matriculasDbSetMock.As<IQueryable<Matricula>>()
            .Setup(m => m.Provider)
            .Returns(matriculasAtivas.Provider);
        _matriculasDbSetMock.As<IQueryable<Matricula>>()
            .Setup(m => m.Expression)
            .Returns(matriculasAtivas.Expression);
        _matriculasDbSetMock.As<IQueryable<Matricula>>()
            .Setup(m => m.ElementType)
            .Returns(matriculasAtivas.ElementType);
        _matriculasDbSetMock.As<IQueryable<Matricula>>()
            .Setup(m => m.GetEnumerator())
            .Returns(matriculasAtivas.GetEnumerator());

        _contextMock.Setup(x => x.Matriculas).Returns(_matriculasDbSetMock.Object);

        _alunoRepositoryMock
            .Setup(x => x.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aluno);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.ActiveRegistration, result.Error);
        _alunoRepositoryMock.Verify(x => x.Delete(It.IsAny<Aluno>()), Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_ReturnDatabaseError_When_DbUpdateExceptionOccurs()
    {
        // Arrange
        var alunoId = Guid.NewGuid();
        var aluno = new Aluno("João Silva", "joao@email.com", new DateTime(2000, 1, 1));
        var command = new DeleteAlunoCommand(alunoId);

        // Criar lista de matrículas desativadas
        var matricula = new Matricula(alunoId, Guid.NewGuid());
        matricula.Desativar();

        var matriculasDesativadas = new List<Matricula>
        {
            matricula
        }.AsQueryable();

        // Configurar o DbSet mockado
        _matriculasDbSetMock.As<IQueryable<Matricula>>()
            .Setup(m => m.Provider)
            .Returns(matriculasDesativadas.Provider);
        _matriculasDbSetMock.As<IQueryable<Matricula>>()
            .Setup(m => m.Expression)
            .Returns(matriculasDesativadas.Expression);
        _matriculasDbSetMock.As<IQueryable<Matricula>>()
            .Setup(m => m.ElementType)
            .Returns(matriculasDesativadas.ElementType);
        _matriculasDbSetMock.As<IQueryable<Matricula>>()
            .Setup(m => m.GetEnumerator())
            .Returns(matriculasDesativadas.GetEnumerator());

        _contextMock.Setup(x => x.Matriculas).Returns(_matriculasDbSetMock.Object);

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
        _matriculasDbSetMock.Verify(x => x.RemoveRange(It.IsAny<IEnumerable<Matricula>>()), Times.Once);
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