using Microsoft.AspNetCore.Http.Features;
using Bodokado.API.DependencyInjection;
using Bodokado.API.Middleware;
using Bodokado.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCoreDependencies(builder.Configuration);
builder.Services.AddHttpClient("RemoteImage", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000" ,"http://94.184.46.18:80" ,"http://94.184.46.18:30000")
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

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/shop/swagger.json", "Shop API");
    options.SwaggerEndpoint("/swagger/customer/swagger.json", "Customer API");
    options.SwaggerEndpoint("/swagger/user-organization/swagger.json", "User Organization API");
    options.SwaggerEndpoint("/swagger/admin/swagger.json", "Admin API");
    options.RoutePrefix = "swagger";
});


app.MapOpenApi();

await app.MigrateAndSeedDatabaseAsync();

app.UseCors("AllowLocalhost3000");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
