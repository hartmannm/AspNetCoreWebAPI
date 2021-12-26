using DevIO.Business.Interfaces;
using DevIO.Business.Interfaces.Repositories;
using DevIO.Business.Interfaces.Services;
using DevIO.Business.Notificacoes;
using DevIO.Business.Services;
using DevIO.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            // Repositórios
            services.AddScoped<IFornecedorRepository, FornecedorRepository>();
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
            // Serviços de negócio
            services.AddScoped<IFornecedorService, FornecedorService>();
            // Demais serviços e classes
            services.AddScoped<INotificador, Notificador>();
            return services;
        }
    }
}
