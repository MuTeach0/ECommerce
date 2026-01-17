using Asp.Versioning;
using ECommerce.API.Contracts.Categories;
using ECommerce.Application.Features.Categories.Commands.CreateCategory;
using ECommerce.Application.Features.Categories.Commands.RemoveCategory;
using ECommerce.Application.Features.Categories.Commands.UpdateCategory;
using ECommerce.Application.Features.Categories.DTOs;
using ECommerce.Application.Features.Categories.Queries.GetCategories;
using ECommerce.Application.Features.Categories.Queries.GetCategoriesById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace ECommerce.API.Controllers;
[Route("api/v{version:apiVersion}/categories")]
[ApiVersion("2.0")]
[Authorize]
public sealed class CategoriesController(ISender sender) : ApiController
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CategoryDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves all categories.")]
    [EndpointDescription("Returns a list of all active categories available in the system.")]
    [EndpointName("GetCategories")]
    [MapToApiVersion("2.0")]
    [OutputCache(Duration = 60)] // Cache for 60 seconds at the response level
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetCategoriesQuery(), ct);

        return result.Match(
            response => Ok(response),
            Problem);
    }

    [HttpGet("{categoryId:guid}", Name = "GetCategoryById")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a category by ID.")]
    [EndpointDescription("Returns detailed information about the specified category if found.")]
    [EndpointName("GetCategoryById")]
    [MapToApiVersion("2.0")]
    [OutputCache(Duration = 60)]
    public async Task<IActionResult> GetById(Guid categoryId, CancellationToken ct)
    {
        // محتاج تعمل Query اسمها GetCategoryByIdQuery
        var result = await sender.Send(new GetCategoryByIdQuery(categoryId), ct);
        return result.Match(Ok, Problem);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Creates a new category.")]
    [EndpointDescription("Allows admins to create new category categories.")]
    [EndpointName("CreateCategory")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        return result.Match(
            id => CreatedAtAction(nameof(Get), new { version = "2.0" }, id),
            Problem);
    }

    [HttpPut("{categoryId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates an existing category.")]
    [EndpointName("Updatecategory")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> Update(Guid categoryId, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        var command = new UpdateCategoryCommand(
            categoryId,
            request.Name,
            request.Description,
            request.ImageUrl,
            request.ParentCategoryId);

        var result = await sender.Send(command, ct);
        return result.Match(_ => NoContent(), Problem);
    }



    [HttpDelete("{categoryId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Removes a Category.")]
    [EndpointName("RemoveCategory")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> Delete(Guid categoryId, CancellationToken ct)
    {
        var result = await sender.Send(new RemoveCategoryCommand(categoryId), ct);
        return result.Match(_ => NoContent(), Problem);
    }
}