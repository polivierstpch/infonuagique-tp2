using AutoRapide.MVC.Interfaces;
using AutoRapide.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddHttpClient<IUsagerService, UtilisateursServiceProxy>(client => 
    client.BaseAddress = new Uri(configuration.GetValue<string>("UrlUsagerAPI")));
builder.Services.AddHttpClient<ICommandesService, CommandesServiceProxy>(client => 
    client.BaseAddress = new Uri(configuration.GetValue<string>("UrlCommandesAPI")));
builder.Services.AddHttpClient<IFavorisService, FavorisServiceProxy>(client => 
    client.BaseAddress = new Uri(configuration.GetValue<string>("UrlFavorisAPI")));

builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
