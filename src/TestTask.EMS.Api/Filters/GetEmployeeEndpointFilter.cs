using TestTask.EMS.Application.Interfaces;

namespace TestTask.EMS.Api.Filters;

public class GetEmployeeEndpointFilter : IEndpointFilter
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeEndpointFilter(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var employeeId = context.GetArgument<int>(0);

        var doesExist = await _employeeRepository.DoesExist(employeeId);

        if (doesExist)
        {
            return await next(context);
        }

        return Results.NotFound();
    }
}
