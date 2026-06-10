using DotNetCommonLib.Models;
using offboarding_prc_api.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using offboarding_prc_api.Constants;
using offboarding_prc_api.Exceptions;


namespace offboarding_prc_api.Services
{
    public class TempCacheService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MemoryCacheService _memoryCacheService;


        public TempCacheService(IHttpClientFactory httpClientFactory, MemoryCacheService memoryCacheService)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCacheService = memoryCacheService;
        }

        private HttpClient createHttpClient(string clientName)
        {
            HttpClient client = _httpClientFactory.CreateClient(clientName);
            return client;
        }
        public async Task<List<EmpInfo>> GetAllEmployeeInfoAsync()
        {
            string uri = "/api/Employee/TempCache";

            HttpClient client = createHttpClient(BussinessConstant.EMS_CLIENT_TITLE);
            string? baseUrl = await _memoryCacheService.GetUrlByKeyAsync(BussinessConstant.EMS_SUB_MODULE_KEY);
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException($"Base URL for {BussinessConstant.EMS_SUB_MODULE_KEY} is missing in configuration");
            }
            string url = baseUrl + uri;
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new BadRequestException($"{ErrorMessages.API_CALL_FAILED} {response.ReasonPhrase}");
            }
            var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<EmpInfo>>>();

            return result?.Result ?? [];

        }
    }
}
