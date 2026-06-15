namespace offboarding_prc_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using offboarding_prc_api.Services;

// ─────────────────────────────────────────────────────────────────
//  MANAGER INFO CONTROLLER
//  GET /api/managerinfo
//
//  Flow:
//    1. Extract JWT from Authorization header
//    2. Call EMS /GetEmployeeById  →  get the logged-in user (EmpInfo)
//    3. Call EMS /GetAllEmployees  →  get the full employee list (TempCache)
//    4. Filter TempCache: employees whose managerEmpCode == loggedIn.EmpId
//    5. Cross-reference against SubmissionLogs to find who is offboarding
//    6. Return the enriched manager payload
// ─────────────────────────────────────────────────────────────────
[ApiController]
[Route("api/managerinfo")]
public class ManagerInfoController(ManagerInfoService managerInfoService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetManagerInfo()
    {
        string authHeader = Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        string token = authHeader["Bearer ".Length..].Trim();

        var result = await managerInfoService.GetManagerInfoAsync(token);
        return Ok(result);
    }
}