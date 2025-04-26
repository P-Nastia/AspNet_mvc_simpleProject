using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Text.Json;
using Web1.Data.Entities;
using Web1.Interfaces;
using Web1.Models.Seeder;

namespace Web1.Data;

public static class DbSeeder
{
    public static async Task SeedData(this WebApplication webApplication)  // this - розширяє WebApplication, тобто це розширяючий метод
    {
        //string str = "Ggsd";
        //str.GetCountItems();// підгрузить створений метод

        using var scope = webApplication.Services.CreateScope();
        //цей об'єкт буде вертає посилання на context, який зареєстровано в Program.cs (через builder)

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        var imageService = scope.ServiceProvider.GetRequiredService<IImageService>();

        await SeedCategories(context, mapper, imageService);
        await SeedUsers(context, mapper, imageService);
    }
    private static async Task SeedCategories(AppDbContext context,IMapper mapper,IImageService imageService)
    {
        context.Database.Migrate();

        if (!context.Categories.Any())
        {
            // записати дані, якщо немає даних в БД
            var jsonFile = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "JsonData", "Categories.json");

            if (File.Exists(jsonFile))
            {
                var jsonData = await File.ReadAllTextAsync(jsonFile);
                try
                {
                    var categories = JsonSerializer.Deserialize<List<SeederCategoryModel>>(jsonData);
                    var categoryEntities = mapper.Map<List<CategoryEntity>>(categories);
                    await context.AddRangeAsync(categoryEntities);
                    await context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error Json Parse Data", ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Not found file Categories.json");
            }
        }
    }

    private static async Task SeedUsers(AppDbContext context, IMapper mapper,IImageService imageService)
    {
        context.Database.Migrate();

        if (!context.Users.Any())
        {
            
            var jsonFile = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "JsonData", "Users.json");

            if (File.Exists(jsonFile))
            {
                var jsonData = await File.ReadAllTextAsync(jsonFile);
                try
                {
                    var users = JsonSerializer.Deserialize<List<SeederUserModel>>(jsonData);
                    var userEntities = mapper.Map<List<UserEntity>>(users);
                    var startPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pictures");
                    for(int i = 0; i < users.Count; i++)
                    {
                        userEntities[i].ImageUrl = await imageService.SaveImageAsync(FormFileFromPath($"{startPath}\\{users[i].Image}"));
                    }
                    await context.AddRangeAsync(userEntities);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Json Parse Data", ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Not found file Users.json");
            }
        }

    }

    private static IFormFile FormFileFromPath(string filePath)
    {
        var fileBytes = File.ReadAllBytes(filePath);
        var stream = new MemoryStream(fileBytes);
        return new FormFile(stream, 0, stream.Length, "file", Path.GetFileName(filePath));
    }

    //public static void GetCountItems(this String str) // this - розширяє String, тобто ще один метод добавиться до String
    //{

    //}
}
