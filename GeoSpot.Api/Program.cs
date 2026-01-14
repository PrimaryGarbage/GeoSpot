using GeoSpot.Api.Initialization;
using GeoSpot.Application;
using GeoSpot.Persistence;
using FluentValidation;
using GeoSpot.Api.Middleware;
using GeoSpot.Application.Dispatcher;
using GeoSpot.Application.Validators.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.BindConfigurationSections(builder.Configuration);

builder.Services.AddGeoSpotPersistenceModule(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddVersioning();

builder.Services.AddApplicationServices();

builder.Services.AddValidatorsFromAssemblyContaining<SendVerificationCodeRequestValidator>();

builder.Services.RegisterHandlers();

builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.PrepareDatabase();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseMiddleware<JwtClaimsExtractionMiddleware>();

app.Run();