using Fcg.User.Application;
using Fcg.User.Application.Requests;
using Fcg.User.Infra;
using Fcg.User.Proxy.Auth;
using Fcg.User.Proxy.Games;
using Fcg.User.Proxy.Payment;
using Fcg.Observability;
using MediatR;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Fcg.User.Web.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add OpenTelemetry observability
builder.Services.AddObservability(builder.Configuration, "fcg-user");

// Add health checks
builder.Services.AddHealthChecks();

builder.Services.AddApplicationLayer();
builder.Services.AddInfraLayer(builder.Configuration);
builder.Services.AddInfraProxyAuth(builder.Configuration);
builder.Services.AddInfraProxyGames(builder.Configuration);
builder.Services.AddInfraProxyPayment(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentApprovedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RabbitMq:Host"] ?? "localhost";
        var vhost = builder.Configuration["RabbitMq:VirtualHost"] ?? "/";
        var username = builder.Configuration["RabbitMq:Username"] ?? "guest";
        var password = builder.Configuration["RabbitMq:Password"] ?? "guest";

        cfg.Host(host, vhost, h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ReceiveEndpoint("fcg-user-payment-approved", e =>
        {
            e.ConfigureConsumer<PaymentApprovedConsumer>(context);
        });
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();

#region Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT com prefixo 'Bearer '"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

#region Authentication & Authorization
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});
#endregion

var app = builder.Build();

#region User Endpoints
app.MapGet("users/{id}", async (Guid id, IMediator _mediator) =>
{
    var response = await _mediator.Send(new GetUserByIdRequest { Id = id });

    return Results.Ok(response);
}).AllowAnonymous().WithTags("Users");

app.MapGet("users/{id}/most-owned-genre", async (Guid id, IMediator _mediator) =>
{
    var response = await _mediator.Send(new GetMostOwnedGenreRequest { UserId = id });

    return Results.Ok(response);
}).AllowAnonymous().WithTags("Users");

app.MapGet("users", async ([AsParameters] GetUsersRequest request, IMediator _mediator) =>
{
    var response = await _mediator.Send(request);

    return Results.Ok(response);
}).AllowAnonymous().WithTags("Users");

app.MapPut("users/{id}", async (Guid id, [FromBody] UpdateUserRequest request, IMediator _mediator) =>
{
    request.Id = id;

    var response = await _mediator.Send(request);

    return Results.Ok(response);
}).AllowAnonymous().WithTags("Users");

app.MapDelete("users/{id}", async (Guid id, IMediator _mediator) =>
{
    var response = await _mediator.Send(new DeleteUserRequest { Id = id });

    return Results.Ok(response);
}).AllowAnonymous().WithTags("Users");

app.MapPost("users", async ([FromBody] RegisterUserRequest request, IMediator _mediator) =>
{
    var response = await _mediator.Send(request);

    return Results.Ok(response);
}).AllowAnonymous().WithTags("Users");

app.MapPost("users/{id}/buy-games", async (Guid id, [FromBody] BuyGamesRequest request, IMediator _mediator) =>
{
    request.UserId = id;

    var response = await _mediator.Send(request);

    return Results.Ok(response);
}).AllowAnonymous().WithTags("Users");

app.MapPost("users/{id}/credit", async (Guid id, [FromBody] CreditWalletRequest request, IMediator _mediator) =>
{
    request.Id = id;

    var response = await _mediator.Send(request);

    return Results.Ok(response);
}).AllowAnonymous().WithTags("Users");

app.MapPost("users/{id}/debit", async (Guid id, [FromBody] DebitWalletRequest request, IMediator _mediator) =>
{
    request.Id = id;

    var response = await _mediator.Send(request);

    return Results.Ok(response);
}).AllowAnonymous().WithTags("Users");
#endregion

#region Middleware Pipeline
app.UseSwagger();
app.UseSwaggerUI();

// Add observability endpoints (health checks, metrics)
app.UseObservabilityEndpoints();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion
