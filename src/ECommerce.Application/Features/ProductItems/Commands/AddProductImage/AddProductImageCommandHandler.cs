using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.ProductItems.Commands.AddProductImage;

public class AddProductImageCommandHandler(
    IAppDbContext context,
    IImageService imageService)
    : IRequestHandler<AddProductImageCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddProductImageCommand request, CancellationToken ct)
    {
        // 1. Find the product in the database
        var product = await context.ProductItems
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

        if (product is null)
        {
            return Error.NotFound("Product.NotFound", $"Product with ID {request.ProductId} was not found.");
        }

        var uploadedImageIds = new List<Guid>();

        foreach (var file in request.Files)
        {
            // 2. Upload image to Cloudinary using the ImageService
            var uploadResult = await imageService.UploadImageAsync(file);

            if (uploadResult is null)
            {
                return Error.Failure("Image.UploadFailed", "Failed to upload image to the cloud provider.");
            }

            // Only the first image in the loop becomes Main if IsMain is true
            var isImageMain = request.IsMain && uploadedImageIds.Count == 0;

            // 3. Use Domain Logic to link the image to the product
            var result = product.AddImage(
                uploadResult.Url, 
                uploadResult.PublicId, 
                isImageMain);

            if (result.IsError)
            {
                // Delete from Cloudinary if Domain validation fails to prevent orphaned files
                await imageService.DeleteImageAsync(uploadResult.PublicId);
                return result.Errors;
            }

            uploadedImageIds.Add(product.Images.Last().Id);
        }

        // 4. Persist changes to the database
        await context.SaveChangesAsync(ct);

        // Return the ID of the last added image (or you could change the return type to Result<List<Guid>>)
        return product.Images.Last().Id;
    }
}