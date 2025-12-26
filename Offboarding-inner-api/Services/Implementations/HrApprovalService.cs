using Offboarding_inner_api.Model;
using Offboarding_inner_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Offboarding_inner_api.Services.Implementations
{
    public class HrApprovalService : IHrApprovalService
    {
        private readonly EmployeeDetailsContext _context;

        public HrApprovalService(EmployeeDetailsContext context)
        {
            _context = context;
        }

        public async Task<List<HrApproval>> GetAllHrApprovalsAsync()
        {
            return await _context.HrApprovals.ToListAsync();
        }

        public async Task<HrApproval?> GetHrApprovalByIdAsync(int hrId)
        {
            return await _context.HrApprovals.FindAsync(hrId);
        }

        public async Task<HrApproval> AddHrApprovalAsync(HrApproval hrApproval)
        {
            _context.HrApprovals.Add(hrApproval);
            await _context.SaveChangesAsync();
            return hrApproval;
        }

        public async Task<bool> UpdateHrApprovalAsync(int hrId, HrApproval hrApproval)
        {
            var existingHr = await _context.HrApprovals.FindAsync(hrId);

            if (existingHr == null)
            {
                return false;
            }

            existingHr.NoticePayStatus = hrApproval.NoticePayStatus;
            existingHr.NoticePayDaysPayable = hrApproval.NoticePayDaysPayable;
            existingHr.NoticePayDaysRecoverable = hrApproval.NoticePayDaysRecoverable;
            existingHr.MediclaimOrMealCardStatus = hrApproval.MediclaimOrMealCardStatus;
            existingHr.IncomeTaxProofStatus = hrApproval.IncomeTaxProofStatus;
            existingHr.ExitInterviewFormAttached = hrApproval.ExitInterviewFormAttached;
            existingHr.ResignationLetterAttached = hrApproval.ResignationLetterAttached;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteHrApprovalAsync(int hrId)
        {
            var hrApproval = await _context.HrApprovals.FindAsync(hrId);

            if (hrApproval == null)
            {
                return false;
            }

            _context.HrApprovals.Remove(hrApproval);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}