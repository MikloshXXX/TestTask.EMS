using TestTask.EMS.Application.Entities;

namespace TestTask.EMS.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<bool> DoesExist(int id);
    Task<Employee> GetById(int id);
    Task UpdateStatus(int id, bool isEnabled);
}
