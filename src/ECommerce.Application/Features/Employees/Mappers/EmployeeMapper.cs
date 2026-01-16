using ECommerce.Application.Features.Employees.DTOs;
using ECommerce.Domain.Employees;

namespace ECommerce.Application.Features.Employees.Mappers;

public static class EmployeeMapper
{
    public static EmployeeDTO ToDTO(this Employee employee)
    {
        return new EmployeeDTO { EmployeeId = employee.Id, Name = employee.FullName };
    }

    public static List<EmployeeDTO> ToDTOs(this IEnumerable<Employee> entities)
    {
        return [.. entities.Select(e => e.ToDTO())];
    }
}