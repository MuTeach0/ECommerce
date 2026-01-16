using ECommerce.Domain.Common.Results;

namespace ECommerce.Domain.Employees;

public static class EmployeeErrors
{
    public static readonly Error IdRequired = Error.Validation(
        code: "Employee.Id.Required",
        description: "Employee ID is required.");
    public static readonly Error FirstNameRequired = Error.Validation(
        code: "Employee.FirstName.Required",
        description: "First name is required.");

    public static readonly Error LastNameRequired = Error.Validation(
        code: "Employee.LastName.Required",
        description: "Last name is required.");

    public static readonly Error RoleInvalid = Error.Validation(
        code: "Employee.Role.Invalid",
        description: "Role is invalid.");
}