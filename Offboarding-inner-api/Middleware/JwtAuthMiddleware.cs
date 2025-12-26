using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Offboarding_inner_api.Middleware
{
    public class JwtAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _jwtSecret;



        public JwtAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _jwtSecret = configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret missing from configuration.");
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";

            // Skip JWT validation for swagger/public paths
            if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            UserDto? user = null;

            // 1. Try JWT from cookies
            var jwtToken = context.Request.Cookies["JWT"];
            if (!string.IsNullOrEmpty(jwtToken))
            {
                user = ValidateJwtLocally(jwtToken);
                if (user != null) context.Items["JWT"] = jwtToken;
            }

            // 2. Try JWT from Authorization header (fallback)
            if (user == null && context.Request.Headers.TryGetValue("Authorization", out var authHeader) &&
                authHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var bearerToken = authHeader.ToString()["Bearer ".Length..].Trim();
                user = ValidateJwtLocally(bearerToken);
                if (user != null) context.Items["JWT"] = bearerToken;

            }

            // 3. If still null → try refresh token
            if (user == null)
            {
                var refreshToken = context.Request.Cookies["REFRESH_TOKEN"];
                if (!string.IsNullOrEmpty(refreshToken)) user = await RefreshJwtUsingRefreshToken(context, refreshToken);
            }

            // 4. If still null → return 401
            if (user == null) throw new UnauthorizedAccessException("No valid JWT or refresh token.");

            // 5. Attach user info to HttpContext
            context.Items["User"] = user;
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {
                        new(CustomClaimTypes.EmpId, user.EmpId.ToString()),
                        new(CustomClaimTypes.Designation, user.Designation)
                    }, "JWT"));

            await _next(context);
        }

        private async Task<UserDto?> RefreshJwtUsingRefreshToken(HttpContext context, string refreshToken)
        {
            var authBaseUrl = context.RequestServices.GetRequiredService<IConfiguration>()["URL:AuthServiceBaseUrl"];

            var refreshUrl = $"{authBaseUrl}/api/auth/refresh-token";

            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };
            handler.CookieContainer.Add(new Uri(authBaseUrl), new Cookie("REFRESH_TOKEN", refreshToken));

            using var httpClient = new HttpClient(handler);
            var response = await httpClient.PostAsync(refreshUrl, null);

            if (!response.IsSuccessStatusCode) return null;

            RefreshResponse? result = await response.Content.ReadFromJsonAsync<RefreshResponse>();
            if (result == null || string.IsNullOrEmpty(result.NewJwt)) return null;

            var user = ValidateJwtLocally(result.NewJwt);
            if (user != null) context.Items["JWT"] = result.NewJwt;


            return user;
        }

        private UserDto? ValidateJwtLocally(string jwtToken)
        {
            if (string.IsNullOrWhiteSpace(jwtToken))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.FromMinutes(0)
            };

            try
            {
                var principal = tokenHandler.ValidateToken(jwtToken, validationParams, out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                var empIdClaim = principal.FindFirst(CustomClaimTypes.EmpId)?.Value;
                var designationClaim = principal.FindFirst(CustomClaimTypes.Designation)?.Value;

                if (!int.TryParse(empIdClaim, out var empId))
                    return null;

                return new UserDto
                {
                    EmpId = empId,
                    Designation = designationClaim ?? "Unknown",

                };
            }
            catch
            {
                return null;
            }
        }
    }

    public class RefreshResponse
    {
        public string NewJwt { get; set; } = string.Empty;
        public string NewRefreshToken { get; set; } = string.Empty;
        public UserDto User { get; set; } = new UserDto();
    }

    public class UserDto
    {
        public int EmpId { get; set; }

        public string? Designation { get; set; }

        public string? EmployeeFirstName { get; set; }
        public string? EmployeeLastName { get; set; }


    }

    public static class CustomClaimTypes
    {
        public const string EmpId = "empId";
        public const string Designation = "designation";
    }
}
