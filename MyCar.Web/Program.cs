using DotNetEd.CoreAdmin;
using Microsoft.AspNetCore.Authentication.Cookies;
using MyCar.Server.DB;
using SmartBreadcrumbs.Extensions;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
    options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
});

//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromSeconds(10);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

builder.Services.AddMvc().AddRazorPagesOptions(options =>
{
    options.Conventions.AddPageRoute("/Account/Login", "");
    
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddBreadcrumbs(Assembly.GetExecutingAssembly(), options =>
{
    options.TagName = "nav";
    options.TagClasses = "";
    options.OlClasses = "breadcrumb";
    options.LiClasses = "breadcrumb-item";
    options.ActiveLiClasses = "breadcrumb-item active";
    options.DontLookForDefaultNode = true;
});
builder.Services.AddSession();
builder.Services.AddSqlServer<MyCar_DBContext>(builder.Configuration.GetConnectionString("Database"));
builder.Services.AddCoreAdmin("Администратор");//admin panel
//builder.Services.AddCoreAdmin(new CoreAdminOptions() { IgnoreEntityTypes = new List<Type>() { typeof(CharacteristicCar) } });
// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddControllers().AddNewtonsoftJson(options =>
//    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
//);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

//app.UseSession();
//app.UseCoreAdminCustomUrl("MyCarAdminPanel");// Задание url для админ панели

app.MapDefaultControllerRoute();// admpnl


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "car",
    pattern: "{controller=Car}/{action=DetailsCarView}/{id?}");


app.Run();
