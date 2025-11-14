using NetDevPack.SimpleMediator;
using StudentsRegistrationSystem.Application.Alunos.Commands;
using StudentsRegistrationSystem.Application.Alunos.Commands.Handlers;
using StudentsRegistrationSystem.Application.Alunos.Queries;
using StudentsRegistrationSystem.Application.Alunos.Queries.Handlers;
using StudentsRegistrationSystem.Application.Cursos.Commands;
using StudentsRegistrationSystem.Application.Cursos.Commands.Handlers;
using StudentsRegistrationSystem.Application.Cursos.Queries;
using StudentsRegistrationSystem.Application.Cursos.Queries.Handlers;
using StudentsRegistrationSystem.Application.Matriculas.Commands;
using StudentsRegistrationSystem.Application.Matriculas.Commands.Handlers;
using StudentsRegistrationSystem.Application.Matriculas.Queries;
using StudentsRegistrationSystem.Application.Matriculas.Queries.Handlers;
using StudentsRegistrationSystem.Core.Alunos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Cursos.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Matriculas.Domains.DTOs.Responses;
using StudentsRegistrationSystem.Core.Shared;
using StudentsRegistrationSystem.Infrastructure.Interfaces.Repositories;
using StudentsRegistrationSystem.Infrastructure.Repositories;

namespace StudentsRegistrationSystem.API.Extensions;

public static class IocExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();

        // Alunos - Commands
        services.AddScoped<IRequestHandler<CreateAlunoCommand, Result<AlunoResponse>>, CreateAlunoHandler>();
        services.AddScoped<IRequestHandler<UpdateAlunoCommand, Result<AlunoResponse>>, UpdateAlunoHandler>();
        services.AddScoped<IRequestHandler<DeleteAlunoCommand, Result<bool>>, DeleteAlunoHandler>();

        // Alunos - Queries
        services.AddScoped<IRequestHandler<GetAlunoByIdQuery, Result<AlunoResponse>>, GetAlunoByIdHandler>();
        services.AddScoped<IRequestHandler<GetAllAlunosQuery, Result<IEnumerable<AlunoResponse>>>, GetAllAlunosHandler>();
        services.AddScoped<IRequestHandler<GetAlunosMatriculadosQuery, Result<IEnumerable<AlunoResponse>>>, GetAlunosMatriculadosHandler>();

        // Cursos - Commands
        services.AddScoped<IRequestHandler<CreateCursoCommand, Result<CursoResponse>>, CreateCursoHandler>();
        services.AddScoped<IRequestHandler<UpdateCursoCommand, Result<CursoResponse>>, UpdateCursoHandler>();
        services.AddScoped<IRequestHandler<DeleteCursoCommand, Result<bool>>, DeleteCursoHandler>();

        // Cursos - Queries
        services.AddScoped<IRequestHandler<GetCursoByIdQuery, Result<CursoResponse>>, GetCursoByIdHandler>();
        services.AddScoped<IRequestHandler<GetAllCursosQuery, Result<IEnumerable<CursoResponse>>>, GetAllCursosHandler>();

        // Matriculas - Commands
        services.AddScoped<IRequestHandler<CreateMatriculaCommand, Result<MatriculaResponse>>, CreateMatriculaHandler>();
        services.AddScoped<IRequestHandler<RemoveMatriculaCommand, Result<bool>>, RemoveMatriculaHandler>();

        // Matriculas - Queries
        services.AddScoped<IRequestHandler<GetAlunosByCursoQuery, Result<IEnumerable<AlunoResponse>>>, GetAlunosByCursoHandler>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAlunoRepository, AlunoRepository>();
        services.AddScoped<ICursoRepository, CursoRepository>();
        services.AddScoped<IMatriculaRepository, MatriculaRepository>();

        return services;
    }
}
