using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Employees.DTOs;
using ECommerce.Application.Features.Employees.Mappers;
using ECommerce.Domain.Common.Results;
using ECommerce.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Employees.Queries.GetEmployees;

public class GetEmployeesQueryHandler(IAppDbContext context)
    : IRequestHandler<GetEmployeesQuery, Result<List<EmployeeDTO>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<List<EmployeeDTO>>> Handle(GetEmployeesQuery query, CancellationToken ct)
    {
        var Employees = await _context.Employees.AsNoTracking().Where(e => e.Role == Role.Seller).ToListAsync(ct);

        return Employees.ToDTOs();
    }
}