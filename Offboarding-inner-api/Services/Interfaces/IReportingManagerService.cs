using Offboarding_inner_api.Model;

namespace Offboarding_inner_api.Services.Interfaces
{
    public interface IReportingManagerService
    {
        Task<List<ReportingManagerApproval>> GetAllReportingManagerApprovalsAsync();
        Task<ReportingManagerApproval?> GetReportingManagerApprovalByIdAsync(int documentId);
        Task<ReportingManagerApproval> AddReportingManagerApprovalAsync(ReportingManagerApproval rmApproval);
        Task<bool> UpdateReportingManagerApprovalAsync(int documentId, ReportingManagerApproval rmApproval);
        Task<bool> DeleteReportingManagerApprovalAsync(int documentId);
    }
}
