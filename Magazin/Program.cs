using System.Net.Http.Headers;
using Magazin.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Magazin.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("BookMagazinApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7176");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});


// Настройка аутентификации через куки
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Autorization"; 
        options.AccessDeniedPath = "/Error/AccessDenied"; 
        options.LogoutPath = "/Account/Logout";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.HttpOnly = true;
    });


// Настройка авторизации с политиками ролей
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CustomerPolicy", policy => policy.RequireRole("Пользователь"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Админ"));
});

// Регистрируем PasswordHasher для User
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// Регистрируем API-сервисы
builder.Services.AddTransient<BookApiService>();
builder.Services.AddTransient<ReviewApiService>();
builder.Services.AddTransient<AdaptationApiService>();
builder.Services.AddTransient<UserApiService>();
builder.Services.AddTransient<GenreApiService>();
builder.Services.AddTransient<AuthorApiService>();
builder.Services.AddTransient<OrderItemApiService>();
builder.Services.AddTransient<RoleApiService>();
builder.Services.AddTransient<BookVoicesApiService>();

// Отдельный HttpClient для OrderApiService
builder.Services.AddHttpClient<OrderApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7176/api/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});




builder.Services.AddHttpContextAccessor();

// Добавляем поддержку контроллеров с представлениями
builder.Services.AddControllersWithViews();


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
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(@"C:\BookAudio"),
    RequestPath = "/audio",
    ServeUnknownFileTypes = true
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
