using TestTask.EMS.Api.Filters;
using TestTask.EMS.Api.Mappers;
using TestTask.EMS.Application;
using TestTask.EMS.Application.Interfaces;

namespace TestTask.EMS.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;

        builder.Services.AddProblemDetails();

        builder.Services.AddInfrastructure(configuration);

        var app = builder.Build();

        app.UseStatusCodePages();

        var employees = app
            .MapGroup("/api/employees/{id}");

        employees.MapGet("/", async (int id, IEmployeeRepository repository) =>
        {
            var employee = await repository.GetById(id);

            return Results.Ok(employee.ToView());
        })
        .AddEndpointFilter<GetEmployeeEndpointFilter>(); ;

        employees.MapPost("/update-status", async (int id, bool isEnabled, IEmployeeRepository repository) =>
        {
            await repository.UpdateStatus(id, isEnabled);

            return Results.Ok();
        });

        app.Run();
    }
}
