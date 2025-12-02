using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<Infrastructure.Persistence.MindWaveDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<Application.Abstractions.Auth.ILoginService, Infrastructure.Auth.LoginService>();
builder.Services.AddScoped<Application.Abstractions.Auth.IRegistrationService, Infrastructure.Auth.RegistrationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
