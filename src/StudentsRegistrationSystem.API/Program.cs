using Microsoft.EntityFrameworkCore;
using StudentsRegistrationSystem.API.Extensions;
using StudentsRegistrationSystem.API.Middlewares;
using StudentsRegistrationSystem.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);

builder.Services.AddHandlers();
builder.Services.AddRepositories();

builder.Services.AddSwagger();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseCors(c =>
{
    c.WithOrigins("http://localhost:5173");
    c.WithHeaders("Content-Type");
    c.WithMethods("GET", "POST", "PUT", "DELETE");
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();