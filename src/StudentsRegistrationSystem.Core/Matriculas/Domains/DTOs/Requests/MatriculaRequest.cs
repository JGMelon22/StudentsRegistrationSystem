using System.ComponentModel.DataAnnotations;

namespace StudentsRegistrationSystem.Core.Matriculas.Domains.DTOs.Requests;

public record MatriculaRequest(
    [Required(ErrorMessage = "O ID do aluno é obrigatório")]
    Guid AlunoId,

    [Required(ErrorMessage = "O ID do curso é obrigatório")]
    Guid CursoId
);