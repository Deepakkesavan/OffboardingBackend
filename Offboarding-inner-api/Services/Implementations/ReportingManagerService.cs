using Offboarding_inner_api.Model;
using Offboarding_inner_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Offboarding_inner_api.Services.Implementations
{
    public class ReportingManagerService : IReportingManagerService
    {
        private readonly EmployeeDetailsContext _context;

        public ReportingManagerService(EmployeeDetailsContext context)
        {
            _context = context;
        }

        public async Task<List<ReportingManagerApproval>> GetAllReportingManagerApprovalsAsync()
        {
            return await _context.ReportingManagerApprovals.ToListAsync();
        }

        public async Task<ReportingManagerApproval?> GetReportingManagerApprovalByIdAsync(int documentId)
        {
            return await _context.ReportingManagerApprovals.FindAsync(documentId);
        }

        public async Task<ReportingManagerApproval> AddReportingManagerApprovalAsync(ReportingManagerApproval rmApproval)
        {
            _context.ReportingManagerApprovals.Add(rmApproval);
            await _context.SaveChangesAsync();
            return rmApproval;
        }

        public async Task<bool> UpdateReportingManagerApprovalAsync(int documentId, ReportingManagerApproval rmApproval)
        {
            var existingRm = await _context.ReportingManagerApprovals.FindAsync(documentId);

            if (existingRm == null)
            {
                return false;
            }

            existingRm.DocumentName = rmApproval.DocumentName;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReportingManagerApprovalAsync(int documentId)
        {
            var rmApproval = await _context.ReportingManagerApprovals.FindAsync(documentId);

            if (rmApproval == null)
            {
                return false;
            }

            _context.ReportingManagerApprovals.Remove(rmApproval);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
