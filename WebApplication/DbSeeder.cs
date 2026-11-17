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

        const string editorRole = "Editor";

        if (!await roleManager.RoleExistsAsync(editorRole))
        {
            await roleManager.CreateAsync(new IdentityRole(editorRole));
        }

        var editorUser =
            await userManager.FindByEmailAsync("editor@websiteapi.com");

        if (editorUser is null)
        {
            editorUser = new User
            {
                UserName = "editor@websiteapi.com",
                Email = "editor@websiteapi.com",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(editorUser, "Editor123!");
        }

        if (!await userManager.IsInRoleAsync(editorUser, editorRole))
        {
            await userManager.AddToRoleAsync(editorUser, editorRole);
        }
    }
}