using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using MyImageResult = ECommerce.Application.Common.Models.ImageUploadResult;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Services;

public class CloudinaryService : IImageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration config)
    {
        // Get credentials from appsettings.json
        var acc = new Account(
            config["CloudinarySettings:CloudName"],
            config["CloudinarySettings:ApiKey"],
            config["CloudinarySettings:ApiSecret"]
        );

        _cloudinary = new Cloudinary(acc);
    }

    public async Task<MyImageResult?> UploadImageAsync(IFormFile file)
    {
        if (file.Length <= 0) return null;

        await using var stream = file.OpenReadStream();
        
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            // Optimized transformations for E-commerce products
            Transformation = new Transformation()
                .Height(500)
                .Width(500)
                .Crop("fill")
                .Gravity("auto") // changed to auto to focus on the product, not just faces
                .Quality("auto")
                .FetchFormat("auto")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            throw new Exception(uploadResult.Error.Message);

        // Map to the Common Model (DTO)
        return new MyImageResult(
            uploadResult.PublicId,
            uploadResult.SecureUrl.ToString()
        );
    }

    public async Task<string?> DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);

        return result.Result == "ok" ? result.Result : null;
    }
}