using Microsoft.EntityFrameworkCore;
using role_play.Models;

var builder = WebApplication.CreateBuilder(args);

// connect db to the container.
// Add DbContext with your database provider
builder.Services.AddDbContext<UserContext>(options =>
{
    // Choose ONE based on your database:

    // For SQL Server:
    options.UseSqlServer(builder.Configuration["SQLServer:DefaultConnection"]);

    // For SQLite:
    // options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

    // For PostgreSQL:
    // options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

    // For MySQL:
    // options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
    //     new MySqlServerVersion(new Version(8, 0, 0)));
});


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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
