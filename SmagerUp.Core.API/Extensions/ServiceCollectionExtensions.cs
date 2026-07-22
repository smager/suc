using Microsoft.AspNetCore.DataProtection;
using SmagerUp.Core.API.Data;
using SmagerUp.Core.API.Data.Admin;
using SmagerUp.Core.API.Data.Client;
using SmagerUp.Core.API.Services;

namespace SmagerUp.Core.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmagerUpCore(this IServiceCollection services, IHostEnvironment environment)
    {

        //data

        services.AddScoped<AdminDbContext>(); // Register AdminDbContext as scoped
        services.AddScoped<IClientDbResolver, ClientDbContext>();
        services.AddScoped<ClientRepository>();
        services.AddScoped<AdminRepository>();

        services.AddScoped<IClientDataRepository, ClientDataRepository>();
        services.AddScoped<ICoreDataRepository, AdminDataRepository>();

        //security
        services.AddSingleton<TokenService>();

        

        services.AddSingleton<IEncryptionService, EncryptionService>();  

        var keysPath = Path.Combine(environment.ContentRootPath, "Keys");

        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
            .SetApplicationName("SmagerUpCore");

        //services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<IPasswordService, PasswordService>();

        //others
        services.AddScoped<SettingsRepository>();
        services.AddScoped<EmailSettingsRepository>();

        return services;
    }
}
