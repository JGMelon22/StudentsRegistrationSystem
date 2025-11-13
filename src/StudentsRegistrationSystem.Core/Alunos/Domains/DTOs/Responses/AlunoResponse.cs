namespace StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;

public record AlunoResponse
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime DataNascimento { get; init; }
    public DateTime CreatedAt { get; init; }
}