using AccountMicroservice.Data;
using Microsoft.AspNetCore.Identity;

public static class Utilities
{
    public static void InitializeDbForTests(AppDbContext db)
    {
        // Seed the database with test data.
        // Add test roles
        var roles = new List<IdentityRole>
        {
            new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Name = "User", NormalizedName = "USER" }
        };

        foreach (var role in roles)
        {
            db.Roles.Add(role);
        }

        // Add test users
        var hasher = new PasswordHasher<IdentityUser>();
        var users = new List<IdentityUser>
        {
            new IdentityUser
            {
                UserName = "testuser",
                NormalizedUserName = "TESTUSER",
                Email = "testuser@example.com",
                NormalizedEmail = "TESTUSER@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Test123!"),
                SecurityStamp = string.Empty
            },
            // Add more test users as needed
        };

        foreach (var user in users)
        {
            db.Users.Add(user);
        }

        // Save changes to the in-memory database
        db.SaveChanges();
    }
}
