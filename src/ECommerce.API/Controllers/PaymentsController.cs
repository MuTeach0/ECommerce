using Asp.Versioning;
using ECommerce.API.Contracts.Payments;
using ECommerce.Application.Features.Payments.Commands.CapturePayment;
using ECommerce.Application.Features.Payments.Commands.CreatePayment;
using ECommerce.Application.Features.Payments.DTOs;
using ECommerce.Application.Features.Payments.Queries.GetOrderPayment;
using ECommerce.Application.Features.Payments.Queries.GetPayment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Route("api/v{version:apiVersion}/payments")]
[ApiVersion("2.0")]
[Authorize]
public sealed class PaymentsController(ISender sender) : ApiController
{
    [HttpPost("paypal/create")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Creates a PayPal order and returns the PayPal Order ID.")]
    [EndpointDescription("Initializes the payment process with PayPal and saves a pending payment record.")]
    [EndpointName("CreatePayPalOrder")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new CreatePaymentCommand(request.OrderId), ct);
        return result.Match(
            payPalOrderId => Ok(new { Id = payPalOrderId }), 
            Problem);
    }

    [HttpPost("paypal/capture/{payPalOrderId}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Captures the funds from an approved PayPal order.")]
    [EndpointDescription("Finalizes the transaction after user approval and updates order status.")]
    [EndpointName("CapturePayPalOrder")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> Capture(string payPalOrderId, CancellationToken ct)
    {
        var result = await sender.Send(new CapturePaymentCommand(payPalOrderId), ct);
       return result.Match(
        success => Ok(new { Success = success, Message = "Payment captured successfully" }), 
        Problem);
    }

    [HttpGet("{paymentId:guid}")]
    [ProducesResponseType(typeof(PaymentDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves detailed information about a specific payment.")]
    [EndpointName("GetPaymentById")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid paymentId, CancellationToken ct)
    {
        var result = await sender.Send(new GetPaymentQuery(paymentId), ct);
        return result.Match(dto => Ok(MapToResponse(dto)), Problem);
    }

    [HttpGet("order/{orderId:guid}")]
    [ProducesResponseType(typeof(PaymentDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves the payment details associated with a specific order.")]
    [EndpointName("GetPaymentByOrderId")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken ct)
    {
        var result = await sender.Send(new GetOrderPaymentQuery(orderId), ct);
        return result.Match(dto => Ok(MapToResponse(dto)), Problem);
    }
    
    private static PaymentResponse MapToResponse(PaymentDTO dto)
    {
        return new PaymentResponse(
            dto.Id,
            dto.OrderId,
            dto.TransactionId,
            dto.Amount,
            dto.Currency,
            dto.Status,
            dto.Provider,
            dto.CreatedAtUtc);
    }
}