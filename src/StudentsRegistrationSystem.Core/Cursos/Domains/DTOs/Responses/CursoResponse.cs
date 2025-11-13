namespace StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;

public record CursoResponse
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
