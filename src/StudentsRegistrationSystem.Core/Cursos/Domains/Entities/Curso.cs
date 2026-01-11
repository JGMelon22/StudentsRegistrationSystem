using StudentsRegistrationSystem.Core.BaseEntity;
using StudentsRegistrationSystem.Core.Matriculas.Domains.Entities;

namespace StudentsRegistrationSystem.Core.Cursos.Domains.Entities;

public class Curso : Entity
{
    private readonly List<Matricula> _matriculas = new();

    protected Curso()
    {
    }

    public Curso(string nome, string descricao)
    {
        Nome = nome;
        Descricao = descricao;
    }

    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

    public void Atualizar(string nome, string descricao)
    {
        Nome = nome;
        Descricao = descricao;
        UpdatedAt = DateTime.UtcNow;
    }
}