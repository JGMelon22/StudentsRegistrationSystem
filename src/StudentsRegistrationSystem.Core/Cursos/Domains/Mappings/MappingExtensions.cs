using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Requests;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Cursos.Domains.Entities;

namespace StudentsRegistrationSystem.Core.Cursos.Domains.Mappings;

public static class MappingExtensions
{
    public static Curso ToDomain(this CursoRequest request)
    {
        return new Curso(request.Nome, request.Descricao);
    }

    public static CursoResponse ToResponse(this Curso curso)
    {
        return new CursoResponse
        {
            Id = curso.Id,
            Nome = curso.Nome,
            Descricao = curso.Descricao,
            CreatedAt = curso.CreatedAt
        };
    }

    public static IEnumerable<CursoResponse> ToResponse(this IEnumerable<Curso> cursos)
    {
        return cursos.Select(curso => curso.ToResponse());
    }
}