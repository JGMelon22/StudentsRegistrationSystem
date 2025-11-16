# Sistema de Gerenciamento de Matrículas

API RESTful desenvolvida em .NET 8 para gerenciamento de cursos, alunos e matrículas.

## 🏗️ Arquitetura

O projeto segue os princípios de Domain-Driven Design (DDD) e Clean Architecture, organizado em 4 camadas:

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

## Padrões Utilizados

- **DDD (Domain-Driven Design)**: Organização em camadas com foco no domínio
- **CQRS**: Separação de Commands (escrita) e Queries (leitura)
- **Result Pattern**: Gerenciamento de erros sem exceptions
- **Repository Pattern**: Abstração do acesso a dados
- **Mediator Pattern**: Desacoplamento com NetDevPack.Mediator

## 🚀 Como Rodar o Projeto

### Opção 1: Execução com Docker (Recomendado) 🐳

Esta é a forma mais simples e rápida de executar a aplicação, pois não requer instalação manual do SQL Server ou do .NET SDK.

#### Pré-requisitos

- Docker Desktop ou Docker Engine
- Docker Compose

#### Configurações Padrão

As migrations do banco de dados são aplicadas automaticamente na inicialização do container da API.

**SQL Server:**
- Porta: 1433 (mapeada para a porta interna)
- Usuário: sa
- Senha: Melon@123 (configurada em docker-compose.yml)
- Database: StudentsRegistrationSystemDb

**API:**
- Portas HTTP/HTTPS: 8080 e 8081 (mapeadas localmente)

#### Passos para Execução

**Clonar o Repositório**

```bash
git clone <url-do-repositorio>
cd StudentsRegistrationSystem
```

**Executar com Docker Compose**

```bash
docker-compose up -d
```

Este comando irá criar e iniciar o container do SQL Server e o container da API.

**Verificar Status**

```bash
docker-compose ps
```

**Acessar a API**

- HTTP: http://localhost:8080
- HTTPS: https://localhost:8081
- Swagger: https://localhost:8081/swagger

#### Comandos de Gerenciamento

| Comando | Descrição |
|---------|-----------|
| `docker-compose logs -f` | Visualizar logs de todos os serviços em tempo real |
| `docker-compose down` | Parar e remover containers e redes |
| `docker-compose down -v` | Parar, remover containers e limpar volumes (dados do banco) |

#### Troubleshooting

- **Erro de porta já em uso**: Altere o mapeamento de portas no docker-compose.yml para uma porta livre (ex: "1434:1433").
- **Container da API não inicia**: Verifique os logs com `docker-compose logs studentsregistrationsystem.api`. A falha pode estar na configuração da Connection String ou no container do SQL Server.

### Opção 2: Execução Local (sem Docker)

Se preferir executar localmente, siga os passos abaixo.

#### 1. Pré-requisitos

- .NET 8 SDK
- SQL Server (LocalDB, Express ou versão completa)

#### 2. Configurar Connection String

Edite o arquivo `src/StudentsRegistrationSystem.API/appsettings.json` e ajuste a connection string para o seu servidor local.

**Exemplo (SQL Server LocalDB):**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentsRegistrationSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
  }
}
```

#### 3. Execução

**Clonar e restaurar pacotes:**

```bash
git clone <url-do-repositorio>
cd StudentsRegistrationSystem
dotnet restore
```

**Aplicar Migrations:**

```bash
# Instalar ferramenta EF Core (se necessário)
dotnet tool install --global dotnet-ef

# Aplicar migrations
dotnet ef database update --project src/StudentsRegistrationSystem.Infrastructure --startup-project src/StudentsRegistrationSystem.API
```

**Executar a API:**

```bash
cd src/StudentsRegistrationSystem.API
dotnet run
```

A API estará disponível em: https://localhost:7034 (Swagger: /swagger).

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

> **Nota:** Use `https://localhost:8081` se estiver usando Docker ou `https://localhost:7034` para execução Local.

### Criar Curso

```http
POST https://localhost:7034/api/cursos
Content-Type: application/json

{
  "nome": "Desenvolvimento Web com ASP.NET Core",
  "descricao": "Curso completo de desenvolvimento web com .NET"
}
```

### Criar Aluno

```http
POST https://localhost:7034/api/alunos
Content-Type: application/json

{
  "nome": "João Silva",
  "email": "joao.silva@email.com",
  "dataNascimento": "1995-05-15"
}
```

> **Observação:** Apenas alunos maiores de 18 anos podem ser cadastrados.

### Matricular Aluno

```http
POST https://localhost:7034/api/matriculas
Content-Type: application/json

{
  "alunoId": "guid-do-aluno",
  "cursoId": "guid-do-curso"
}
```

### Remover Matrícula

```http
DELETE https://localhost:7034/api/matriculas
Content-Type: application/json

{
  "alunoId": "guid-do-aluno",
  "cursoId": "guid-do-curso"
}
```

## 🧪 Testes Unitários

### Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes com relatório de cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

O projeto utiliza xUnit e Moq.

## 🔧 Comandos Úteis para Desenvolvimento Local

### Migrations

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration --project src/StudentsRegistrationSystem.Infrastructure --startup-project src/StudentsRegistrationSystem.API

# Aplicar migrations
dotnet ef database update --project src/StudentsRegistrationSystem.Infrastructure --startup-project src/StudentsRegistrationSystem.API
```

### Build e Execução

```bash
# Compilar solução
dotnet build

# Executar aplicação via CLI
dotnet run --project src/StudentsRegistrationSystem.API
```