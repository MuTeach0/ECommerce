using ECommerce.Application.Features.Dashboard.DTOs;
using ECommerce.Domain.Common.Results;
using MediatR;

namespace ECommerce.Application.Features.Dashboard.Queries.GetOrderStats;

public sealed record GetOrderStatsQuery(DateOnly Date) : IRequest<Result<TodayOrderStatsDTO>>;