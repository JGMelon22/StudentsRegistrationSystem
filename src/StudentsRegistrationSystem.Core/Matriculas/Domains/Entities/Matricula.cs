using StudentsRegistrationSystem.Core.Alunos.Domains.Entities;
using StudentsRegistrationSystem.Core.BaseEntity;
using StudentsRegistrationSystem.Core.Cursos.Domains.Entities;

namespace StudentsRegistrationSystem.Core.Matriculas.Domains.Entities;

public class Matricula : Entity
{
    public Guid AlunoId { get; private set; }
    public Aluno Aluno { get; private set; }

    public Guid CursoId { get; private set; }
    public Curso Curso { get; private set; }

    public DateTime DataMatricula { get; private set; }
    public bool Ativa { get; private set; }

    protected Matricula() { }

    public Matricula(Guid alunoId, Guid cursoId)
    {
        AlunoId = alunoId;
        CursoId = cursoId;
        DataMatricula = DateTime.UtcNow;
        Ativa = true;
    }

    public void Desativar()
    {
        Ativa = false;
        UpdatedAt = DateTime.UtcNow;
    }
}