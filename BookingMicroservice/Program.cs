using BookingMicroservice.Entities;
using BookingMicroservice.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => { 
    options.AddPolicy("AllowAll", builder => { 
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); 
    }); 
});

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
builder.Services.AddHostedService<BookingConsumerService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowAll");

app.MapControllers();
app.MapGet("/api/sse/{appointmentId}", async (long appointmentId, HttpResponse response, SseService sseService) => { 
    await sseService.SubscribeAsync(appointmentId, response); 
});

app.Run();
