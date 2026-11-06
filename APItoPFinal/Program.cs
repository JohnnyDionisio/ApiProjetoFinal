using APItoPFinal.Data;
using APItoPFinal.Documentation;
using APItoPFinal.Exceptions;
using APItoPFinal.Repository;
using APItoPFinal.Service;
using APItoPFinal.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

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

builder.Services.AddScoped<UsuariosRepository>();
builder.Services.AddScoped<UsuariosService>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();

if (string.IsNullOrWhiteSpace(jwt.Key) || Encoding.UTF8.GetBytes(jwt.Key).Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key ausente ou curta demais (mínimo 32 caracteres). " +
        "Configure com: dotnet user-secrets set \"Jwt:Key\" \"<sua-chave>\"");
}

var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

builder.Services.AddSingleton(jwt);
builder.Services.AddSingleton(chave);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwt.Issuer,
        ValidateAudience = true,
        ValidAudience = jwt.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = chave,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"[JWT] Falha na validação: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();