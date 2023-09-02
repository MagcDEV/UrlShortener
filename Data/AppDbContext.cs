using Microsoft.EntityFrameworkCore;
using UrlShortener.Entities;
using UrlShortener.Services;

namespace UrlShortener.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortenedUrl>(builder => {
            builder.Property(s => s.Code).HasMaxLength(UrlShorteningService.NumberOfCharsInShortLink);
            builder.HasIndex(x => x.Code).IsUnique();
        });
            
    }
}

    
