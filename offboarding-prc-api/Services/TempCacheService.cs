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
        private readonly IConfiguration _configuration;


        public TempCacheService(IHttpClientFactory httpClientFactory, MemoryCacheService memoryCacheService, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCacheService = memoryCacheService;
            _configuration = configuration;
        }

        //private HttpClient createHttpClient(string clientName)
        //{
        //    HttpClient client = _httpClientFactory.CreateClient(clientName);
        //    return client;
        //}
        public async Task<EmpInfo> GetAllEmployeeInfoDirectAsync(string token)
        {
            string? baseUrl = _configuration["Ems_Url"];

            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("Ems_Url is missing in configuration");

            HttpClient client = _httpClientFactory.CreateClient();

            // Attach the JWT token from the incoming request
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            string url = baseUrl + "/api/Employee/GetEmployeeById";

            // Corrected the PostAsync call to include a valid HttpContent object
            var response = await client.PostAsync(url, new StringContent(string.Empty));

            if (!response.IsSuccessStatusCode)
                throw new BadRequestException($"{ErrorMessages.API_CALL_FAILED} {response.ReasonPhrase}");

            var result = await response.Content.ReadFromJsonAsync<BaseResponse<EmpInfo>>();
            return result?.Result ?? new EmpInfo();
        }
    }
}
