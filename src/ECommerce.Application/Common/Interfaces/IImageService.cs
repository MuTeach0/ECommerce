using ECommerce.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.Common.Interfaces;

public interface IImageService
{
    Task<ImageUploadResult?> UploadImageAsync(IFormFile file);
    Task<string?> DeleteImageAsync(string publicId);
}