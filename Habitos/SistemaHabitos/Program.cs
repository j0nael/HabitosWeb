using FluentValidation;
using FluentValidation.AspNetCore;
using GestorHabitos.Application.Contracts;
using GestorHabitos.Application.Servicios;
using GestorHabitos.Application.Validation;
using GestorHabitos.Infrastructure.Data;
using GestorHabitos.Infrastructure.Interfaces;
using GestorHabitos.Infrastructure.REPOSITORIOS;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddValidatorsFromAssemblyContaining<ValidationUsuario>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidationCategoriaHabito>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidationHabito>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidationRegistro>();
builder.Services.AddValidatorsFromAssemblyContaining<ValidationMeta>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICategoriaHabitoService, CategoriaHabitoService>();
builder.Services.AddScoped<IHabitoService, HabitoService>();
builder.Services.AddScoped<IRegistroService, RegistroService>();
builder.Services.AddScoped<IMetaService, MetaService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositorie>();
builder.Services.AddScoped<ICategoriaHabitoRepository, CategoriaHabitoRepositorie>();
builder.Services.AddScoped<IHabitoRepository, HabitoRepositorie>();
builder.Services.AddScoped<IRegistroRepository, RegistroRepositorie>();
builder.Services.AddScoped<IMetaRepository, MetaRepositorie>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
