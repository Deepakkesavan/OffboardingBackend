namespace offboarding_prc_api.Models;

// Mirrors the { isSuccess, result: [...] } wrapper that EMS returns
// for the "get all employees" (TempCache) endpoint.
public class TempCacheResponse
{
    public bool IsSuccess { get; set; }
    public List<EmpInfo> Result { get; set; } = [];
}