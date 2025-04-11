using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.ValueContentAnalysis;

namespace FirstProject_ECommerce.Data
{
    public static class RoleSeeder
    {
      

        public static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            { 
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {

                   await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            string adminEmail = "admin@example.com";
            string adminPassword = "Admin@123";
            var adminUser = await UserManager.FindByEmailAsync(adminEmail);

            if(adminUser == null)
            {
                 adminUser = new IdentityUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed=true
                };
                var createAdmin = await UserManager.CreateAsync(adminUser, adminPassword);
                if(createAdmin.Succeeded)
                {
                    await UserManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
     
    }
}
