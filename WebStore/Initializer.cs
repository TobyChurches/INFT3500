using Microsoft.AspNetCore.Identity;
using WebStore.Controllers;
using WebStore.Models;

namespace WebStore;

public static class Initializer
{
    public static async Task RoleInitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "Admin", "Employee", "Customer" };
        foreach (var roleName in roleNames)
        {
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    public static async Task UserInitializeAsync(UserManager<User> userManager)
    {

        // Customer Credentials
        if (await userManager.FindByEmailAsync("customer@example.com") == null)
        {
            var salt = AuthenticationController.GenerateSalt();
            var user = new User()
            {
                UserName = "CusTomer",
                Email = "customer@example.com",
                Name = "Cus Tomer",
                IsAdmin = false,
                IsEmployee = false,
                Salt = salt,
                HashPw = AuthenticationController.GenerateHash("Password1", salt),
                NormalizedEmail = userManager.NormalizeEmail("customer@example.com"),
                NormalizedUserName = userManager.NormalizeName("CusTomer"),
            };

            await userManager.CreateAsync(user, user.HashPw);
            await userManager.AddToRoleAsync(user, "Customer");
        }

        // Employee Credentials
        if (await userManager.FindByEmailAsync("employee@example.com") == null)
        {
            var salt = AuthenticationController.GenerateSalt();
            var user = new User()
            {
                UserName = "EmplOyee",
                Email = "employee@example.com",
                Name = "Empl Oyee",
                IsAdmin = false,
                IsEmployee = true,
                Salt = salt,
                HashPw = AuthenticationController.GenerateHash("Passw0rd", salt),
                NormalizedEmail = userManager.NormalizeEmail("employee@example.com"),
                NormalizedUserName = userManager.NormalizeName("EmplOyee"),
            };

            await userManager.CreateAsync(user, user.HashPw);
            await userManager.AddToRoleAsync(user, "Employee");
        }


        // Admin Credentials
        if (await userManager.FindByEmailAsync("administrator@example.com") == null)
        {
            var salt = AuthenticationController.GenerateSalt();
            var user = new User()
            {
                UserName = "AdminIstrator",
                Email = "administrator@example.com",
                Name = "Admin Istrator",
                IsAdmin = true,
                IsEmployee = false,
                Salt = salt,
                HashPw = AuthenticationController.GenerateHash("Pa$$w0rd", salt),
                NormalizedEmail = userManager.NormalizeEmail("administrator@example.com"),
                NormalizedUserName = userManager.NormalizeName("AdminIstrator"),
            };

            await userManager.CreateAsync(user, user.HashPw);
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
