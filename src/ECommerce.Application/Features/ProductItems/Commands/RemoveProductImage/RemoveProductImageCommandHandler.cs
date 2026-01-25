using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.ProductItems.Commands.RemoveProductImage;
public class RemoveProductImageCommandHandler(
    IAppDbContext context,
    IImageService imageService)
    : IRequestHandler<RemoveProductImageCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(RemoveProductImageCommand request, CancellationToken ct)
    {
        var product = await context.ProductItems
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

        if (product is null)
            return Error.NotFound("Product.NotFound", "Product not found.");

        var image = product.Images.FirstOrDefault(i => i.Id == request.ImageId);
        if (image is null)
            return Error.NotFound("Image.NotFound", "Image not found on this product.");

        var deletionResult = await imageService.DeleteImageAsync(image.PublicId);

        if (deletionResult == "error")
        {
            return Error.Failure("Image.CloudDeleteFailed", "Failed to delete image from Cloudinary.");
        }

        product.RemoveImage(request.ImageId);

        await context.SaveChangesAsync(ct);

        return Result.Deleted;
    }
}