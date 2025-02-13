using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.Seed;

public static class UserSeed
{
    public static async Task SeedDefaultAdminAsync(this IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<UserEntity>>();

        var admin = await userManager.FindByNameAsync("Admin");
        if (admin is null)
        {
            admin = new UserEntity
            {
                UserName = "Admin",
            };

            await userManager.CreateAsync(admin, "Admin123!");
            var added = await userManager.AddToRoleAsync(admin, "Admin");
            if (!added.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", added.Errors.Select(e => e.Description)));
            }
        }
    }
}