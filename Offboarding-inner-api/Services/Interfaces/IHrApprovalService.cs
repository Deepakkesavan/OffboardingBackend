using Offboarding_inner_api.Model;

namespace Offboarding_inner_api.Services.Interfaces
{
    public interface IHrApprovalService
    {
        Task<List<HrApproval>> GetAllHrApprovalsAsync();
        Task<HrApproval?> GetHrApprovalByIdAsync(int hrId);
        Task<HrApproval> AddHrApprovalAsync(HrApproval hrApproval);
        Task<bool> UpdateHrApprovalAsync(int hrId, HrApproval hrApproval);
        Task<bool> DeleteHrApprovalAsync(int hrId);
    }
}