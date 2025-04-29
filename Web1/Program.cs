using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Web1.Data;
using Web1.Interfaces;
using Web1.Services;
using Web1.Data.Entities.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseNpgsql(builder.Configuration.GetConnectionString("MyConnection")));


//додаємо налаштування для UserManager і RoleManager і SignInManager -- займається cookies

builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    //тут опис який має бути пароль
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//////////////////////////////

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());// реєстрація AutoMapper
builder.Services.AddScoped<IImageService, ImageService>();

// будуть View - сторінки, де можна писати на c# (exp - Index.cshtml)
// фішка -- перевіряються на c# і компілюються у збірку
// Web1/(назва файлу).dll - вихідний файл проєкту
// контролер - це клас на c#, який приймає запити від клієнта і виконує всю логіку роботи
// результати роботи (Model) контролера передаються на view для відображення
builder.Services.AddControllersWithViews(); // налаштування контейнерів, сервісів, репозиторі.

var app = builder.Build(); // створюється збірка на основі даних налаштувань вище

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");  // якщо виникає помилка, то нас кидає на сторінку /Home/Error
}
app.UseRouting(); // підтримка маршрутизації - коли ми можемо писати в url(посиланні) localhost:2334/... - тобто шлях до сторінки

app.UseAuthorization(); // підтримка авторизації - вчитимемо на Identity

app.MapStaticAssets(); // використання статичних файлів, тобто буде працювати папка wwwroot

// налаштування для маршрутів. Є контролери -- наприклад HomeController, при цьому враховується лише Home
// при цьому методи цього класу називаються Action - обробники
// для того, щоби при запуску сайту ми бачили щось, воно визивається згідно налатувань HomeController
// і його метод index, при цьому може бути параметр у маршруті id - але там є знак питання (?) - тобто може бути null

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Categories}/{action=Index}/{id?}") // тут викликається контролер при запуску програми
    .WithStaticAssets();

var dir = builder.Configuration["ImagesDir"];
string path = Path.Combine(Directory.GetCurrentDirectory(), dir);
Directory.CreateDirectory(path);

app.UseStaticFiles(new StaticFileOptions // надання доступу до папки з фото
{
    FileProvider = new PhysicalFileProvider(path),
    RequestPath = $"/{dir}" // куди звертатися
});


await app.SeedData();

app.Run(); // запуск хосту (сервер)

// !!!!!!!!!!!!!!!! тут код писати не можна -- він працювати не буде !!!!!!!!!!!!!!!

