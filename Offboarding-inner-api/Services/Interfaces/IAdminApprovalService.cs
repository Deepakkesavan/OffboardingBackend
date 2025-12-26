using Offboarding_inner_api.Model;

namespace Offboarding_inner_api.Services.Interfaces
{
    public interface IAdminApprovalService
    {
        Task<List<AdminApproval>> GetAllAdminApprovalsAsync();
        Task<AdminApproval?> GetAdminApprovalByIdAsync(int adminId);
        Task<AdminApproval> AddAdminApprovalAsync(AdminApproval adminApproval);
        Task<bool> UpdateAdminApprovalAsync(int adminId, AdminApproval adminApproval);
        Task<bool> DeleteAdminApprovalAsync(int adminId);
    }
}