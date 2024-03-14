namespace TestTask.EMS.Api.Views;

public record EmployeeView
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int? ManagerId { get; init; }
    public List<EmployeeView> Employees { get; init; }
}
