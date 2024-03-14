namespace TestTask.EMS.Application.Entities;

public class Employee
{
    public Employee(
        int id,
        string name,
        int? managerId,
        bool isEnabled,
        List<Employee> subordinates = null)
    {
        Id = id;
        Name = name;
        ManagerId = managerId;
        IsEnabled = isEnabled;
        Subordinates = subordinates ?? new();
    }

    public int Id { get; }
    public string Name { get; }
    public int? ManagerId { get; }
    public bool IsEnabled { get; }
    public List<Employee> Subordinates { get; }
}
