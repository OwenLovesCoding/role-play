using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using role_play.Models;
using FluentEmail.Core;
using FluentEmail.Smtp;
using Resend;
//using ResendClient;

var builder = WebApplication.CreateBuilder(args);

// connect db to the container.
// Add DbContext with your database provider

var connectionString = "Host=shortline.proxy.rlwy.net;Port=49418;Database=railway;Username=postgres;Password=yFwomSrjwHWhSZFzzyMppoKhrNZqncQd;SSL Mode=Require;Trust Server Certificate=true";

builder.Services.AddDbContext<UserContext>(options =>
{
    // Choose ONE based on your database:

    // For SQL Server:
    //options.UseSqlServer(builder.Configuration["SQLServer:DefaultConnection"]);

    // For SQLite:
    // options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

    // For PostgreSQL:
    //options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseNpgsql(connectionString);

    // For MySQL:
    // options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
    //     new MySqlServerVersion(new Version(8, 0, 0)));
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
//builder.Services.AddTransient<IResend>(sp =>
//ResendClient.Create(builder.Configuration["Resend:ApiKey"]!));

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
