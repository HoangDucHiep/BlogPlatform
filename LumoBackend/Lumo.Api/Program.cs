using Lumo.Api.Extensions;
using Lumo.Application;
using Lumo.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllers();

// OpenAPI and Swagger configuration
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Add Configurations
//builder.Services.Configure<JsonOptions>(options =>
//{
//    options.SerializerOptions.Converters.Add(new ResultJsonConverterFactory());
//});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add Health checks
builder.Services.AddHealthChecks();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
    app.ApplyMigrations();
}

app.UseCustomExceptionHandler();

app.MapGet("/", () => "Hello from Lumo Blog API!");

// Add health check endpoint
app.MapHealthChecks("/health");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
