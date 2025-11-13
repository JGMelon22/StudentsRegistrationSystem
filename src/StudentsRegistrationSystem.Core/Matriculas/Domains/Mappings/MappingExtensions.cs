using StudentsRegistrationSystem.Core.Matriculas.Domains.DTOs.Requests;
using StudentsRegistrationSystem.Core.Matriculas.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Matriculas.Domains.Entities;

namespace StudentsRegistrationSystem.Core.Matriculas.Domains.Mappings;

public static class MappingExtensions
{
    public static Matricula ToDomain(this MatriculaRequest request)
        => new(request.AlunoId, request.CursoId);

    public static MatriculaResponse ToResponse(this Matricula matricula)
        => new()
        {
            Id = matricula.Id,
            AlunoId = matricula.AlunoId,
            AlunoNome = matricula.Aluno?.Nome ?? string.Empty,
            CursoId = matricula.CursoId,
            CursoNome = matricula.Curso?.Nome ?? string.Empty,
            DataMatricula = matricula.DataMatricula,
            Ativa = matricula.Ativa
        };

    public static IEnumerable<MatriculaResponse> ToResponse(this IEnumerable<Matricula> matriculas)
        => matriculas.Select(matricula => matricula.ToResponse());
}