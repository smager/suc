using SmagerUp.Core.API.Data.Client;
using SmagerUp.Core.API.Data.Core;
using SmagerUp.Core.API.Services;
namespace SmagerUp.Core.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSmagerUpCore( this IServiceCollection services)
        {

            //data
            services.AddScoped<IClientDbResolver,ClientDbResolver>();
            services.AddScoped<HostRepository>();
            services.AddScoped<SmagerUp.Core.API.Data.Client.ClientRepository>();
            services.AddScoped<SmagerUp.Core.API.Data.Core.ClientRepository>();
            services.AddScoped<LicenseTypeRepository>();
            services.AddScoped<ComponentRepository>();
            services.AddScoped<ResourceRepository>();
            services.AddScoped<UserRepository>();

            //security
            services.AddScoped<TokenService>();
            services.AddDataProtection();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IPasswordService, PasswordService>();

            return services;
        }
    }
}
