using System.Reflection;
using AutoRapide.Fichiers.API.Interfaces;
using AutoRapide.Fichiers.API.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IStorageService, FileSystemStorageService>();
builder.Services.AddTransient<IFileValidationService, FileValidationService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "AutoRapide Fichiers API",
        Description = "Une API pour permettre de stocker et accéder à des fichiers. Permet d'accéder à des fichiers multimédias seulement (images, vidéos et audio).",
        Contact = new OpenApiContact
        {
            Name = "Pier-Olivier St-Pierre-Chouinard",
            Url = new Uri("https://github.com/polivierstpch"),
            Email = "pier.olivier.stpch@gmail.com"
        }
        
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

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

app.Run();
