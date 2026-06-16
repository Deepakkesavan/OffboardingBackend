// ─────────────────────────────────────────────────────────────────────────────
//  PROGRAM.CS  —  Application entry point and full configuration
//
//  .NET 8 uses the "minimal hosting model" which merges Startup.cs and
//  Program.cs into a single file.
//
//  The file is split into two phases:
//    1. SERVICE REGISTRATION  — tell the DI container what exists
//    2. MIDDLEWARE PIPELINE   — tell ASP.NET Core how to handle requests
//
//  Trimmed to register only what the four active endpoints need:
//    EmsDataController, SubmissionLogController, ManagerInfoController,
//    ApproveOffboardingController.
// ─────────────────────────────────────────────────────────────────────────────

using DotNetCommonLib.Extensions;
using Microsoft.EntityFrameworkCore;
using offboarding_prc_api.Constants;
using offboarding_prc_api.Data;
using offboarding_prc_api.Middleware;
using offboarding_prc_api.Services;

// ═════════════════════════════════════════════════════════════════════════════
//  STEP 1 — Create the builder
// ═════════════════════════════════════════════════════════════════════════════
var builder = WebApplication.CreateBuilder(args);


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 2 — Register the DbContext (EF Core + SQL Server)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 3 — Register application services
//
//  Only the services backing the four live endpoints are registered:
//    • ManagerInfoService    → /api/managerinfo
//    • TempCacheService      → /api/EmsData
//    • SubmissionLogService  → /api/submission/submit, /api/submission/getsubmit,
//                              and AdvanceStageAsync used by ApproveOffboardingController
// ─────────────────────────────────────────────────────────────────────────────

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ManagerInfoService>();
builder.Services.AddScoped<TempCacheService>();
builder.Services.AddScoped<SubmissionLogService>();
builder.Services.AddHttpClient(BussinessConstant.EMS_CLIENT_TITLE)
    .AddHttpMessageHandler<OutgoingRequestHandler>();
builder.Services.AddTransient<OutgoingRequestHandler>();

// ─────────────────────────────────────────────────────────────────────────────
//  STEP 4 — Register Controllers
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;

        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 5 — Configure CORS (Cross-Origin Resource Sharing)
// ─────────────────────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? ["http://localhost:5173", "https://workspace-dev.clarium.tech"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 6 — Register Swagger (API documentation UI)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 7 — Build the app
// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();


// ═════════════════════════════════════════════════════════════════════════════
//  MIDDLEWARE PIPELINE
// ═════════════════════════════════════════════════════════════════════════════


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 8 — Global exception handler (MUST be first)
// ─────────────────────────────────────────────────────────────────────────────
app.UseMiddleware<GlobalExceptionMiddleware>();


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 9 — Swagger UI (Development only)
// ─────────────────────────────────────────────────────────────────────────────
app.UsePathBase("/offapi");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("v1/swagger.json", "Offboarding API v1");
        options.RoutePrefix = "swagger";
    });
}


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 10 — HTTPS Redirection
// ─────────────────────────────────────────────────────────────────────────────
app.UseHttpsRedirection();


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 11 — CORS (MUST be before UseAuthorization and MapControllers)
// ─────────────────────────────────────────────────────────────────────────────
app.UseCors("AllowFrontend");
app.UseJwtAuthMiddleware(); // Custom middleware to validate JWTs in Phase 2 (no-op in Phase 1)


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 12 — Authorization (placeholder for Phase 2)
// ─────────────────────────────────────────────────────────────────────────────
app.UseAuthorization();


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 13 — Map controllers
// ─────────────────────────────────────────────────────────────────────────────
app.MapControllers();


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 14 — Auto-run EF Core migrations on startup (Development only)
// ─────────────────────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 15 — Start the server
// ─────────────────────────────────────────────────────────────────────────────
app.Run();