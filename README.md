# Sistema de Gerenciamento de Matrículas

API RESTful desenvolvida em .NET 8 para gerenciamento de cursos, alunos e matrículas.

## 📋 Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB, Express ou versão completa)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

## 🏗️ Arquitetura

O projeto segue os princípios de **Domain-Driven Design (DDD)** e **Clean Architecture**, organizado em 4 camadas:

```
StudentsRegistrationSystem/
├── src/
│   ├── StudentsRegistrationSystem.Core/          # Entidades, DTOs, Interfaces
│   ├── StudentsRegistrationSystem.Application/   # Handlers, Commands, Queries (CQRS)
│   ├── StudentsRegistrationSystem.Infrastructure/# Repositórios, DbContext, Migrations
│   └── StudentsRegistrationSystem.API/           # Controllers, Configuração
└── tests/
    └── UnitTests/
        ├── StudentsRegistrationSystem.API.UnitTests/        # Testes dos Controllers
        └── StudentsRegistrationSystem.Application.UnitTests/# Testes de Handlers
```

### Padrões Utilizados

- **DDD (Domain-Driven Design)**: Organização em camadas com foco no domínio
- **CQRS**: Separação de Commands (escrita) e Queries (leitura)
- **Result Pattern**: Gerenciamento de erros sem exceptions
- **Repository Pattern**: Abstração do acesso a dados
- **Mediator Pattern**: Desacoplamento com NetDevPack.Mediator

## 🚀 Como Rodar o Projeto

### 1. Clonar o Repositório

```bash
git clone <url-do-repositorio>
cd StudentsRegistrationSystem
```

### 2. Configurar Connection String

Edite o arquivo `src/StudentsRegistrationSystem.API/appsettings.json` e ajuste a connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<HOST>,<PORT>;Database=<DATABASE_NAME>;User Id=<USERNAME>;Password=<PASSWORD>;TrustServerCertificate=True;"
  }
}
```

**Alternativas de Connection String:**

- **SQL Server LocalDB**: `Server=(localdb)\\mssqllocaldb;Database=StudentsRegistrationSystemDb;Trusted_Connection=True;`
- **SQL Server com usuário/senha**: `Server=localhost;Database=StudentsRegistrationSystemDb;User Id=seu_usuario;Password=sua_senha;TrustServerCertificate=True;`
- **SQL Server Express**: `Server=localhost\\SQLEXPRESS;Database=StudentsRegistrationSystemDb;Trusted_Connection=True;TrustServerCertificate=True;`

### 3. Restaurar Pacotes

```bash
dotnet restore
```

### 4. Aplicar Migrations

```bash
# Instalar ferramenta EF Core (se necessário)
dotnet tool install --global dotnet-ef

# Aplicar migrations
dotnet ef database update --project src/StudentsRegistrationSystem.Infrastructure --startup-project src/StudentsRegistrationSystem.API
```

### 5. Executar a API

```bash
cd src/StudentsRegistrationSystem.API
dotnet run
```

A API estará disponível em:
- **HTTPS**: https://localhost:7034
- **HTTP**: http://localhost:5238
- **Swagger**: https://localhost:7034/swagger (documentação interativa)

## 📚 Documentação da API

### Endpoints de Cursos

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/cursos` | Listar todos os cursos |
| GET | `/api/cursos/{id}` | Buscar curso por ID |
| POST | `/api/cursos` | Criar novo curso |
| PUT | `/api/cursos/{id}` | Atualizar curso |
| DELETE | `/api/cursos/{id}` | Excluir curso |

### Endpoints de Alunos

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/alunos` | Listar todos os alunos |
| GET | `/api/alunos/matriculados` | Listar alunos matriculados |
| GET | `/api/alunos/{id}` | Buscar aluno por ID |
| POST | `/api/alunos` | Criar novo aluno |
| PUT | `/api/alunos/{id}` | Atualizar aluno |
| DELETE | `/api/alunos/{id}` | Excluir aluno |

### Endpoints de Matrículas

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/matriculas/curso/{cursoId}/alunos` | Listar alunos de um curso |
| POST | `/api/matriculas` | Matricular aluno em curso |
| DELETE | `/api/matriculas` | Remover matrícula |

## 🧪 Exemplos de Uso

### Criar Curso

```bash
POST https://localhost:7034/api/cursos
Content-Type: application/json

{
  "nome": "Desenvolvimento Web com ASP.NET Core",
  "descricao": "Curso completo de desenvolvimento web com .NET"
}
```

### Criar Aluno

```bash
POST https://localhost:7034/api/alunos
Content-Type: application/json

{
  "nome": "João Silva",
  "email": "joao.silva@email.com",
  "dataNascimento": "1995-05-15"
}
```

**Observação**: Apenas alunos maiores de 18 anos podem ser cadastrados.

### Matricular Aluno

```bash
POST https://localhost:7034/api/matriculas
Content-Type: application/json

{
  "alunoId": "guid-do-aluno",
  "cursoId": "guid-do-curso"
}
```

### Remover Matrícula

```bash
DELETE https://localhost:7034/api/matriculas
Content-Type: application/json

{
  "alunoId": "guid-do-aluno",
  "cursoId": "guid-do-curso"
}
```

## 🔍 Validações Implementadas

### Curso
- Nome: obrigatório, entre 3 e 200 caracteres
- Descrição: obrigatória, entre 10 e 1000 caracteres

### Aluno
- Nome: obrigatório, entre 3 e 200 caracteres
- Email: obrigatório, formato válido, único no sistema
- Data de Nascimento: obrigatória, **aluno deve ter 18 anos ou mais**

### Matrícula
- Aluno e Curso devem existir
- Aluno não pode estar matriculado duas vezes no mesmo curso

## 🧪 Testes Unitários

O projeto conta com uma suíte completa de testes unitários organizados em duas camadas:

### Estrutura de Testes

```
tests/
└── UnitTests/
    ├── StudentsRegistrationSystem.API.UnitTests/        # Testes de Controllers
    │   └── Controllers/
    │       ├── AlunosControllerTests.cs
    │       ├── CursosControllerTests.cs
    │       └── MatriculasControllerTests.cs
    │
    └── StudentsRegistrationSystem.Application.UnitTests/ # Testes de Handlers
        ├── Alunos/          # Testes de Commands e Queries de Alunos
        ├── Cursos/          # Testes de Commands e Queries de Cursos
        └── Matriculas/      # Testes de Commands de Matrículas
```

### Tecnologias de Teste

- **xUnit**: Framework de testes
- **Moq**: Biblioteca de mocking
- **AwesomeAssertions**: Asserções fluentes e expressivas
- **Coverlet**: Análise de cobertura de código

### Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes de um projeto específico
dotnet test tests/UnitTests/StudentsRegistrationSystem.API.UnitTests
dotnet test tests/UnitTests/StudentsRegistrationSystem.Application.UnitTests

# Executar testes com relatório de cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Executar testes com saída detalhada
dotnet test --verbosity detailed

# Executar testes e exibir apenas resultados
dotnet test --logger:"console;verbosity=minimal"
```

### Cobertura de Testes

Os testes cobrem:
- ✅ **Controllers**: Validação de respostas HTTP e integração com Mediator
- ✅ **Handlers**: Lógica de negócio de Commands e Queries
- ✅ **Validações**: Regras de negócio e validação de dados
- ✅ **Cenários de Sucesso**: Fluxos principais da aplicação
- ✅ **Cenários de Erro**: Tratamento de casos excepcionais

## 🛠️ Tecnologias Utilizadas

- **.NET 8**: Framework principal
- **ASP.NET Core**: Web API
- **Entity Framework Core 8**: ORM
- **SQL Server**: Banco de dados
- **NetDevPack.SimpleMediator**: Implementação do padrão Mediator (CQRS)
- **Swagger/OpenAPI**: Documentação da API
- **xUnit**: Framework de testes unitários
- **Moq**: Biblioteca de mocking
- **AwesomeAssertions**: Biblioteca de asserções

## 📁 Estrutura de Pastas Detalhada

```
StudentsRegistrationSystem/
├── src/
│   ├── StudentsRegistrationSystem.Core/
│   │   ├── Alunos/
│   │   │   └── Domains/           # DTOs e interfaces de Alunos
│   │   ├── Cursos/
│   │   │   └── Domains/           # DTOs e interfaces de Cursos
│   │   ├── Matriculas/
│   │   │   └── Domains/           # DTOs e interfaces de Matrículas
│   │   ├── BaseEntity/            # Entidade base
│   │   └── Shared/                # Result Pattern e utilitários
│   │
│   ├── StudentsRegistrationSystem.Application/
│   │   ├── Alunos/
│   │   │   ├── Commands/          # Commands CQRS para Alunos
│   │   │   └── Queries/           # Queries CQRS para Alunos
│   │   ├── Cursos/
│   │   │   ├── Commands/          # Commands CQRS para Cursos
│   │   │   └── Queries/           # Queries CQRS para Cursos
│   │   └── Matriculas/
│   │       ├── Commands/          # Commands CQRS para Matrículas
│   │       └── Queries/           # Queries CQRS para Matrículas
│   │
│   ├── StudentsRegistrationSystem.Infrastructure/
│   │   ├── Data/
│   │   │   ├── Configurations/    # IEntityTypeConfiguration
│   │   │   ├── Migrations/        # Migrations do EF Core
│   │   │   └── AppDbContext.cs
│   │   ├── Repositories/          # Implementação dos repositórios
│   │   └── Interfaces/            # Interfaces de repositórios
│   │
│   └── StudentsRegistrationSystem.API/
│       ├── Controllers/           # Endpoints REST
│       ├── Extensions/            # Extensões e configurações
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── Program.cs             # Configuração da aplicação
│       └── appsettings.json       # Configurações
│
└── tests/
    └── UnitTests/
        ├── StudentsRegistrationSystem.API.UnitTests/
        │   └── Controllers/       # Testes dos Controllers
        │
        └── StudentsRegistrationSystem.Application.UnitTests/
            ├── Alunos/            # Testes de Alunos
            ├── Cursos/            # Testes de Cursos (note: pasta chamada "Cursors")
            └── Matriculas/        # Testes de Matrículas
```

## 🔧 Comandos Úteis

### Migrations

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration --project src/StudentsRegistrationSystem.Infrastructure --startup-project src/StudentsRegistrationSystem.API

# Aplicar migrations
dotnet ef database update --project src/StudentsRegistrationSystem.Infrastructure --startup-project src/StudentsRegistrationSystem.API

# Reverter última migration
dotnet ef migrations remove --project src/StudentsRegistrationSystem.Infrastructure --startup-project src/StudentsRegistrationSystem.API

# Gerar script SQL
dotnet ef migrations script --project src/StudentsRegistrationSystem.Infrastructure --startup-project src/StudentsRegistrationSystem.API --output migration.sql
```

### Build e Testes

```bash
# Compilar solução
dotnet build

# Compilar em modo Release
dotnet build --configuration Release

# Executar aplicação via CLI
dotnet run --project src/StudentsRegistrationSystem.API

# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test /p:CollectCoverage=true

# Limpar build
dotnet clean
```

## 📝 Observações

- O sistema utiliza **Guids** como identificadores únicos
- As matrículas são **soft deleted** (campo `Ativa` ao invés de exclusão física)
- Todos os endpoints retornam respostas padronizadas com `Result Pattern`
- A API possui validação de dados tanto no nível de **Data Annotations** quanto na **camada de aplicação**
- O projeto possui **testes unitários** para Controllers e Handlers, garantindo a qualidade do código
