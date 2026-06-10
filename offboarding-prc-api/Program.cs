// ─────────────────────────────────────────────────────────────────────────────
//  PROGRAM.CS  —  Application entry point and full configuration
//
//  .NET 8 uses the "minimal hosting model" which merges Startup.cs and
//  Program.cs into a single file.
//
//  The file is split into two phases:
//    1. SERVICE REGISTRATION  — tell the DI container what exists
//    2. MIDDLEWARE PIPELINE   — tell ASP.NET Core how to handle requests
// ─────────────────────────────────────────────────────────────────────────────

using DotNetCommonLib.Extensions;
using Microsoft.EntityFrameworkCore;
using offboarding_prc_api.Constants;
using offboarding_prc_api.Data;
using offboarding_prc_api.Middleware;
using offboarding_prc_api.Services;

// ═════════════════════════════════════════════════════════════════════════════
//  STEP 1 — Create the builder
//  WebApplication.CreateBuilder() sets up:
//    • Configuration (reads appsettings.json, environment variables, etc.)
//    • Logging (console, debug)
//    • The DI (Dependency Injection) container  ← builder.Services
// ═════════════════════════════════════════════════════════════════════════════
var builder = WebApplication.CreateBuilder(args);


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 2 — Register the DbContext (EF Core + SQL Server)
//
//  What this does:
//    • Reads "DefaultConnection" from appsettings.json
//    • Registers AppDbContext as a Scoped service (one instance per HTTP request)
//    • Tells EF Core to use SQL Server as the database provider
//
//  "Scoped" is the correct lifetime for DbContext because:
//    - It keeps a single DB connection open for the duration of one request
//    - It is disposed automatically when the request ends
//    - It prevents connection leaks and concurrency issues
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            // Automatically retry transient SQL failures (network blips, timeouts)
            // up to 5 times with exponential backoff. Built into the SQL Server provider.
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 3 — Register application services
//
//  Services are classes that controllers depend on (injected via constructors).
//  Lifetime options:
//    • Scoped    → one instance per HTTP request  (use for DB-aware services)
//    • Transient → new instance every time it is requested
//    • Singleton → one instance for the whole app lifetime
//
//  StageGateService and ClearanceService use AppDbContext so they must be
//  Scoped (same lifetime as the DbContext they depend on).
//  NoticePeriodService has no dependencies so Transient is fine.
// ─────────────────────────────────────────────────────────────────────────────

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<TempCacheService>();
builder.Services.AddScoped<MemoryCacheService>();
builder.Services.AddScoped<StageGateService>();
builder.Services.AddScoped<ClearanceService>();
builder.Services.AddTransient<NoticePeriodService>();
builder.Services.AddHttpClient(BussinessConstant.EMS_CLIENT_TITLE)
.AddHttpMessageHandler<OutgoingRequestHandler>();
builder.Services.AddTransient<OutgoingRequestHandler>();
builder.Services.AddHttpClient(BussinessConstant.CONFIGURATION_CLIENT_TITLE, (client) =>
{
    client.BaseAddress = new Uri(builder.Configuration["Config_Url"] ?? throw new InvalidOperationException(ErrorMessages.CONFIGURATION_MISSING));
});

// ─────────────────────────────────────────────────────────────────────────────
//  STEP 4 — Register Controllers
//
//  AddControllers() scans the assembly for classes decorated with [ApiController]
//  and registers them. It also configures:
//    • Model binding  (JSON body → C# objects)
//    • Automatic 400 responses for invalid models
//    • Action filters
//
//  We add a JSON option here: PropertyNamingPolicy = camelCase
//  This means C# PascalCase properties (EmployeeName) are serialised as
//  camelCase JSON (employeeName) — which is what the React frontend expects.
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // camelCase JSON  →  matches JavaScript / React conventions
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;

        // Ignore null values in responses to keep payloads clean
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 5 — Configure CORS (Cross-Origin Resource Sharing)
//
//  The React app runs on http://localhost:5173 (Vite default).
//  The API runs on http://localhost:5000.
//  Browsers block cross-origin requests by default — CORS tells the browser
//  "it is safe to let the frontend call this API".
//
//  We define a named policy called "AllowFrontend" and apply it globally.
//
//  AllowedOrigins comes from appsettings.json → Cors → AllowedOrigins
//  so you can change it per environment without recompiling.
// ─────────────────────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? ["http://localhost:5173" , "https://workspace-dev.clarium.tech"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)   // only allow our React app
            .AllowAnyHeader()              // allow Content-Type, Authorization, etc.
            .AllowAnyMethod()              // allow GET, POST, PATCH, DELETE, OPTIONS
            .AllowCredentials();           // allow cookies (needed for Phase 2 JWT)
    });
});


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 6 — Register Swagger (API documentation UI)
//
//  Swagger generates a browser-based UI at /swagger where you can see and
//  test every endpoint without needing Postman.
//
//  AddEndpointsApiExplorer() — discovers minimal API endpoints
//  AddSwaggerGen()           — generates the OpenAPI spec + UI
//
//  Only enable Swagger in Development — never expose it in production.
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 7 — Build the app
//
//  builder.Build() finalises the DI container and creates the WebApplication
//  object. After this line you cannot register any more services — only
//  configure the middleware pipeline.
// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();


// ═════════════════════════════════════════════════════════════════════════════
//  MIDDLEWARE PIPELINE
//
//  Each middleware component is a layer in a stack. A request passes through
//  every layer on the way IN, hits the endpoint, then passes through every
//  layer again on the way OUT (in reverse order).
//
//  ORDER MATTERS — middleware runs in the exact order it is added here.
// ═════════════════════════════════════════════════════════════════════════════


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 8 — Global exception handler (MUST be first)
//
//  By registering this before everything else it acts as a safety net:
//  any unhandled exception thrown anywhere in the pipeline is caught here
//  and returned as a clean JSON error response instead of a crash page.
// ─────────────────────────────────────────────────────────────────────────────
app.UseMiddleware<GlobalExceptionMiddleware>();


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 9 — Swagger UI (Development only)
//
//  Navigate to https://localhost:PORT/swagger to see the API explorer.
//  This is guarded by IsDevelopment() so it never appears in production.
// ─────────────────────────────────────────────────────────────────────────────
app.UsePathBase("/offapi");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("v1/swagger.json", "Offboarding API v1");
        options.RoutePrefix = "swagger";  // access at /swagger
    });
}


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 10 — HTTPS Redirection
//
//  Automatically redirects HTTP requests to HTTPS.
//  Comment this out if you are running on plain HTTP during local dev.
// ─────────────────────────────────────────────────────────────────────────────
app.UseHttpsRedirection();


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 11 — CORS (MUST be before UseAuthorization and MapControllers)
//
//  Applies the "AllowFrontend" policy defined in Step 5.
//  The browser sends an OPTIONS "preflight" request before every cross-origin
//  POST/PATCH. This middleware handles those preflight requests.
// ─────────────────────────────────────────────────────────────────────────────
app.UseCors("AllowFrontend");
app.UseJwtAuthMiddleware(); // Custom middleware to validate JWTs in Phase 2 (no-op in Phase 1)


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 12 — Authorization (placeholder for Phase 2)
//
//  In Phase 2, JWT validation middleware will be added here with
//  builder.Services.AddAuthentication().AddJwtBearer(...)
//  and app.UseAuthentication() will be added BEFORE this line.
//
//  For Phase 1, UseAuthorization() is a no-op (no [Authorize] attributes used).
// ─────────────────────────────────────────────────────────────────────────────
app.UseAuthorization();


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 13 — Map controllers
//
//  Scans all registered [ApiController] classes and maps their routes.
//  This is the line that makes GET /api/employees actually work.
// ─────────────────────────────────────────────────────────────────────────────
app.MapControllers();


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 14 — Auto-run EF Core migrations on startup (Development only)
//
//  This block creates the database and applies all pending migrations
//  automatically every time the app starts in Development mode.
//
//  In Production: run  dotnet ef database update  from the CI/CD pipeline
//  instead of doing it at runtime — it gives you more control.
// ─────────────────────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}


// ─────────────────────────────────────────────────────────────────────────────
//  STEP 15 — Start the server
//
//  app.Run() blocks and starts listening for HTTP requests.
//  The port is configured in Properties/launchSettings.json or by the
//  ASPNETCORE_URLS environment variable.
// ─────────────────────────────────────────────────────────────────────────────
app.Run();