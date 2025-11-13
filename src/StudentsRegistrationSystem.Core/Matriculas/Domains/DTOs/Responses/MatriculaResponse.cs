namespace StudentsRegistrationSystem.Core.Matriculas.Domains.DTOs.Responses;

public record MatriculaResponse
{
    public Guid Id { get; init; }
    public Guid AlunoId { get; init; }
    public string AlunoNome { get; init; } = string.Empty;
    public Guid CursoId { get; init; }
    public string CursoNome { get; init; } = string.Empty;
    public DateTime DataMatricula { get; init; }
    public bool Ativa { get; init; }
}