using Offboarding_inner_api.Model;
using Offboarding_inner_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Offboarding_inner_api.Services.Implementations
{
    public class AdminApprovalService : IAdminApprovalService
    {
        private readonly EmployeeDetailsContext _context;

        public AdminApprovalService(EmployeeDetailsContext context)
        {
            _context = context;
        }

        public async Task<List<AdminApproval>> GetAllAdminApprovalsAsync()
        {
            return await _context.AdminApprovals.ToListAsync();
        }

        public async Task<AdminApproval?> GetAdminApprovalByIdAsync(int adminId)
        {
            return await _context.AdminApprovals.FindAsync(adminId);
        }

        public async Task<AdminApproval> AddAdminApprovalAsync(AdminApproval adminApproval)
        {
            _context.AdminApprovals.Add(adminApproval);
            await _context.SaveChangesAsync();
            return adminApproval;
        }

        public async Task<bool> UpdateAdminApprovalAsync(int adminId, AdminApproval adminApproval)
        {
            var existingAdmin = await _context.AdminApprovals.FindAsync(adminId);

            if (existingAdmin == null)
            {
                return false;
            }

            existingAdmin.IdentityCardorAccesscard = adminApproval.IdentityCardorAccesscard;
            existingAdmin.Laptopwithallaccessories = adminApproval.Laptopwithallaccessories;
            existingAdmin.OfficeorDeskKeys = adminApproval.OfficeorDeskKeys;
            existingAdmin.BusinessCards = adminApproval.BusinessCards;
            existingAdmin.Companydocuments = adminApproval.Companydocuments;
            existingAdmin.CompanyBookorManuals = adminApproval.CompanyBookorManuals;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAdminApprovalAsync(int adminId)
        {
            var adminApproval = await _context.AdminApprovals.FindAsync(adminId);

            if (adminApproval == null)
            {
                return false;
            }

            _context.AdminApprovals.Remove(adminApproval);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}