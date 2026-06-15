namespace offboarding_prc_api.Services;
using System.Net.Http.Headers;
using DotNetCommonLib.Models;
using Microsoft.EntityFrameworkCore;
using offboarding_prc_api.Data;
using offboarding_prc_api.DTOs;
using offboarding_prc_api.Exceptions;
using offboarding_prc_api.Models;

// ─────────────────────────────────────────────────────────────────
//  MANAGER INFO SERVICE
//
//  Orchestrates three data sources:
//    A. EMS  → logged-in employee record   (single EmpInfo)
//    B. EMS  → full employee list          (TempCacheResponse)
//    C. DB   → SubmissionLogs              (who is actively offboarding)
//
//  All heavy lifting (HTTP calls, DB query, enrichment) lives here
//  so the controller stays thin.
// ─────────────────────────────────────────────────────────────────
public class ManagerInfoService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    AppDbContext db)
{
    // ── Config keys ──────────────────────────────────────────────
    private string EmsBaseUrl =>
        configuration["Ems_Url"]
        ?? throw new InvalidOperationException("Ems_Url missing in configuration.");

    // ── Public entry point ───────────────────────────────────────
    public async Task<ManagerInfoResponse> GetManagerInfoAsync(string bearerToken)
    {
        // ── Step A: fetch logged-in user ─────────────────────────
        EmpInfo me = await FetchMeAsync(bearerToken);
        string myEmpId = me.EmpId ?? string.Empty;

        // ── Step B: fetch full employee list ─────────────────────
        List<EmpInfo> allEmployees = await FetchAllEmployeesAsync(bearerToken);

        // ── Step C: find direct reports ──────────────────────────
        // An employee is a direct report when their managerEmpCode == myEmpId
        List<EmpInfo> directReports = allEmployees
            .Where(e => e.ManagerEmpCode == myEmpId)
            .ToList();

        bool isReportingManager = directReports.Count > 0;

        // ── Step D: find who among the team is offboarding ───────
        // Pull all active SubmissionLog empIds in one DB query
        List<string> directReportEmpIds = directReports
            .Where(e => e.EmpId is not null)
            .Select(e => e.EmpId!)
            .ToList();

        HashSet<string> offboardingIds = directReportEmpIds.Count == 0
            ? []
            : (await db.SubmissionLogs
                .Where(s => s.IsActive && directReportEmpIds.Contains(s.EmployeeId))
                .Select(s => s.EmployeeId)
                .Distinct()
                .ToListAsync())
              .ToHashSet();

        // ── Step E: count active vs offboarding ──────────────────
        // "Active" = in the team list AND not offboarding
        // (EMS status field is null for most records; we use submission presence as the offboarding signal)
        int offboardingCount = directReports.Count(e => offboardingIds.Contains(e.EmpId ?? ""));
        int activeCount = directReports.Count - offboardingCount;

        // ── Step F: build team member DTOs ───────────────────────
        List<TeamMemberDto> teamDtos = directReports.Select(e => new TeamMemberDto(
            EmpId: e.EmpId ?? "",
            FullName: $"{e.FirstName} {e.LastName}".Trim(),
            Desg: e.Desg ?? "",
            Project: e.Project,
            Grade: e.Grade,
            Email: e.Email,
            Gender: e.Gender,
            Doj: e.Doj?.ToString("yyyy-MM-dd"),
            IsOffboarding: offboardingIds.Contains(e.EmpId ?? "")
        )).ToList();

        // ── Step G: assemble final response ──────────────────────
        return new ManagerInfoResponse(
            EmpId: myEmpId,
            FullName: $"{me.FirstName} {me.LastName}".Trim(),
            Desg: me.Desg ?? "",
            Email: me.Email,
            Project: me.Project,
            Grade: me.Grade,
            ManagerEmpCode: me.ManagerEmpCode,
            ReportingManager: me.ReportingManager,

            IsReportingManager: isReportingManager,
            NoOfTotalMembers: directReports.Count.ToString(),
            NoOfActive: activeCount.ToString(),
            NoOfOffboarding: offboardingCount.ToString(),

            TotalMembers: teamDtos
        );
    }

    // ── Private helpers ──────────────────────────────────────────

    /// <summary>Calls EMS /GetEmployeeById → single EmpInfo for the token owner.</summary>
    private async Task<EmpInfo> FetchMeAsync(string bearerToken)
    {
        using HttpClient client = CreateAuthorizedClient(bearerToken);
        string url = $"{EmsBaseUrl}/api/Employee/GetEmployeeById";

        var response = await client.PostAsync(url, new StringContent(string.Empty));

        if (!response.IsSuccessStatusCode)
            throw new BadRequestException($"EMS GetEmployeeById failed: {response.ReasonPhrase}");

        var wrapper = await response.Content.ReadFromJsonAsync<BaseResponse<EmpInfo>>();
        return wrapper?.Result ?? new EmpInfo();
    }

    /// <summary>Calls EMS endpoint that returns ALL employees (TempCache shape).</summary>
    private async Task<List<EmpInfo>> FetchAllEmployeesAsync(string bearerToken)
    {
        using HttpClient client = CreateAuthorizedClient(bearerToken);

        // EMS returns { isSuccess, result: [...] }
        // Adjust the path below to match your actual EMS "get all" endpoint.
        string url = $"{EmsBaseUrl}/api/Employee/TempCache";

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            throw new BadRequestException($"EMS GetAllEmployees failed: {response.ReasonPhrase}");

        var wrapper = await response.Content.ReadFromJsonAsync<TempCacheResponse>();
        return wrapper?.Result ?? [];
    }

    /// <summary>Creates a pre-authorised HttpClient for EMS calls.</summary>
    private HttpClient CreateAuthorizedClient(string bearerToken)
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", bearerToken);
        return client;
    }
}