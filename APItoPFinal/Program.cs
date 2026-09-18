using APItoPFinal.Data;
using APItoPFinal.Repository;
using APItoPFinal.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var chave = new SymmetricSecurityKey(RandomNumberGenerator.GetBytes(32));

builder.Services.AddSingleton(chave);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "ExemploJwt",
        ValidateAudience = true,
        ValidAudience = "Alunos",
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = chave,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256}
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registra o Repository e a Service dos Instrumentos
builder.Services.AddScoped<InstrumentosRepository>();
builder.Services.AddScoped<InstrumentoService>();

// Registra o Repository e a Service dos Compras
builder.Services.AddScoped<CompraRepository>();
builder.Services.AddScoped<CompraService>();

// Registre o Repository e a Service de Categorias
builder.Services.AddScoped<CategoriasRepository>();
builder.Services.AddScoped<CategoriaService>();

// Registra o Repository e a Service das Marcas
builder.Services.AddScoped<MarcaRepository>();
builder.Services.AddScoped<MarcaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
