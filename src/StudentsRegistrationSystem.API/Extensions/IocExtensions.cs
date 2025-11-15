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
        services.AddScoped<IRequestHandler<CreateAlunoCommand, Result<AlunoResponse>>, CreateAlunoCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateAlunoCommand, Result<AlunoResponse>>, UpdateAlunoCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteAlunoCommand, Result<bool>>, DeleteAlunoCommandHandler>();

        // Alunos - Queries
        services.AddScoped<IRequestHandler<GetAlunoByIdQuery, Result<AlunoResponse>>, GetAlunoByIdQueryHandler>();
        services.AddScoped<IRequestHandler<GetAllAlunosQuery, Result<PagedResponseOffset<AlunoResponse>>>, GetAllAlunosQueryHandler>();
        services.AddScoped<IRequestHandler<GetAlunosMatriculadosQuery, Result<PagedResponseOffset<AlunoResponse>>>, GetAlunosMatriculadosQueryHandler>();

        // Cursos - Commands
        services.AddScoped<IRequestHandler<CreateCursoCommand, Result<CursoResponse>>, CreateCursoCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateCursoCommand, Result<CursoResponse>>, UpdateCursoCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteCursoCommand, Result<bool>>, DeleteCursoCommandHandler>();

        // Cursos - Queries
        services.AddScoped<IRequestHandler<GetCursoByIdQuery, Result<CursoResponse>>, GetCursoByIdQueryHandler>();
        services.AddScoped<IRequestHandler<GetAllCursosQuery, Result<PagedResponseOffset<CursoResponse>>>, GetAllCursosQueryHandler>();

        // Matriculas - Commands
        services.AddScoped<IRequestHandler<CreateMatriculaCommand, Result<MatriculaResponse>>, CreateMatriculaCommandHandler>();
        services.AddScoped<IRequestHandler<RemoveMatriculaCommand, Result<bool>>, RemoveMatriculaCommandHandler>();

        // Matriculas - Queries
        services.AddScoped<IRequestHandler<GetAlunosByCursoQuery, Result<PagedResponseOffset<AlunoResponse>>>, GetAlunosByCursoQueryHandler>();

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
