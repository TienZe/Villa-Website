using Microsoft.AspNetCore.Identity;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaUtility;

namespace VillaAPI.Infrastructure;

public class SeedData
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public SeedData(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task SeedAdminAccount()
    {
        var admin = await _userManager.FindByNameAsync("admin");
        if (admin is null) {
            var defaultAdmin = new ApplicationUser {
                UserName = "admin",
                Name = "Admin",
            };

            var result = await _userManager.CreateAsync(defaultAdmin, "111111");
            var add2RoleResult = await _userManager.AddToRoleAsync(defaultAdmin, SD.Role.Admin);

            // Handle error ...
        }
    }

    public async Task SeedRoles()
    {
        if (!_context.Roles.Any()) {
            await _context.Roles.AddRangeAsync(
                new IdentityRole { Name = SD.Role.Admin, NormalizedName = SD.Role.Admin.ToUpper() },
                new IdentityRole { Name = SD.Role.User, NormalizedName = SD.Role.User.ToUpper() }
            );

            await _context.SaveChangesAsync();
        }
    }

    public async Task SeedExampleData()
    {
        if (!_context.Villas.Any()) {
            await _context.Villas.AddRangeAsync(
                new Villa
                {
                    Name = "Royal Villa",
                    Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://dotnetmastery.com/bluevillaimages/villa3.jpg",
                    Occupancy = 4,
                    Rate = 200,
                    Sqft = 550,
                    Amenity = "",
                    CreatedDate = DateTime.Now
                },
                new Villa
                {
                    Name = "Premium Pool Villa",
                    Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://dotnetmastery.com/bluevillaimages/villa1.jpg",
                    Occupancy = 4,
                    Rate = 300,
                    Sqft = 550,
                    Amenity = "",
                    CreatedDate = DateTime.Now
                },
                new Villa
                {
                    Name = "Luxury Pool Villa",
                    Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://dotnetmastery.com/bluevillaimages/villa4.jpg",
                    Occupancy = 4,
                    Rate = 400,
                    Sqft = 750,
                    Amenity = "",
                    CreatedDate = DateTime.Now
                },
                new Villa
                {
                    Name = "Diamond Villa",
                    Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://dotnetmastery.com/bluevillaimages/villa5.jpg",
                    Occupancy = 4,
                    Rate = 550,
                    Sqft = 900,
                    Amenity = "",
                    CreatedDate = DateTime.Now
                },
                new Villa
                {
                    Name = "Diamond Pool Villa",
                    Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://dotnetmastery.com/bluevillaimages/villa2.jpg",
                    Occupancy = 4,
                    Rate = 600,
                    Sqft = 1100,
                    Amenity = "",
                    CreatedDate = DateTime.Now
                }
            );

            await _context.SaveChangesAsync();
        }
    }
}