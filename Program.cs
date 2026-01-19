using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using role_play.Models;
using FluentEmail.Core;
using FluentEmail.Smtp;
using Resend;
//using ResendClient;

var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine($"Connection string starts with: {connectionString.Substring(0, Math.Min(30, connectionString.Length))}...");
}

builder.Services.AddDbContext<UserContext>(options =>
{
    
    // For PostgreSQL:
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    
});


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Resend
builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(o => {
    o.ApiToken = builder.Configuration["Resend:ApiKey"]!;
});
builder.Services.AddTransient<IResend, ResendClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
