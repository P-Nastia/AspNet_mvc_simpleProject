using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Web1.Data;
using Web1.Interfaces;
using Web1.Services;
using Web1.Data.Entities.Identity;
using Web1.Areas.Admin.Mapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseNpgsql(builder.Configuration.GetConnectionString("MyConnection")));


//������ ������������ ��� UserManager � RoleManager � SignInManager -- ��������� cookies

builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    //��� ���� ���� �� ���� ������
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//////////////////////////////

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());// ��������� AutoMapper
builder.Services.AddAutoMapper(typeof(UserMapper));
builder.Services.AddScoped<IImageService, ImageService>();

// ������ View - �������, �� ����� ������ �� c# (exp - Index.cshtml)
// ����� -- ������������ �� c# � ����������� � �����
// Web1/(����� �����).dll - �������� ���� ������
// ��������� - �� ���� �� c#, ���� ������ ������ �� �볺��� � ������ ��� ����� ������
// ���������� ������ (Model) ���������� ����������� �� view ��� �����������
builder.Services.AddControllersWithViews(); // ������������ ����������, ������, ���������.

var app = builder.Build(); // ����������� ����� �� ����� ����� ����������� ����

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");  // ���� ������ �������, �� ��� ���� �� ������� /Home/Error
}
app.UseRouting(); // �������� ������������� - ���� �� ������ ������ � url(��������) localhost:2334/... - ����� ���� �� �������

app.UseAuthorization(); // �������� ����������� - ��������� �� Identity

app.MapStaticAssets(); // ������������ ��������� �����, ����� ���� ��������� ����� wwwroot

// ������������ ��� ��������. � ���������� -- ��������� HomeController, ��� ����� ����������� ���� Home
// ��� ����� ������ ����� ����� ����������� Action - ���������
// ��� ����, ���� ��� ������� ����� �� ������ ����, ���� ���������� ����� ���������� HomeController
// � ���� ����� index, ��� ����� ���� ���� �������� � ������� id - ��� ��� � ���� ������� (?) - ����� ���� ���� null

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Categories}/{action=Index}/{id?}") // ��� ����������� ��������� ��� ������� ��������
//    .WithStaticAssets();

app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
      name: "admin_area",
      areaName: "Admin",
      pattern: "admin/{controller=Home}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Categories}/{action=Index}/{id?}"
    );
});

var dir = builder.Configuration["ImagesDir"];
string path = Path.Combine(Directory.GetCurrentDirectory(), dir);
Directory.CreateDirectory(path);

app.UseStaticFiles(new StaticFileOptions // ������� ������� �� ����� � ����
{
    FileProvider = new PhysicalFileProvider(path),
    RequestPath = $"/{dir}" // ���� ����������
});


await app.SeedData();

app.Run(); // ������ ����� (������)

// !!!!!!!!!!!!!!!! ��� ��� ������ �� ����� -- �� ��������� �� ���� !!!!!!!!!!!!!!!

