using Microsoft.AspNetCore.Identity;

namespace SandboxAuthentication;

public static class AuthInitializer
{
    public static async Task Initialize(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        var roles = new[] { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminUser = new IdentityUser { UserName = "Admin", Email = "admin@admins.com" };
        if (await userManager.FindByNameAsync(adminUser.UserName) == null)
        {
            var result = await userManager.CreateAsync(adminUser, "Admin123@");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "User");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        var basicUser = new IdentityUser { UserName = "basicUser", Email = "basicUser@users.com" };
        if (await userManager.FindByNameAsync(basicUser.UserName) == null)
        {
            var result = await userManager.CreateAsync(basicUser, "User123@");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(basicUser, "User");
            }
        }
    }
}