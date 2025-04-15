using Microsoft.EntityFrameworkCore;
using Web1.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseNpgsql(builder.Configuration.GetConnectionString("MyConnection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());// ��������� AutoMapper

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Categories}/{action=Index}/{id?}") // ��� ����������� ��������� ��� ������� ��������
    .WithStaticAssets();

await app.SeedData();

app.Run(); // ������ ����� (������)

// !!!!!!!!!!!!!!!! ��� ��� ������ �� ����� -- �� ��������� �� ���� !!!!!!!!!!!!!!!

