using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestTask.EMS.Application.Infrastructure.Repositories;
using TestTask.EMS.Application.Interfaces;
using TestTask.EMS.Application.Options;

namespace TestTask.EMS.Application;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStringSection = configuration.GetSection("ConnectionStrings");

        services.Configure<ConnectionString>(connectionStringSection);
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
    }
}
