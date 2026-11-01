using GroceryList.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryList.Application;

public static class DependencyInjection
{
    /// <summary>Registers the application services. Called from Program.cs.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<GroceryItemService>();
        return services;
    }
}
