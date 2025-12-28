using GeoSpot.Api.Initialization;
using GeoSpot.Application;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Persistence;
using FluentValidation;
using GeoSpot.Api.Middleware;
using GeoSpot.Api.PipelineBehaviors;
using GeoSpot.Application.Validators.Auth;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.BindConfigurationSections(builder.Configuration);

builder.Services.AddGeoSpotPersistenceModule(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddApplicationServices();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IJwtTokenService>());
builder.Services.AddValidatorsFromAssemblyContaining<SendVerificationCodeRequestValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.Run();