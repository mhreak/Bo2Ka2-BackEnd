using Microsoft.AspNetCore.Http.Features;
using Bodokado.API.DependencyInjection;
using Bodokado.API.Middleware;
using Bodokado.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCoreDependencies(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600;
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/shop/swagger.json", "Shop API");
    options.SwaggerEndpoint("/swagger/customer/swagger.json", "Customer API");
    options.SwaggerEndpoint("/swagger/corporate/swagger.json", "Corporate API");
    options.SwaggerEndpoint("/swagger/admin/swagger.json", "Admin API");
    options.RoutePrefix = "swagger";
});


app.MapOpenApi();

await app.MigrateAndSeedDatabaseAsync();

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost3000");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
