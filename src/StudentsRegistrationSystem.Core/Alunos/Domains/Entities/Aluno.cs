using StudentsRegistrationSystem.Core.BaseEntity;
using StudentsRegistrationSystem.Core.Matriculas.Domains.Entities;

namespace StudentsRegistrationSystem.Core.Alunos.Domains.Entities;

public class Aluno : Entity
{
    private readonly List<Matricula> _matriculas = new();

    protected Aluno()
    {
    }

    public Aluno(string nome, string email, DateTime dataNascimento)
    {
        Nome = nome;
        Email = email;
        DataNascimento = dataNascimento;
    }

    public string Nome { get; private set; }
    public string Email { get; private set; }
    public DateTime DataNascimento { get; private set; }
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

    public void Atualizar(string nome, string email, DateTime dataNascimento)
    {
        Nome = nome;
        Email = email;
        DataNascimento = dataNascimento;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsMaiorDeIdade()
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - DataNascimento.Year;

        if (DataNascimento.Date > hoje.AddYears(-idade))
            idade--;

        return idade >= 18;
    }
}