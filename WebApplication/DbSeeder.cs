using Microsoft.AspNetCore.Identity;

namespace WebsiteAPI;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();

        var userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        const string adminRole = "Admin";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        var adminUser =
            await userManager.FindByEmailAsync("admin@websiteapi.com");

        if (adminUser is null)
        {
            adminUser = new User
            {
                UserName = "admin@websiteapi.com",
                Email = "admin@websiteapi.com",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(adminUser, "Admin123!");
        }

        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}