using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


[Authorize(Roles = "Admin")]

public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    // Action to assign the "Admin" role to an existing user with known userId
    public async Task<IActionResult> AssignAdminRole(string userId)
    {
        // Find the user by userId
        var user = await userManager.FindByEmailAsync(userId);

        if (user == null)
        {
            // Handle case when user is not found
            // For example, return a view with an error message
            return RedirectToAction("Index", "Home");
        }

        // Check if the "Admin" role exists
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            // Ensure the "Admin" role exists (this is a safety check)
            var adminRole = new IdentityRole("Admin");
            await roleManager.CreateAsync(adminRole);
        }

        // Assign the "Admin" role to the user
        await userManager.AddToRoleAsync(user, "Admin");

        // Role assignment successful, you can redirect to a success page or perform other actions
        return RedirectToAction("Index", "Home");
    }
}
