using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using TestTask.EMS.Application.Entities;
using TestTask.EMS.Application.Interfaces;
using TestTask.EMS.Application.Options;

namespace TestTask.EMS.Application.Infrastructure.Repositories;

internal class EmployeeRepository : IEmployeeRepository
{
    private const string _doesEmployeeExistQuery = @"
        SELECT CASE WHEN EXISTS(
            SELECT 1
            FROM Employee AS e
            WHERE e.ID = @EmployeeId AND Enabled = 1
        ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END
    ";

    private const string _getEmployeeByIdQuery = @"
        WITH employee_hierarchy (ID, Name, ManagerID, Enabled) AS (
	        SELECT ID, Name, ManagerID, Enabled
	        FROM Employee
	        WHERE ID = @EmployeeId AND Enabled = 1

	        UNION ALL

	        SELECT e.ID, e.Name, e.ManagerID, e.Enabled
	        FROM Employee AS e
	        INNER JOIN employee_hierarchy AS eh
	        ON eh.ID = e.ManagerID
        )
        SELECT * FROM employee_hierarchy
    ";

    private const string _updateEmployeeStatusCommand = @"
        UPDATE Employee
        SET Enabled = @IsEnabled
        WHERE ID = @EmployeeId
    ";

    private readonly ConnectionString _connectionString;

    public EmployeeRepository(IOptions<ConnectionString> connectionString)
    {
        _connectionString = connectionString?.Value ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<bool> DoesExist(int id)
    {
        using var connection = new SqlConnection(_connectionString.SqlServer);
        await connection.OpenAsync();

        var command = new SqlCommand(_doesEmployeeExistQuery, connection);
        command.Parameters.AddWithValue("@EmployeeId", id);

        var doesExist = await command.ExecuteScalarAsync();

        return Convert.ToBoolean(doesExist);
    }

    public async Task<Employee> GetById(int id)
    {
        using var connection = new SqlConnection(_connectionString.SqlServer);
        await connection.OpenAsync();

        var command = new SqlCommand(_getEmployeeByIdQuery, connection);
        command.Parameters.AddWithValue("@EmployeeId", id);

        using var reader = await command.ExecuteReaderAsync();

        var employeeMap = new Dictionary<int, Employee>();
        var managerMap = new Dictionary<int, int>();

        while (await reader.ReadAsync())
        {
            var employeeId = reader.GetInt32(0);
            var name = reader.GetString(1);
            var managerIdSql = reader.GetSqlInt32(2);
            var managerId = managerIdSql.IsNull ? (int?) null : managerIdSql.Value;
            var isEnabled = reader.GetBoolean(3);

			// In case, if direct manager of the employee is disabled
   			// we get the first enabled higher-level manager for this employee
            var implicitManager = managerMap.ContainsKey(managerId ?? default)
                ? managerMap[managerId.Value]
                : managerId;

            var employee = new Employee(employeeId, name, implicitManager, isEnabled);

            if (employee.IsEnabled)
            {
                employeeMap.Add(employeeId, employee);
            }
            else
            {
                var managerIdFunc = managerMap.ContainsKey(managerId.Value)
                    ? managerMap[managerId.Value]
                    : managerId.Value;

                managerMap.Add(employeeId, managerIdFunc);
                continue;
            }

            if (implicitManager is not null && employeeId != id)
            {
                employeeMap[implicitManager.Value].Subordinates.Add(employee);
            }
        }

        return employeeMap[id];
    }

    public async Task UpdateStatus(int id, bool isEnabled)
    {
        using var connection = new SqlConnection(_connectionString.SqlServer);
        await connection.OpenAsync();

        var command = new SqlCommand(_updateEmployeeStatusCommand, connection);
        command.Parameters.AddWithValue("@EmployeeId", id);
        command.Parameters.AddWithValue("@IsEnabled", isEnabled);

        await command.ExecuteNonQueryAsync();
    }
}
