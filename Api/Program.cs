using Api;
using Application;
using Application.Interfaces;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("ServerManagerDb"));

builder.Services.AddScoped<IServerRepository, ServerRepository>();
builder.Services.AddScoped<IServerService, ServerService>();

builder.Services.AddHostedService<AutoReleaseServerJob>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!context.Servers.Any())
    {
        context.Servers.AddRange(
            new ServerEntity { Id = Guid.NewGuid(), OsName = "Ubuntu 22.04", RamGb = 16, CpuCores = 8, DiskGb = 500, IsPoweredOn = true, State = ServerState.Available },
            new ServerEntity { Id = Guid.NewGuid(), OsName = "Windows Server 2022", RamGb = 32, CpuCores = 16, DiskGb = 1000, IsPoweredOn = false, State = ServerState.Available },
            new ServerEntity { Id = Guid.NewGuid(), OsName = "Debian 12", RamGb = 8, CpuCores = 4, DiskGb = 250, IsPoweredOn = true, State = ServerState.Available }
        );
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }