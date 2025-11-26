# Princípios SOLID no StudentsRegistrationSystem

Este documento explica como os princípios SOLID estão aplicados neste projeto, preparando você para explicar de forma simples e profissional em entrevistas técnicas.

---

## 📌 O que é SOLID?

SOLID é um acrônimo para cinco princípios de design de software orientado a objetos que tornam o código mais:
- **Manutenível** - Fácil de modificar
- **Extensível** - Fácil de adicionar novas funcionalidades
- **Testável** - Fácil de criar testes unitários
- **Desacoplado** - Baixa dependência entre componentes

---

## 🔤 S - Single Responsibility Principle (Princípio da Responsabilidade Única)

### Definição
> Uma classe deve ter apenas uma razão para mudar, ou seja, apenas uma responsabilidade.

### Como explicar em entrevista
*"Cada classe no projeto tem uma única responsabilidade bem definida. Por exemplo, o `AlunoRepository` só lida com persistência de dados de alunos, enquanto o `CreateAlunoCommandHandler` só lida com a lógica de criação de alunos."*

### Exemplos no projeto

**1. Handlers com Responsabilidade Única:**
```csharp
// CreateAlunoCommandHandler.cs - Só lida com criação de alunos
public class CreateAlunoCommandHandler : IRequestHandler<CreateAlunoCommand, Result<AlunoResponse>>
{
    public async Task<Result<AlunoResponse>> Handle(CreateAlunoCommand command, CancellationToken cancellationToken)
    {
        // Lógica específica para CRIAR alunos
    }
}

// GetAlunoByIdQueryHandler.cs - Só lida com consulta de alunos
public class GetAlunoByIdQueryHandler : IRequestHandler<GetAlunoByIdQuery, Result<AlunoResponse>>
{
    public async Task<Result<AlunoResponse>> Handle(GetAlunoByIdQuery query, CancellationToken cancellationToken)
    {
        // Lógica específica para BUSCAR alunos
    }
}
```

**2. Separação Command/Query (CQRS):**
- `Commands/` - Responsável por operações de escrita (Create, Update, Delete)
- `Queries/` - Responsável por operações de leitura (Get, GetAll)

**3. Repositórios Específicos:**
```csharp
// AlunoRepository.cs - Só lida com dados de Aluno
public class AlunoRepository : Repository<Aluno>, IAlunoRepository { }

// CursoRepository.cs - Só lida com dados de Curso
public class CursoRepository : Repository<Curso>, ICursoRepository { }
```

---

## 🔤 O - Open/Closed Principle (Princípio Aberto/Fechado)

### Definição
> Classes devem estar abertas para extensão, mas fechadas para modificação.

### Como explicar em entrevista
*"Usamos herança e abstrações para permitir extensões sem modificar código existente. Por exemplo, o `Repository<T>` genérico fornece operações CRUD básicas, e os repositórios específicos como `AlunoRepository` estendem esse comportamento sem alterar a classe base."*

### Exemplos no projeto

**1. Repository Genérico Extensível:**
```csharp
// Repository.cs - Classe base com operações genéricas
public class Repository<T> : IRepository<T> where T : Entity
{
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(id, cancellationToken);
    }
    // Operações CRUD básicas...
}

// AlunoRepository.cs - Estende sem modificar a base
public class AlunoRepository : Repository<Aluno>, IAlunoRepository
{
    // Adiciona método específico SEM alterar Repository<T>
    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        // Implementação específica para Aluno
    }
}
```

**2. Entidade Base:**
```csharp
// Entity.cs - Classe base fechada para modificação
public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
}

// Aluno.cs - Estende Entity adicionando comportamentos específicos
public class Aluno : Entity
{
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public DateTime DataNascimento { get; private set; }
    
    public bool IsMaiorDeIdade() { /* Comportamento específico */ }
}
```

---

## 🔤 L - Liskov Substitution Principle (Princípio da Substituição de Liskov)

### Definição
> Objetos de uma classe derivada devem poder substituir objetos da classe base sem alterar o comportamento correto do programa.

### Como explicar em entrevista
*"Todas as nossas classes derivadas, como `AlunoRepository`, podem substituir a classe base `Repository<Aluno>` sem problemas. Se uma função espera um `IRepository<Aluno>`, podemos passar um `AlunoRepository` e o sistema funciona corretamente."*

### Exemplos no projeto

**1. Repositórios Substituíveis:**
```csharp
// A interface genérica...
public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    // ...
}

// Pode ser substituída por qualquer implementação que a estenda
public interface IAlunoRepository : IRepository<Aluno>
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    // Métodos adicionais específicos
}
```

**2. Handlers Intercambiáveis:**
```csharp
// Todos os handlers implementam IRequestHandler<TRequest, TResponse>
// O Mediator pode usar qualquer handler que implemente a interface

public class CreateAlunoCommandHandler : IRequestHandler<CreateAlunoCommand, Result<AlunoResponse>> { }
public class UpdateAlunoCommandHandler : IRequestHandler<UpdateAlunoCommand, Result<AlunoResponse>> { }
```

---

## 🔤 I - Interface Segregation Principle (Princípio da Segregação de Interfaces)

### Definição
> Clientes não devem ser forçados a depender de interfaces que não utilizam.

### Como explicar em entrevista
*"Criamos interfaces específicas para cada domínio. Em vez de uma interface `IRepositório` gigante com todos os métodos, temos `IAlunoRepository`, `ICursoRepository` e `IMatriculaRepository`, cada um com métodos específicos do seu domínio."*

### Exemplos no projeto

**1. Interfaces de Repositório Segregadas:**
```csharp
// Interface base genérica e enxuta
public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponseOffset<T>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}

// Interface específica para Aluno - só tem métodos que Aluno precisa
public interface IAlunoRepository : IRepository<Aluno>
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<PagedResponseOffset<Aluno>> GetAlunosMatriculadosAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}

// Interface específica para Curso
public interface ICursoRepository : IRepository<Curso>
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponseOffset<Aluno>> GetAlunosByCursoIdAsync(Guid cursoId, int PageNumber = 1, int PageSize = 10, CancellationToken cancellationToken = default);
}

// Interface específica para Matrícula
public interface IMatriculaRepository : IRepository<Matricula>
{
    Task<bool> AlunoJaMatriculadoAsync(Guid alunoId, Guid cursoId, CancellationToken cancellationToken = default);
    Task<Matricula?> GetMatriculaAtivaAsync(Guid alunoId, Guid cursoId, CancellationToken cancellationToken = default);
}
```

**2. Commands e Queries Segregados:**
```csharp
// Cada operação tem sua própria interface/record
public record CreateAlunoCommand(AlunoRequest Request) : IRequest<Result<AlunoResponse>>;
public record UpdateAlunoCommand(Guid Id, AlunoRequest Request) : IRequest<Result<AlunoResponse>>;
public record DeleteAlunoCommand(Guid Id) : IRequest<Result<bool>>;
public record GetAlunoByIdQuery(Guid Id) : IRequest<Result<AlunoResponse>>;
```

---

## 🔤 D - Dependency Inversion Principle (Princípio da Inversão de Dependência)

### Definição
> Módulos de alto nível não devem depender de módulos de baixo nível. Ambos devem depender de abstrações.

### Como explicar em entrevista
*"Os handlers da camada de Application não dependem diretamente das implementações de repositório. Eles dependem das interfaces (`IAlunoRepository`), e a injeção de dependência resolve qual implementação usar. Isso permite trocar a implementação sem alterar a lógica de negócio."*

### Exemplos no projeto

**1. Handlers Dependem de Abstrações:**
```csharp
public class CreateAlunoCommandHandler : IRequestHandler<CreateAlunoCommand, Result<AlunoResponse>>
{
    // Depende de INTERFACES, não de implementações concretas
    private readonly IAlunoRepository _alunoRepository;
    private readonly ILogger<CreateAlunoCommandHandler> _logger;

    public CreateAlunoCommandHandler(
        IAlunoRepository alunoRepository,  // Interface injetada
        ILogger<CreateAlunoCommandHandler> logger)  // Interface injetada
    {
        _alunoRepository = alunoRepository;
        _logger = logger;
    }
}
```

**2. Controllers Dependem do Mediator (Abstração):**
```csharp
public class AlunosController : ControllerBase
{
    private readonly IMediator _mediator;  // Depende da interface

    public AlunosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        // Usa abstração para enviar comandos/queries
        var result = await _mediator.Send(new GetAlunoByIdQuery(id));
        // ...
    }
}
```

**3. Configuração de Injeção de Dependência:**
```csharp
// IocExtensions.cs - Configuração centralizada
public static IServiceCollection AddRepositories(this IServiceCollection services)
{
    // Registra INTERFACES apontando para IMPLEMENTAÇÕES
    services.AddScoped<IAlunoRepository, AlunoRepository>();
    services.AddScoped<ICursoRepository, CursoRepository>();
    services.AddScoped<IMatriculaRepository, MatriculaRepository>();

    return services;
}
```

---

## 🎯 Resumo para Entrevista

| Princípio | Aplicação no Projeto | Frase-chave |
|-----------|---------------------|-------------|
| **S**ingle Responsibility | Handlers separados por operação, CQRS | *"Cada classe faz uma coisa só e faz bem"* |
| **O**pen/Closed | Repository genérico extensível | *"Extensão por herança, não modificação"* |
| **L**iskov Substitution | Repositórios derivados substituíveis | *"Derivados funcionam onde a base funciona"* |
| **I**nterface Segregation | Interfaces específicas por domínio | *"Interfaces pequenas e focadas"* |
| **D**ependency Inversion | DI com interfaces, Mediator | *"Dependa de abstrações, não implementações"* |

---

## 💡 Dicas para Entrevista

1. **Comece simples**: *"SOLID são 5 princípios que ajudam a criar código limpo e manutenível."*

2. **Dê exemplos concretos**: Mencione classes específicas do projeto como `CreateAlunoCommandHandler` ou `IAlunoRepository`.

3. **Conecte com benefícios**:
   - *"Isso facilita os testes porque podemos mockar as interfaces"*
   - *"Isso permite adicionar novas funcionalidades sem quebrar as existentes"*

4. **Mencione padrões relacionados**:
   - Repository Pattern
   - CQRS (Command Query Responsibility Segregation)
   - Dependency Injection
   - Mediator Pattern

5. **Seja honesto**: Se não lembrar de um princípio, explique o conceito geral e dê um exemplo prático.

---

## 📚 Estrutura do Projeto que Demonstra SOLID

```
StudentsRegistrationSystem/
├── Core/                           # Domínio puro, sem dependências externas
│   ├── Alunos/Domains/Entities/    # Entidades de domínio
│   ├── Shared/                     # Result Pattern, Errors
│   └── BaseEntity/                 # Entidade abstrata base
│
├── Application/                    # Casos de uso (Use Cases)
│   ├── Alunos/Commands/            # SRP: Handlers de escrita
│   └── Alunos/Queries/             # SRP: Handlers de leitura
│
├── Infrastructure/                 # Implementações concretas
│   ├── Interfaces/Repositories/    # ISP: Interfaces segregadas
│   └── Repositories/               # OCP: Implementações extensíveis
│
└── API/                            # Controllers
    └── Extensions/                 # DIP: Configuração de DI
```

Esta arquitetura em camadas também segue os princípios de **Clean Architecture**, onde:
- Camadas internas (Core) não conhecem camadas externas
- Dependências apontam para dentro (em direção ao domínio)
- Abstrações definem contratos entre camadas
