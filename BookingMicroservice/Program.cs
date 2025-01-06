using BookingMicroservice;
using BookingMicroservice.Entities;
using BookingMicroservice.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Ocelot.Values;
using RabbitMQ.Client;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddSignalR();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BookingDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddSingleton<RabbitMQ.Client.IConnectionFactory, ConnectionFactory>(sp => new ConnectionFactory
{
    HostName = "localhost"
});
builder.Services.AddSingleton<SseService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddHostedService<BookingConsumerService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:5207")
                .AllowAnyHeader()
                .WithMethods("GET", "POST")
                .AllowCredentials();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors();

app.MapControllers();

app.MapGet("/api/sse/{appointmentId}", async (long appointmentId, HttpResponse response, SseService sseService) =>
{
    await sseService.SubscribeAsync(appointmentId, response);
});
app.MapHub<AppointmentHub>("/appointmentHub");

app.Run();
