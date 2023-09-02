using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;

namespace UrlShortener.Extensions;

public static class MigrationExtensions
{
    public static async void ApplyMigrations(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }
    
}