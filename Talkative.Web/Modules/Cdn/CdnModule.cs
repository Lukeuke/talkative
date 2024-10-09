using Microsoft.AspNetCore.Mvc;
using Talkative.Domain.Entities;
using Talkative.Infrastructure.Repositories.Abstraction;
using Path = System.IO.Path;

namespace Talkative.Web.Modules.Cdn;

public static class CdnModule
{
    public static void AddCdnEndpoint(this WebApplication app)
    {
        // r_(guid) = room profile
        app.MapGet("api/cdn/{id}", async (string id) =>
        {
            var assetsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Talkative", "assets");
            
            var imagePath = Path.Combine(assetsPath, id);

            if (!File.Exists(imagePath))
            {
                return Results.NotFound();
            }

            var imageBytes = await File.ReadAllBytesAsync(imagePath);

            var contentType = GetContentType(id);

            return Results.File(imageBytes, contentType);
        });
        
        app.MapGet("api/cdn/user/{id}", async (
            Guid id,
            [FromServices] BaseRepository<User> userRepository
            ) =>
        {
            var user = await userRepository.GetByIdAsync(id);

            if (user?.ImageUrl is null)
            {
                return Results.NotFound();
            }
            
            var assetsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Talkative", "assets");
            
            var imagePath = Path.Combine(assetsPath, user.ImageUrl);

            if (!File.Exists(imagePath))
            {
                return Results.NotFound();
            }

            var imageBytes = await File.ReadAllBytesAsync(imagePath);

            var contentType = GetContentType(imagePath);

            return Results.File(imageBytes, contentType);
        });
    }
    
    private static string GetContentType(string filename)
    {
        var extension = Path.GetExtension(filename).ToLowerInvariant();
        return extension switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }
}