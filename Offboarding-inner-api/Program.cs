using Microsoft.EntityFrameworkCore;
using Offboarding_inner_api.Model;
using Offboarding_inner_api.Services.Implementations;
using Offboarding_inner_api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IEmployeeOffboardingService, EmployeeOffboardingService>();
builder.Services.AddScoped<IAdminApprovalService, AdminApprovalService>();
builder.Services.AddScoped<IHrApprovalService, HrApprovalService>();
builder.Services.AddScoped<IItDepartmentService, ItDepartmentService>();
builder.Services.AddScoped<IReportingManagerService, ReportingManagerService>();
// Add services to the container.


builder.Services.AddControllers();
builder.Services.AddDbContext<EmployeeDetailsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Only if needed
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
