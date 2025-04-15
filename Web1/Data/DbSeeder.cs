using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Text.Json;
using Web1.Data.Entities;
using Web1.Models.Seeder;

namespace Web1.Data;

public static class DbSeeder
{
    public static async Task SeedData(this WebApplication webApplication) // this - розширяє WebApplication, тобто це розширяючий метод
    {
        //string str = "Ggsd";
        //str.GetCountItems();// підгрузить створений метод

        using var scope = webApplication.Services.CreateScope();
        //цей об'єкт буде вертає посилання на context, який зареєстровано в Program.cs (через builder)
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

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

    //public static void GetCountItems(this String str) // this - розширяє String, тобто ще один метод добавиться до String
    //{

    //}
}
