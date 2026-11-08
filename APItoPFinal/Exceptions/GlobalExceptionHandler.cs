using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace APItoPFinal.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetails;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandler(
            IProblemDetailsService problemDetails,
            ILogger<GlobalExceptionHandler> logger,
            IHostEnvironment env)
        {
            _problemDetails = problemDetails;
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, title, detail) = exception switch
            {
                DbUpdateConcurrencyException =>
                     (StatusCodes.Status409Conflict, "Conflito de concorrência",
                       "Os dados foram alterados por outra operação ao mesmo tempo. Atualize e tente novamente."),
                NotFoundException or KeyNotFoundException =>
                    (StatusCodes.Status404NotFound, "Recurso não encontrado", exception.Message),
                ConflictException =>
                    (StatusCodes.Status409Conflict, "Conflito", exception.Message),
                BadRequestException =>
                    (StatusCodes.Status400BadRequest, "Requisição inválida", exception.Message),
                UnauthorizedAccessException =>
                    (StatusCodes.Status403Forbidden, "Acesso negado", exception.Message),
                _ =>
                    (StatusCodes.Status500InternalServerError, "Erro interno do servidor",
                     _env.IsDevelopment()
                        ? exception.Message
                        : "Ocorreu um erro inesperado. Informe o código de rastreio (traceId) ao suporte.")
            };

            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            if(status == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Erro não tratado. TraceId: {TraceId}", traceId);
            }

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };
            problem.Extensions["TraceId"] = traceId;

            httpContext.Response.StatusCode = status;

            return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem
            });

        }
    }
}
