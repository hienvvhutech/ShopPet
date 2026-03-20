using Microsoft.AspNetCore.Identity;
using Shopping_Pet.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Shopping_Pet.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<Role> roleManager,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("Ensuring database is ready...");
                await context.Database.EnsureCreatedAsync(); // Đảm bảo database sẵn sàng
                logger.LogInformation("Database is ready.");

                // 1. Tạo các vai trò nếu chưa tồn tại
                logger.LogInformation("Seeding roles...");
                string[] roles = { "Admin", "Customer", "Employee" };
                foreach (var roleName in roles)
                {
                    logger.LogInformation("Checking if role {RoleName} exists", roleName);
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        logger.LogInformation("Creating role {RoleName}", roleName);
                        var role = new Role
                        {
                            Id = Guid.NewGuid().ToString(), // Gán Id cho Role
                            Name = roleName,
                            NormalizedName = roleName.ToUpper(),
                            Description = $"{roleName} role"
                        };
                        var result = await roleManager.CreateAsync(role);
                        if (!result.Succeeded)
                        {
                            logger.LogError("Failed to create role {RoleName}: {Errors}", roleName, string.Join("; ", result.Errors.Select(e => e.Description)));
                            throw new Exception($"Loi tao vai tro {roleName}: {string.Join("; ", result.Errors.Select(e => e.Description))}");
                        }
                        logger.LogInformation("Created role {RoleName} successfully", roleName);
                    }
                    else
                    {
                        logger.LogInformation("Role {RoleName} already exists", roleName);
                    }
                }

                // 2. Tạo tài khoản Admin mặc định
                logger.LogInformation("Seeding admin user...");
                string adminEmail = "admin@shop.com";
                string adminPassword = "Admin@123";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString(), // Gán Id cho ApplicationUser
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "Admin Root",
                        CreatedAt = DateTime.UtcNow,
                        EmailConfirmed = true
                    };

                    logger.LogInformation("Creating admin user {Email}", adminEmail);
                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (!result.Succeeded)
                    {
                        logger.LogError("Failed to create admin user {Email}: {Errors}", adminEmail, string.Join("; ", result.Errors.Select(e => e.Description)));
                        throw new Exception($"Loi tao Admin {adminEmail}: {string.Join("; ", result.Errors.Select(e => e.Description))}");
                    }
                    logger.LogInformation("Created admin user {Email} successfully", adminEmail);
                }
                else
                {
                    logger.LogInformation("Admin user {Email} already exists", adminEmail);
                }

                // 3. Gán role Admin nếu chưa có
                logger.LogInformation("Checking roles for admin user {Email}", adminEmail);
                var rolesOfUser = await userManager.GetRolesAsync(adminUser);
                if (!rolesOfUser.Contains("Admin"))
                {
                    logger.LogInformation("Assigning 'Admin' role to user {Email}", adminEmail);
                    var result = await userManager.AddToRoleAsync(adminUser, "Admin");
                    if (!result.Succeeded)
                    {
                        logger.LogError("Failed to assign Admin role to {Email}: {Errors}", adminEmail, string.Join("; ", result.Errors.Select(e => e.Description)));
                        throw new Exception($"Loi gan vai tro Admin cho {adminEmail}: {string.Join("; ", result.Errors.Select(e => e.Description))}");
                    }
                    logger.LogInformation("Assigned 'Admin' role to user {Email} successfully", adminEmail);
                }
                else
                {
                    logger.LogInformation("User {Email} already has 'Admin' role", adminEmail);
                }

                logger.LogInformation("Seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during seeding");
                throw new Exception($"Loi khi seeding du lieu: {ex.Message}", ex);
            }
        }
    }
}