using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Features.Reviews.DTOs;

public sealed record ProductReviewDTO(
    Guid Id,
    int Stars,
    string Comment,
    DateTimeOffset CreatedAtUtc,
    string CustomerName);