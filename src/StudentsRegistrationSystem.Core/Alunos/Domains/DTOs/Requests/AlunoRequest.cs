using System.ComponentModel.DataAnnotations;

namespace StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Requests;

public record AlunoRequest(
    [Required(ErrorMessage = "O nome do aluno é obrigatório")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 200 caracteres")]
    string Nome,

    [Required(ErrorMessage = "O e-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [StringLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres")]
    string Email,

    [Required(ErrorMessage = "A data de nascimento é obrigatória")]
    DateTime DataNascimento
);
