using Microsoft.EntityFrameworkCore;
using SIPM_Backend.Data;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// ====================================================
// 1. Konfigurimi i Shërbimeve (Services)
// ====================================================

// Add controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });


// Entity Framework Core + SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }
    )
);

// CORS - Lejo frontend-in të lidhet me API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5500",
                "http://127.0.0.1:5500",
                "http://localhost:3000",
                "http://127.0.0.1:3000"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Swagger/OpenAPI për dokumentim API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SIPM ARKIMEDO-21 API",
        Version = "v1",
        Description = "Backend API për Sistemin e Informacionit të Pajisjeve Mjekësore",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "KRIZA",
            Email = "info@oni.al"
        }
    });
});

// ====================================================
// 2. Ndërtimi i Aplikacionit (App)
// ====================================================

var app = builder.Build();

// ====================================================
// 3. Middleware Pipeline
// ====================================================

// Swagger UI (vetëm në Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SIPM API v1");
        c.RoutePrefix = string.Empty; // Swagger në root: https://localhost:5001/
    });
}

// HTTPS Redirect
app.UseHttpsRedirection();

// CORS - Duhet para Authorization
app.UseCors("AllowFrontend");

// Authorization
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// ====================================================
// 4. Database Migration & Seeding
// ====================================================

// Automatikisht krijon database në startup (nëse nuk ekziston)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Krijo database + tabelat
        context.Database.EnsureCreated();
        
        Console.WriteLine("✅ Database u krijua ose ekziston tashmë");
        Console.WriteLine($"📊 Connection String: {builder.Configuration.GetConnectionString("DefaultConnection")}");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Gabim gjatë krijimit të database");
    }
}

// ====================================================
// 5. Run API
// ====================================================

Console.WriteLine("🚀 SIPM ARKIMEDO-21 Backend API po fillon...");
Console.WriteLine("📍 API URL: https://localhost:5001");
Console.WriteLine("📖 Swagger UI: https://localhost:5001/swagger");
Console.WriteLine("🔗 Frontend: http://localhost:5500 (Live Server)");

app.Run();
