using Elmah.Io.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services)
        {
            // Habilita o Elmah como provider do pipeline
            services.AddElmahIo(s =>
            {
                // Para usar o Elmah é necessário colocar as chaves corretas nesse ponto
                s.ApiKey = "inserir elmah key aqui";
                s.LogId = new Guid("859d9481-8aff-4f29-9ee8-2a2a92127aef");
            });
            // Habilita o Elmah como provider dos logs
            services.AddLogging(builder =>
            {
                builder.AddElmahIo(s =>
                {
                    // Para usar o Elmah é necessário colocar as chaves corretas nesse ponto
                    s.ApiKey = "inserir elmah key aqui";
                    s.LogId = new Guid("859d9481-8aff-4f29-9ee8-2a2a92127aef");
                });
                builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);
            });
            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();
            return app;
        }
    }
}
