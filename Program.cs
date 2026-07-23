using System;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Ajout des services au conteneur d'injection de dépendances.
builder.Services.AddControllersWithViews();

// Configuration de l'authentification par Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

// Ajout de la gestion des sessions
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// S'assurer que le dossier d'images existe et copier accueil.png si présent
try
{
    var webRoot = app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
    var imagesDir = Path.Combine(webRoot, "images");
    if (!Directory.Exists(imagesDir))
    {
        Directory.CreateDirectory(imagesDir);
    }
    var sourceImagePath = Path.Combine(app.Environment.ContentRootPath, "accueil.png");
    var destImagePath = Path.Combine(imagesDir, "accueil.png");
    if (File.Exists(sourceImagePath))
    {
        File.Copy(sourceImagePath, destImagePath, true);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Erreur lors de la copie des images : {ex.Message}");
}

// Configuration du pipeline de requêtes HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
