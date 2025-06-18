using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;

using System.Data;
using System.Data.SqlClient;

using WebApp.IRepository;
using WebApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// 加入 Dapper 所需的服務注入
builder.Services.AddScoped<IDbConnection>(sp =>
    new Microsoft.Data.SqlClient.SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 加入你的 Repository（如 CourseRepository）
builder.Services.AddScoped<CourseRepository>();
builder.Services.AddScoped<IFaqRepository, FaqRepository>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

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
app.UseAuthentication();   // ⭐️ 這兩行務必加！
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
