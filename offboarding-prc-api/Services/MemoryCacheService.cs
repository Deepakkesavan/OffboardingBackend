using Microsoft.Extensions.Caching.Memory;
using offboarding_prc_api.Constants;
using offboarding_prc_api.Models;

namespace offboarding_prc_api.Services
{
    public class MemoryCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MemoryCacheService(IMemoryCache cache, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _cache = cache;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string> GetUrlByKeyAsync(string key)
        {
            Dictionary<string, string> urls = await GetAllUrlsAsync();

            return urls.TryGetValue(key, out var value)
                ? value
                : throw new KeyNotFoundException($"URL for {key} not found");
        }

        private async Task<Dictionary<string, string>> GetAllUrlsAsync()
        {
            return await _cache.GetOrCreateAsync(BussinessConstant.URLS_CACHE_KEY, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
                ModulesResponse modulesResponse = await GetModulesConfigAsync();
                Dictionary<string, string> urls = [];
                foreach (var module in modulesResponse.Modules)
                {
                    urls[module.Key] = module.Url;
                    foreach (var subModule in module.SubModules)
                    {
                        urls[subModule.Key] = subModule.Url;
                    }
                }
                return urls;
            }) ?? [];
        }
        private async Task<ModulesResponse> GetModulesConfigAsync()
        {
            return await FetchConfig<ModulesResponse>(BussinessConstant.MODULES_URI);
        }
        private async Task<T> FetchConfig<T>(string uri)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            string? serviceUrl = _configuration["Config_Url"];

            if (string.IsNullOrEmpty(serviceUrl))
                throw new InvalidOperationException("Missing Config Url");

            string url = serviceUrl + uri;

            HttpResponseMessage response =
                await client.PostAsJsonAsync(url, new { });

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>()
                   ?? throw new InvalidOperationException("Invalid config response");
        }
    }
}
