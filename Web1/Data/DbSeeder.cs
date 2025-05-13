using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Text.Json;
using Web1.Data.Entities;
using Web1.Interfaces;
using Web1.Models.Seeder;
using Web1.Constants;
using Web1.Data.Entities.Identity;
using Web1.SMTP;

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
        var smtpService = scope.ServiceProvider.GetRequiredService<ISMTPService>();
        //webApplication.Use(async (context, next) =>
        //{
        //    var host = context.Request.Host.Host;

        //    Message msgEmail = new Message
        //    {
        //        Body = $"Додаток успішно запущено {DateTime.Now}",
        //        Subject = $"Запуск сайту {host}",
        //        To="nastyapivza1999@gmail.com"
        //    };
        //    Console.WriteLine(host);
        //    await smtpService.SendMessage(msgEmail);
        //    await next.Invoke();
        //});

        context.Database.Migrate();

        await SeedCategories(context, mapper, imageService);
        await SeedRoles(scope, context, mapper, imageService);
        await SeedUsers(scope,context, mapper, imageService);

        if (!context.Products.Any())
        {
            var jsonFile = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "JsonData", "Products.json");

            if (File.Exists(jsonFile))
            {
                var jsonData = await File.ReadAllTextAsync(jsonFile);
                try
                {
                    var products = JsonSerializer.Deserialize<List<SeederProductModel>>(jsonData);

                    foreach (var product in products)
                    {
                        // Знайти відповідну категорію
                        var category = await context.Categories
                            .FirstOrDefaultAsync(c => c.Name == product.CategoryName);

                        if (category == null)
                        {
                            Console.WriteLine($"Category '{product.CategoryName}' not found for product '{product.Name}'");
                            continue;
                        }

                        var productEntity = new ProductEntity
                        {
                            Name = product.Name,
                            Description = product.Description,
                            CategoryId = category.Id,
                            ProductImages = new List<ProductImageEntity>()
                        };

                        int priority = 0;
                        try
                        {
                            foreach (var imageUrl in product.Images)
                            {
                                var savedImageUrl = await imageService.SaveImageFromUrlAsync(imageUrl);
                                productEntity.ProductImages.Add(new ProductImageEntity
                                {
                                    Name = savedImageUrl,
                                    Priotity = priority++
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("------------Error Add Product {0}", ex.Message);
                            continue;
                        }

                        await context.Products.AddAsync(productEntity);
                    }

                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Json Parse Product Data: {0}", ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Products.json file not found");
            }
        }

        

        
    }
    private static async Task SeedRoles(IServiceScope scope,AppDbContext context, IMapper mapper, IImageService imageService)
    {
        if (!context.Roles.Any())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();
            var admin = new RoleEntity { Name = Roles.Admin };

            var result = await roleManager.CreateAsync(admin);

            if (result.Succeeded)
            {
                Console.WriteLine($"Роль {Roles.Admin} створено успішно");
            }
            else
            {
                Console.WriteLine($"Помилка створення ролі:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"- {error.Code}: {error.Description}");
                }
            }

            var user = new RoleEntity { Name = Roles.User };
            result = await roleManager.CreateAsync(user);

            if (result.Succeeded)
            {
                Console.WriteLine($"Роль {Roles.Admin} створено успішно");
            }
            else
            {
                Console.WriteLine($"Помилка створення ролі:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"- {error.Code}: {error.Description}");
                }
            }
        }
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
                    var startPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pictures");
                    for (int i = 0; i < categories.Count; i++)
                    {
                        categoryEntities[i].ImageUrl = await imageService.SaveImageAsync(FormFileFromPath($"{startPath}\\{categories[i].Image}"));
                    }
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

    private static async Task SeedUsers(IServiceScope scope, AppDbContext context, IMapper mapper,IImageService imageService)
    {
        
        if (!context.Users.Any())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();

            string email = "admin@gmail.com";
            var user = new UserEntity
            {
                UserName = email,
                Email = email,
                LastName = "Марко",
                FirstName = "Онутрій",
                Image = "j0ieqvrx.m3a"
            };

            var result = await userManager.CreateAsync(user, "123456");
            if (result.Succeeded)
            {
                Console.WriteLine($"Користувача успішно створено {user.LastName} {user.FirstName}!");
                await userManager.AddToRoleAsync(user, Roles.Admin);
            }
            else
            {
                Console.WriteLine($"Помилка створення користувача:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"- {error.Code}: {error.Description}");
                }
            }


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
                        userEntities[i].Image = await imageService.SaveImageAsync(FormFileFromPath($"{startPath}\\{users[i].Image}"));

                        result = await userManager.CreateAsync(userEntities[i], users[i].Password);
                        if (result.Succeeded)
                        {
                            Console.WriteLine($"Користувача успішно створено {userEntities[i].LastName} {userEntities[i].FirstName}!");
                            await userManager.AddToRoleAsync(userEntities[i], Roles.User);
                        }
                        else
                        {
                            Console.WriteLine($"Помилка створення користувача:");
                            foreach (var error in result.Errors)
                            {
                                Console.WriteLine($"- {error.Code}: {error.Description}");
                            }
                        }
                    }
                    
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
