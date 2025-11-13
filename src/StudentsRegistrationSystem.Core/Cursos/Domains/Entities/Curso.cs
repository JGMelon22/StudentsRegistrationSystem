using StudentsRegistrationSystem.Core.BaseEntity;

namespace StudentsRegistrationSystem.Core.Cursos.Domains.Entities;

public class Curso : Entity
{
    public string Nome { get; private set; }
    public string Descricao { get; private set; }

    private readonly List<Matricula> _matriculas = new();
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

    protected Curso() { } 

    public Curso(string nome, string descricao)
    {
        Nome = nome;
        Descricao = descricao;
    }

    public void Atualizar(string nome, string descricao)
    {
        Nome = nome;
        Descricao = descricao;
        UpdatedAt = DateTime.UtcNow;
    }
}