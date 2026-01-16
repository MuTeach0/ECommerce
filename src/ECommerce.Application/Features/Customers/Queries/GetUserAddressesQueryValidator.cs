using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Features.Customers.Queries;

public class GetUserAddressesQueryValidator : AbstractValidator<GetUserAddressesQuery>
{
    public GetUserAddressesQueryValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required to fetch addresses.");
    }
}