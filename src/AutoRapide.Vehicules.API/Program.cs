using AutoRapide.Vehicules.API.Data;
using AutoRapide.Vehicules.API.Entities;
using AutoRapide.Vehicules.API.Interfaces;
using AutoRapide.Vehicules.API.Repositories;
using AutoRapide.Vehicules.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<VehiculeContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAsyncRepository<Vehicule>, VehiculeRepository>();
builder.Services.AddScoped<IVehiculeService, VehiculeService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var contexte = scope.ServiceProvider.GetRequiredService<VehiculeContext>();
        InitialiseurBd.Initialiser(contexte, "https://localhost:44314");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Une erreur est survenue lors de l'initialisation de la base de données");
    }
}

app.Run();
