using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Requests;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;

namespace StudentsRegistrationSystem.Core.Alunos.Domains.Mappings;

public static class MappingExtensions
{
    public static Aluno ToDomain(this AlunoRequest request)
        => new(request.Nome, request.Email, request.DataNascimento);

    public static AlunoResponse ToResponse(this Aluno aluno)
        => new()
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Email = aluno.Email,
            DataNascimento = aluno.DataNascimento,
            CreatedAt = aluno.CreatedAt
        };

    public static IEnumerable<AlunoResponse> ToResponse(this IEnumerable<Aluno> alunos)
        => alunos.Select(aluno => aluno.ToResponse());
}