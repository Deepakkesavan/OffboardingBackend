namespace offboarding_prc_api.Services
{
    public class OutgoingRequestHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OutgoingRequestHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpContext? context = _httpContextAccessor.HttpContext;
            if (context == null)
                throw new InvalidOperationException("HttpContext not available");

            string? token = null;

            // 1️⃣ Check HttpContext.Items
            if (context.Items.TryGetValue("JWT", out var jwtObj))
            {
                token = jwtObj as string;
            }

            // 2️⃣ Check Authorization Header
            if (string.IsNullOrEmpty(token))
            {
                string authHeader = context.Request.Headers.Authorization.ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authHeader["Bearer ".Length..].Trim();
                }
            }

            // 3️⃣ Check Cookies
            if (string.IsNullOrEmpty(token))
            {
                if (context.Request.Cookies.TryGetValue("JWT", out var cookieToken))
                {
                    token = cookieToken;
                }
            }

            // 4️⃣ Attach to outgoing request
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                throw new UnauthorizedAccessException("No JWT found in Items, Header, or Cookies.");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
