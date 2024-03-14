using TestTask.EMS.Api.Views;
using TestTask.EMS.Application.Entities;

namespace TestTask.EMS.Api.Mappers;

public static class EmployeeMapper
{
    public static EmployeeView ToView(this Employee employee)
    {
        return new EmployeeView
        {
            Id = employee.Id,
            Name = employee.Name,
            ManagerId = employee.ManagerId,
            Employees = employee.Subordinates.Select(ToView).ToList()
        };
    }
}
