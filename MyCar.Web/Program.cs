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

builder.Services.AddMvc().AddRazorPagesOptions(options =>
{
    options.Conventions.AddPageRoute("/Account/Login", "");
    
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
builder.Services.AddSqlServer<MyCar_DBContext>(builder.Configuration.GetConnectionString("Database"));
builder.Services.AddCoreAdmin("�������������");//admin panel
builder.Services.AddCoreAdmin(new CoreAdminOptions() { IgnoreEntityTypes = new List<Type>() { typeof(CharacteristicCar) } });
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

app.UseAuthentication();
app.UseAuthorization();

//app.UseCoreAdminCustomUrl("MyCarAdminPanel");// ������� url ��� ����� ������

app.MapDefaultControllerRoute();// admpnl


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "car",
    pattern: "{controller=Car}/{action=DetailsCarView}/{CarName?}");

app.Run();
