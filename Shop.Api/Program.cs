using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shop.Api;
using Shop.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ShopContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Shop.Data")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Shop API",
            Version = "v1",
            Description = "REST API for Shop Application"
        });
        
        options.DocumentFilter<SwaggerDocumentFilter>();
    }

    );

builder.Services.AddHttpLogging(options => {});
builder.Services.AddLogging();
builder.Services.AddDefaultCorrelationId();
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ShopContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop API V1");
        c.OAuthAppName("Shop API");
    });
}

app.UseCorrelationId();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
