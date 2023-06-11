using Fatima.DAM;
using Fatima.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using System.Security.Claims;
using System.Text.Json;

namespace Fatima.Services
{
    public static class Identity
    {
        public static readonly string Schema = CookieAuthenticationDefaults.AuthenticationScheme;

        public static Account Get(HttpContext ctx)
        {
            ctx.Items.TryGetValue("Account", out object? acc);
            if (acc is null)
            {
                var err = new Exception("Error getting account without identity");
                Log.Error(err, "{Path}: Error", "Identity.Get");
                throw err;
            }
            return (acc as Account)!;
        }

        public static async Task Login(HttpContext ctx, string email, bool rememberMe)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Email, email),
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Schema));
            await ctx.SignInAsync(Schema, principal, new AuthenticationProperties
            {
                IsPersistent = rememberMe
            });
        }

        public static async Task Logout(HttpContext ctx)
        {
            await ctx.SignOutAsync(Schema);
        }
    }
}
