using APItoPFinal.Helpers;
using APItoPFinal.Models;
using APItoPFinal.Settings;
using Microsoft.EntityFrameworkCore;

namespace APItoPFinal.Data
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AdminSeeder");

            try
            {
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var settings = configuration.GetSection("AdminSeed").Get<AdminSeedSettings>();

                // seção não configurada: nada a fazer
                if (settings == null
                    || string.IsNullOrWhiteSpace(settings.CPF)
                    || string.IsNullOrWhiteSpace(settings.Senha))
                {
                    return;
                }

                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // já existe pelo menos um Admin
                if (await context.Usuarios.AnyAsync(u => u.Cargo == Cargos.Admin))
                {
                    return;
                }

                var cpf = Cpf.Normalizar(settings.CPF);
                if (!Cpf.EhValido(cpf))
                {
                    logger.LogWarning("AdminSeed ignorado: o CPF configurado é inválido.");
                    return;
                }

                if (settings.Senha.Length < 8 || settings.Senha.Length > 72)
                {
                    logger.LogWarning("AdminSeed ignorado: a senha deve ter entre 8 e 72 caracteres.");
                    return;
                }

                // já existe um usuário com esse CPF: apenas promove, sem mexer na senha dele
                var existente = await context.Usuarios.FirstOrDefaultAsync(u => u.CPF == cpf);
                if (existente != null)
                {
                    existente.Cargo = Cargos.Admin;
                    await context.SaveChangesAsync();
                    logger.LogInformation("Usuário existente com o CPF configurado foi promovido a Admin.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(settings.Email))
                {
                    logger.LogWarning("AdminSeed ignorado: o e-mail não foi configurado.");
                    return;
                }

                var email = settings.Email.Trim().ToLowerInvariant();
                var nomeUsuario = settings.NomeUsuario.Trim();

                if (await context.Usuarios.AnyAsync(u => u.Email == email || u.NomeUsuario == nomeUsuario))
                {
                    logger.LogWarning("AdminSeed ignorado: e-mail ou nome de usuário já em uso por outro usuário.");
                    return;
                }

                context.Usuarios.Add(new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nome = settings.Nome.Trim(),
                    NomeUsuario = nomeUsuario,
                    Email = email,
                    CPF = cpf,
                    Senha = BCrypt.Net.BCrypt.HashPassword(settings.Senha),
                    Cargo = Cargos.Admin
                });

                await context.SaveChangesAsync();
                logger.LogInformation("Admin inicial criado (CPF {Cpf}).", cpf);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Não foi possível verificar ou criar o Admin inicial. O banco está atualizado? Rode 'dotnet ef database update'.");
            }
        }
    }
}