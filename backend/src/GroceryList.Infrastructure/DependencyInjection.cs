using GroceryList.Application.Interfaces;
using GroceryList.Infrastructure.Persistence;
using GroceryList.Infrastructure.Repositories;
using GroceryList.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryList.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the implementations of the Application interfaces. Called from Program.cs.
    /// To swap the database (e.g. for MongoDB), only this layer needs to change.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("GroceryDb")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGroceryItemRepository, GroceryItemRepository>();

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("The 'Jwt' section is missing in appsettings.json.");

        services.AddSingleton(jwtSettings);
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
