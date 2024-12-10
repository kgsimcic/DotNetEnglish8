using BookingMicroservice.Entities;
using BookingMicroservice.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BookingDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddSingleton<RabbitMQ.Client.IConnectionFactory, ConnectionFactory>(sp => new ConnectionFactory
{
    HostName = "localhost"
});
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddHostedService<BookingConsumerService>();
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
