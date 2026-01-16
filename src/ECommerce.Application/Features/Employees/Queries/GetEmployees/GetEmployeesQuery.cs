using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Employees.DTOs;
using ECommerce.Domain.Common.Results;

namespace ECommerce.Application.Features.Employees.Queries.GetEmployees;

public sealed record GetEmployeesQuery() : ICachedQuery<Result<List<EmployeeDTO>>>
{
    public string CacheKey => $"employees";
    public string[] Tags => ["employees"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}