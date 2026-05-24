using System.Security.Claims;
using CMIS.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CMIS.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _localStorage;
    private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    
    // 1. THE CACHE: Stores the user in server memory to prevent JS Interop crashes
    private ClaimsPrincipal? _currentUser; 

    public CustomAuthStateProvider(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // If we already loaded the user into memory, return them instantly!
        if (_currentUser != null)
        {
            return new AuthenticationState(_currentUser);
        }

        try
        {
            var result = await _localStorage.GetAsync<UserSession>("cmis_session_v2");
            if (result.Success && result.Value is not null)
            {
                var claims = BuildClaims(result.Value);
                var identity = new ClaimsIdentity(claims, "apiauth_type");
                _currentUser = new ClaimsPrincipal(identity);
                return new AuthenticationState(_currentUser);
            }
        }
        catch
        {
            // JavaScript interop is not available
        }

        return new AuthenticationState(_anonymous);
    }

    public async Task MarkUserAsAuthenticated(Account account)
    {
        // Grab the active assignment to find the real Church and District IDs
        var activeAssignment = account.Profile?.LeadershipAssignments?.FirstOrDefault(la => la.Status == "Active");

        var session = new UserSession
        {
            AccountId = account.AccountId,
            Username = account.Username ?? "",
            Email = account.Email ?? "",
            RoleId = account.RoleId,
            ChurchId = activeAssignment?.ChurchId ?? account.Profile?.ChurchId ?? 0,
            DistrictId = activeAssignment?.DistrictId ?? 0, // 2. FIX: No longer hardcoded to 0
            RoleName = account.Role?.RoleName ?? "Member",
            FullName = account.Profile != null 
                ? $"{account.Profile.FirstName} {account.Profile.LastName}" 
                : (account.Username ?? account.Email ?? "Unknown User")
        };

        try
        {
            await _localStorage.SetAsync("cmis_session_v2", session);
        }
        catch { /* Ignore storage errors */ }

        var claims = BuildClaims(session);
        var identity = new ClaimsIdentity(claims, "apiauth_type");
        
        // Update the server cache
        _currentUser = new ClaimsPrincipal(identity); 

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        _currentUser = null; // Clear the cache
        try { await _localStorage.DeleteAsync("cmis_session_v2"); } catch { }
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    public async Task SwitchRoleContextAsync(int newRoleId, string newRoleName, int newChurchId, int newDistrictId)
    {
        try
        {
            var result = await _localStorage.GetAsync<UserSession>("cmis_session_v2");
            if (result.Success && result.Value is not null)
            {
                var session = result.Value;
                session.RoleId = newRoleId;
                session.RoleName = newRoleName;
                session.ChurchId = newChurchId;
                session.DistrictId = newDistrictId;

                await _localStorage.SetAsync("cmis_session_v2", session);

                var claims = BuildClaims(session);
                var identity = new ClaimsIdentity(claims, "apiauth_type");
                
                // Update the server cache
                _currentUser = new ClaimsPrincipal(identity); 

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
            }
        }
        catch { }
    }

    private static List<Claim> BuildClaims(UserSession session)
    {
        return new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, session.AccountId.ToString()),
            new(ClaimTypes.Name, session.Username ?? ""),
            new(ClaimTypes.Email, session.Email ?? ""),
            new(ClaimTypes.Role, session.RoleName ?? "Member"),
            new("RoleId", session.RoleId.ToString()),
            new("ChurchId", session.ChurchId.ToString()),
            new("DistrictId", session.DistrictId.ToString()),
            new("FullName", session.FullName ?? "Unknown User") 
        };
    }
}

public class UserSession
{
    public int AccountId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public int ChurchId { get; set; }
    public int DistrictId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}