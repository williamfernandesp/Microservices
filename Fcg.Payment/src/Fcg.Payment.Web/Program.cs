using Fcg.Payment.Application.Requests;
using Fcg.Payment.Application.Services;
using Fcg.Payment.Domain.Repositories;
using Fcg.Payment.Infra.Data;
using Fcg.Payment.Infra.Repositories;
using Fcg.Payment.Proxy.Auth;
using Fcg.Payment.Proxy.Games;
using Fcg.Payment.Proxy.User;
using Fcg.Observability;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Fcg.Payment.Application.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add OpenTelemetry observability
builder.Services.AddObservability(builder.Configuration, "fcg-payment");

// Add health checks
builder.Services.AddHealthChecks();

builder.Services.AddInfraProxyAuth(builder.Configuration);
builder.Services.AddInfraProxyUser(builder.Configuration);
builder.Services.AddInfraProxyGames(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GamePurchaseValidatedConsumer>();
    x.AddConsumer<GamePurchaseRejectedConsumer>();

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

        cfg.ReceiveEndpoint("fcg-payment-game-purchase-validated", e =>
        {
            e.ConfigureConsumer<GamePurchaseValidatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("fcg-payment-game-purchase-rejected", e =>
        {
            e.ConfigureConsumer<GamePurchaseRejectedConsumer>(context);
        });
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Fcg.Payments API",
        Version = "v1"
    });
});

var connection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=payments.db";

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(connection));

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<PaymentService>();

var app = builder.Build();

// Commented out for Docker - run migrations manually if needed
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
//     db.Database.Migrate();
// }

app.UseSwagger();
app.UseSwaggerUI(c =>
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fcg.Payments API v1"));

app.UseHttpsRedirection();

#region Payment Endpoints
app.MapPost("/payments/checkout", async (CheckoutRequest req, PaymentService service) =>
{
    if (req.Amount <= 0)
        return Results.BadRequest(new { message = "O valor deve ser maior que zero." });

    var response = await service.CheckoutAsync(req.UserId, req.Amount);

    if (response.Erros.Any())
        return Results.BadRequest(new { message = response.Erros });

    return Results.Created($"/payments/{response.Result.PaymentId}", response.Result);
}).WithTags("Payments");

app.MapPost("/payments/purchase-game", async (PurchaseGameRequest req, PaymentService service) =>
{
    var response = await service.PurchaseGameAsync(req.UserId, req.GameId);

    if (response.Erros.Any())
        return Results.BadRequest(new { message = response.Erros });

    return Results.Accepted($"/payments/{response.Result.PaymentId}", new
    {
        message = "Compra solicitada. A validação do game/preço será processada de forma assíncrona.",
        data = response.Result
    });
}).WithTags("Payments");

app.MapGet("/payments/{id:guid}", async (Guid id, PaymentService service) =>
{
    var response = await service.GetByIdAsync(id);

    if (response.Erros.Any())
        return Results.NotFound(new { message = response.Erros });

    return Results.Ok(response.Result);
}).WithTags("Payments");

app.MapPut("/payments/{id:guid}/approve", async (Guid id, PaymentService service) =>
{
    var response = await service.ApproveAsync(id);

    if (response.Erros.Any())
        return Results.BadRequest(new { message = response.Erros });

    return Results.Ok(new { message = "Pagamento aprovado com sucesso." });
}).WithTags("Payments");

app.MapPut("/payments/{id:guid}/reject", async (Guid id, PaymentService service) =>
{
    var response = await service.RejectAsync(id);

    if (response.Erros.Any())
        return Results.BadRequest(new { message = response.Erros });

    return Results.Ok(new { message = "Pagamento rejeitado com sucesso." });
}).WithTags("Payments");

#endregion

// Add observability endpoints (health checks, metrics)
app.UseObservabilityEndpoints();

app.Run();