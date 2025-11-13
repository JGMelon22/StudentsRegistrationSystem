using System.ComponentModel.DataAnnotations;

namespace StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Requests;

public record CursoRequest(
    [Required(ErrorMessage = "O nome do curso é obrigatório")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 200 caracteres")]
    string Nome,

    [Required(ErrorMessage = "A descrição do curso é obrigatória")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 1000 caracteres")]
    string Descricao
);
