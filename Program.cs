using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// app.Use(async (context, next) =>
// {
//     if (context.User.Identity?.IsAuthenticated != true)
//     {
//         var claims = new List<System.Security.Claims.Claim>
//         {
//             new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "1"),
//             new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "Adama Diarra"),
//             new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.MobilePhone, "76000000"),
//             new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin")
//         };
//         var identity = new System.Security.Claims.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//         context.User = new System.Security.Claims.ClaimsPrincipal(identity);
//     }
//     await next();
// });

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
