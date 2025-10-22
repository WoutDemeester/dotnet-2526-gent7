using Microsoft.Extensions.DependencyInjection;
using Rise.Persistence;
using Rise.Services.Deadlines;
using Rise.Services.Departments;
using Rise.Services.Resto;
using Rise.Shared.Deadlines;

//using Rise.Services.Projects;
using Rise.Shared.Departments;
using Rise.Shared.Resto;

namespace Rise.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDepartmentService, DepartmentService>();        
        services.AddScoped<IRestoService, RestoService>();    
        services.AddScoped<IDeadlineService, DeadlineService>(); 
        services.AddTransient<DbSeeder>();       
        
        // Add other application services here.
        return services;
    }
}