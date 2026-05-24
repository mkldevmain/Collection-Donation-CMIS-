using CMIS.Data;
using CMIS.Models;
using CMIS.Services;
using Microsoft.EntityFrameworkCore;

namespace CMIS.Data;

public static class DatabaseSeeder
{
    /// <summary>
    /// Seeds initial data: Roles, a District, a Church, a Profile, and a test Account.
    /// If data already exists, migrates passwords to BCrypt format.
    /// </summary>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.Roles.AnyAsync())
        {
            // Data already exists — migrate passwords and ensure reference data
            await MigratePasswordsToBCryptAsync(db);
            await EnsureTestAccountAsync(db);
            await EnsureSampleDistrictsAsync(db);
            await EnsureSampleChurchesAsync(db);
            await EnsureAllRoleAccountsAsync(db);
            return;
        }

        // 1. Seed Roles
        var roles = new List<Role>
        {
            new() { RoleName = "Head Pastor", Description = "Senior pastor overseeing all church operations" },
            new() { RoleName = "District Head", Description = "Oversees a district of churches" },
            new() { RoleName = "Ministry Head", Description = "Leads a specific ministry" },
            new() { RoleName = "Board of Directors", Description = "Board member with governance authority" },
            new() { RoleName = "Leadership Council", Description = "Member of the leadership council" },
            new() { RoleName = "Member", Description = "Regular church member" },
        };
        db.Roles.AddRange(roles);
        await db.SaveChangesAsync();

        // 2. Seed a District
        var district = new District
        {
            DistrictName = "Metro Manila District",
            DistrictCode = "MMD-001",
            Address = "Metro Manila, Philippines",
            Status = "Active"
        };
        db.Districts.Add(district);
        await db.SaveChangesAsync();

        // 3. Seed a Church
        var church = new Church
        {
            DistrictId = district.DistrictId,
            ChurchName = "Jesus Our Banner Ministry - Main",
            Address = "Manila, Philippines",
            ContactNumber = "09171234567",
            Status = "Active"
        };
        db.Churches.Add(church);
        await db.SaveChangesAsync();

        // 4. Seed a Profile
        var profile = new Profile
        {
            ChurchId = church.ChurchId,
            FirstName = "Admin",
            MiddleName = null,
            LastName = "User",
            Sex = "Male",
            BirthDate = new DateTime(1990, 1, 1),
            ContactNumber = "09171234567",
            Address = "Manila, Philippines",
            ProfileStatus = "Active"
        };
        db.Profiles.Add(profile);
        await db.SaveChangesAsync();

        // 5. Seed a test Account (Head Pastor role)
        var headPastorRole = roles.First(r => r.RoleName == "Head Pastor");
        var account = new Account
        {
            ProfileId = profile.ProfileId,
            RoleId = headPastorRole.RoleId,
            ChurchId = church.ChurchId,
            Username = "admin",
            Email = "admin@jobm.org",
            PasswordHash = AuthService.HashPassword("Admin@123"),
            Status = "Active"
        };
        db.Accounts.Add(account);
        await db.SaveChangesAsync();

        // 6. Seed additional districts and churches
        await EnsureSampleDistrictsAsync(db);
        await EnsureSampleChurchesAsync(db);

        // 7. Seed all role-based accounts
        await EnsureAllRoleAccountsAsync(db);
    }

    /// <summary>
    /// Finds existing accounts whose password_hash is NOT in BCrypt format
    /// and re-hashes the current value as the "plain text" password.
    /// 
    /// BCrypt hashes always start with "$2a$", "$2b$", or "$2y$".
    /// If the stored value doesn't match this pattern, it's treated as a
    /// plain-text (or non-BCrypt) password and gets re-hashed.
    /// </summary>
    private static async Task MigratePasswordsToBCryptAsync(ApplicationDbContext db)
    {
        var accounts = await db.Accounts.ToListAsync();
        var migrated = 0;

        foreach (var account in accounts)
        {
            if (!string.IsNullOrEmpty(account.PasswordHash) &&
                !account.PasswordHash.StartsWith("$2"))
            {
                // Current value is plain text or a non-BCrypt hash — re-hash it
                account.PasswordHash = AuthService.HashPassword(account.PasswordHash);
                migrated++;
            }
        }

        if (migrated > 0)
        {
            await db.SaveChangesAsync();
            Console.WriteLine($"[Seeder] Migrated {migrated} account password(s) to BCrypt.");
        }
    }

    /// <summary>
    /// Ensures a test admin account exists with known credentials.
    /// Email: admin@jobm.org | Username: admin | Password: Admin@123
    /// </summary>
    private static async Task EnsureTestAccountAsync(ApplicationDbContext db)
    {
        // Skip if account already exists
        if (await db.Accounts.AnyAsync(a => a.Email == "admin@jobm.org"))
            return;

        // Get or create the Head Pastor role
        var role = await db.Roles.FirstOrDefaultAsync(r => r.RoleName == "Head Pastor");
        if (role is null)
        {
            role = new Role { RoleName = "Head Pastor", Description = "Senior pastor overseeing all church operations" };
            db.Roles.Add(role);
            await db.SaveChangesAsync();
        }

        // Get or create a district
        var district = await db.Districts.FirstOrDefaultAsync();
        if (district is null)
        {
            district = new District
            {
                DistrictName = "Default District",
                DistrictCode = "DEF-001",
                Address = "Philippines",
                Status = "Active"
            };
            db.Districts.Add(district);
            await db.SaveChangesAsync();
        }

        // Get or create a church
        var church = await db.Churches.FirstOrDefaultAsync();
        if (church is null)
        {
            church = new Church
            {
                DistrictId = district.DistrictId,
                ChurchName = "Jesus Our Banner Ministry - Main",
                Address = "Philippines",
                ContactNumber = "09170000000",
                Status = "Active"
            };
            db.Churches.Add(church);
            await db.SaveChangesAsync();
        }

        // Create a profile for the admin
        var profile = new Profile
        {
            ChurchId = church.ChurchId,
            FirstName = "Admin",
            LastName = "User",
            Sex = "Male",
            BirthDate = new DateTime(1990, 1, 1),
            ProfileStatus = "Active"
        };
        db.Profiles.Add(profile);
        await db.SaveChangesAsync();

        // Create the account
        var account = new Account
        {
            ProfileId = profile.ProfileId,
            RoleId = role.RoleId,
            ChurchId = church.ChurchId,
            Username = "admin",
            Email = "admin@jobm.org",
            PasswordHash = AuthService.HashPassword("Admin@123"),
            Status = "Active"
        };
        db.Accounts.Add(account);
        await db.SaveChangesAsync();

        Console.WriteLine("[Seeder] Test account created — Email: admin@jobm.org | Password: Admin@123");
    }

    /// <summary>
    /// Seeds 5 accounts — one for each primary role.
    /// All use password: Password@123
    /// </summary>
    private static async Task EnsureAllRoleAccountsAsync(ApplicationDbContext db)
    {
        var church = await db.Churches.FirstOrDefaultAsync();
        if (church is null) return;

        var seedAccounts = new[]
        {
            new { Email = "headpastor@jobm.org", Username = "headpastor", FirstName = "Juan", LastName = "Dela Cruz", Role = "Head Pastor" },
            new { Email = "districthead@jobm.org", Username = "districthead", FirstName = "Maria", LastName = "Santos", Role = "District Head" },
            new { Email = "ministryhead@jobm.org", Username = "ministryhead", FirstName = "Pedro", LastName = "Reyes", Role = "Ministry Head" },
            new { Email = "board@jobm.org", Username = "board", FirstName = "Elena", LastName = "Garcia", Role = "Board of Directors" },
            new { Email = "council@jobm.org", Username = "council", FirstName = "Carlos", LastName = "Mendoza", Role = "Leadership Council" },
        };

        var created = 0;
        foreach (var seed in seedAccounts)
        {
            // Skip if account already exists (by email or username)
            if (await db.Accounts.AnyAsync(a => a.Email == seed.Email || a.Username == seed.Username))
                continue;

            var role = await db.Roles.FirstOrDefaultAsync(r => r.RoleName == seed.Role);
            if (role is null) continue;

            var profile = new Profile
            {
                ChurchId = church.ChurchId,
                FirstName = seed.FirstName,
                LastName = seed.LastName,
                Sex = "Male",
                BirthDate = new DateTime(1985, 6, 15),
                ContactNumber = "09170000000",
                Address = "Manila, Philippines",
                ProfileStatus = "Active"
            };
            db.Profiles.Add(profile);
            await db.SaveChangesAsync();

            var account = new Account
            {
                ProfileId = profile.ProfileId,
                RoleId = role.RoleId,
                ChurchId = church.ChurchId,
                Username = seed.Username,
                Email = seed.Email,
                PasswordHash = AuthService.HashPassword("Password@123"),
                Status = "Active"
            };
            db.Accounts.Add(account);
            await db.SaveChangesAsync();
            created++;
        }

        if (created > 0)
            Console.WriteLine($"[Seeder] {created} role-based accounts created. Password for all: Password@123");
    }

    private static async Task EnsureSampleDistrictsAsync(ApplicationDbContext db)
    {
        var districtSeeds = new[]
        {
            new { Name = "Metro Manila District",        Code = "MMD-001", Address = "Metro Manila, Philippines" },
            new { Name = "North Luzon District",         Code = "NLD-001", Address = "Pampanga, Philippines" },
            new { Name = "South Luzon & Bicol District", Code = "SLB-001", Address = "Naga City, Philippines" },
            new { Name = "Visayas District",             Code = "VSD-001", Address = "Cebu City, Philippines" },
        };

        var added = 0;
        foreach (var d in districtSeeds)
        {
            if (!await db.Districts.AnyAsync(x => x.DistrictCode == d.Code))
            {
                db.Districts.Add(new District { DistrictName = d.Name, DistrictCode = d.Code, Address = d.Address, Status = "Active" });
                added++;
            }
        }

        if (added > 0)
        {
            await db.SaveChangesAsync();
            Console.WriteLine($"[Seeder] {added} district(s) created.");
        }
    }

    private static async Task EnsureSampleChurchesAsync(ApplicationDbContext db)
    {
        var districts = await db.Districts.ToListAsync();
        int? IdOf(string code) => districts.FirstOrDefault(d => d.DistrictCode == code)?.DistrictId;

        var churchSeeds = new[]
        {
            // Metro Manila District
            new { Name = "JOBM Main",                    DistrictCode = "MMD-001", Contact = "09171234567", Address = "Manila, Philippines" },
            new { Name = "JOBM Quezon City Branch",      DistrictCode = "MMD-001", Contact = "09172345678", Address = "Quezon City, Philippines" },
            new { Name = "JOBM Makati Branch",           DistrictCode = "MMD-001", Contact = "09173456789", Address = "Makati, Philippines" },
            // North Luzon District
            new { Name = "JOBM Baguio City",             DistrictCode = "NLD-001", Contact = "09174567890", Address = "Baguio City, Philippines" },
            new { Name = "JOBM Dagupan",                 DistrictCode = "NLD-001", Contact = "09175678901", Address = "Dagupan City, Philippines" },
            // South Luzon & Bicol District
            new { Name = "JOBM Naga City",               DistrictCode = "SLB-001", Contact = "09176789012", Address = "Naga City, Philippines" },
            new { Name = "JOBM Legazpi",                 DistrictCode = "SLB-001", Contact = "09177890123", Address = "Legazpi City, Philippines" },
            // Visayas District
            new { Name = "JOBM Cebu City",               DistrictCode = "VSD-001", Contact = "09178901234", Address = "Cebu City, Philippines" },
            new { Name = "JOBM Iloilo City",             DistrictCode = "VSD-001", Contact = "09179012345", Address = "Iloilo City, Philippines" },
        };

        var added = 0;
        foreach (var c in churchSeeds)
        {
            var districtId = IdOf(c.DistrictCode);
            if (districtId == null) continue;
            if (!await db.Churches.AnyAsync(x => x.ChurchName == c.Name))
            {
                db.Churches.Add(new Church { DistrictId = districtId.Value, ChurchName = c.Name, ContactNumber = c.Contact, Address = c.Address, Status = "Active" });
                added++;
            }
        }

        if (added > 0)
        {
            await db.SaveChangesAsync();
            Console.WriteLine($"[Seeder] {added} church(es) created.");
        }
    }

}

