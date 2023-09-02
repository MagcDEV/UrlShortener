using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Entities;
using UrlShortener.Extensions;
using UrlShortener.Models;
using UrlShortener.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UrlShorteningService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.ApplyMigrations();


app.MapPost("api/shorter", async (
    ShortenerUrlRequest request,
    UrlShorteningService service,
    AppDbContext context,
    HttpContext httpContext) =>
{

    if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("The specified URL is invalid");
    }

    var code = await service.GenerateShortLink();

    var ShortenedUrl = new ShortenedUrl
    {
        Id = Guid.NewGuid(),
        LongUrl = request.Url,
        Code = code,
        ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{code}",
        CreatedOnUtc = DateTime.UtcNow
    };

    context.ShortenedUrls.Add(ShortenedUrl);
    await context.SaveChangesAsync();

    return Results.Ok(ShortenedUrl.ShortUrl);
});

app.MapGet("api/{code}", async (
    string code,
    AppDbContext context) =>
{

    var ShortenedUrl = await context.ShortenedUrls.FirstOrDefaultAsync(x => x.Code == code);

    if (ShortenedUrl == null)
    {
        return Results.NotFound();
    }

    return Results.Redirect(ShortenedUrl.LongUrl);

});


app.UseHttpsRedirection();

app.Run();

