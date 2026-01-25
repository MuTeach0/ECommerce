using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.ProductItems.Commands.SetMainImage;

public class SetMainImageCommandHandler(IAppDbContext context) 
    : IRequestHandler<SetMainImageCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(SetMainImageCommand request, CancellationToken ct)
    {
        var product = await context.ProductItems
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

        if (product is null) return Error.NotFound("Product.NotFound", "المنتج غير موجود");

        var image = product.Images.FirstOrDefault(i => i.Id == request.ImageId);
        if (image is null) return Error.NotFound("Image.NotFound", "الصورة غير موجودة لهذا المنتج");

        foreach (var img in product.Images)
        {
            if (img.Id == request.ImageId)
            {
                img.SetAsMain();
            }
            else
            {
                img.UnsetMain();
            }
        }

        await context.SaveChangesAsync(ct);
        return Result.Updated;
    }
}