<<<<<<< Updated upstream
=======
using System.Text.Json.Serialization;

using Microsoft.OpenApi.Models;

>>>>>>> Stashed changes
using PaymentGateway.Api.Domain.Services;
using PaymentGateway.Api.Infrastructure.AcquiringBank;
using PaymentGateway.Api.Infrastructure.Configuration;
using PaymentGateway.Api.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration
    .AddJsonFile("appsettings.json")
    .Build();

// Add services to the container.
var acquiringPayment = config.GetRequiredSection(nameof(AcquiringPaymentSettings)).Get<AcquiringPaymentSettings>();
if (acquiringPayment?.ServiceEndpoint == null)
{
    throw new Exception("Acquiring Service Endpoint not defined");
}

builder.Services.AddSingleton<IPaymentsRepository, InMemoryPaymentsRepository>();
builder.Services.AddSingleton<ICardPaymentService, CardPaymentService>();
builder.Services.AddSingleton<IAcquiringBankClient, AcquiringBankClient>();
builder.Services.AddHttpClient(nameof(AcquiringBankClient), client =>
{
    client.BaseAddress = acquiringPayment.ServiceEndpoint;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var filePath = Path.Combine(AppContext.BaseDirectory, "PaymentGateway.Api.xml");
    options.IncludeXmlComments(filePath);
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Payment Gateway API", Description = "API for making and retrieving payments.", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
