using StudentsRegistrationSystem.Core.BaseEntity;

namespace StudentsRegistrationSystem.Core.Alunos.Domains.Entities;

public class Aluno : Entity
{
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public DateTime DataNascimento { get; private set; }

    private readonly List<Matricula> _matriculas = new();
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

    protected Aluno() { } 

    public Aluno(string nome, string email, DateTime dataNascimento)
    {
        Nome = nome;
        Email = email;
        DataNascimento = dataNascimento;
    }

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
