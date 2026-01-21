using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Features.Customers.DTOs;

public sealed record AddressDTO(
    Guid Id,
    string Title,
    string City,
    string Street,
    string FullAddress);