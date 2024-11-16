using PaymentGateway.Api.Repository;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Tests.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = builder.Configuration
    .AddJsonFile("appsettings.json")
    .Build();

var acquiringPayment = config.GetRequiredSection(AquiringPaymentSettings.Section).Get<AquiringPaymentSettings>();
if (acquiringPayment?.ServiceEndpoint == null)
{
    throw new Exception("Acquiring Service Endpoint not defined");
}

builder.Services.AddSingleton<IPaymentsRepository, InMemoryPaymentsRepository>();
builder.Services.AddSingleton<ICardPaymentService, CardPaymentService>();
builder.Services.AddHttpClient(nameof(CardPaymentService), client =>
{
    client.BaseAddress = acquiringPayment.ServiceEndpoint;
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
