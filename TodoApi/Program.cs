using Microsoft.EntityFrameworkCore;
using TodoApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ EF Core + PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Pega a connection string do appsettings.json
    var cs = builder.Configuration.GetConnectionString("Default");
    options.UseNpgsql(cs);
});

// ✅ CORS (para o Vue conseguir acessar a API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowVue");

app.MapControllers();

app.Run();