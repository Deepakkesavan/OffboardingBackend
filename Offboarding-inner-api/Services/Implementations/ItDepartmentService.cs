using Offboarding_inner_api.Model;
using Offboarding_inner_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Offboarding_inner_api.Services.Implementations
{
    public class ItDepartmentService : IItDepartmentService
    {
        private readonly EmployeeDetailsContext _context;

        public ItDepartmentService(EmployeeDetailsContext context)
        {
            _context = context;
        }

        public async Task<List<ItDepartment>> GetAllItDepartmentsAsync()
        {
            return await _context.ItDepartments.ToListAsync();
        }

        public async Task<ItDepartment?> GetItDepartmentByIdAsync(int itDepartmentId)
        {
            return await _context.ItDepartments.FindAsync(itDepartmentId);
        }

        public async Task<ItDepartment> AddItDepartmentAsync(ItDepartment itDepartment)
        {
            _context.ItDepartments.Add(itDepartment);
            await _context.SaveChangesAsync();
            return itDepartment;
        }

        public async Task<bool> UpdateItDepartmentAsync(int itDepartmentId, ItDepartment itDepartment)
        {
            var existingIt = await _context.ItDepartments.FindAsync(itDepartmentId);

            if (existingIt == null)
            {
                return false;
            }

            existingIt.LoginIdToBeDisabledFrom = itDepartment.LoginIdToBeDisabledFrom;
            existingIt.MailIdToBeDisabledFrom = itDepartment.MailIdToBeDisabledFrom;
            existingIt.Vdiaccess = itDepartment.Vdiaccess;
            existingIt.BioMetricOdc = itDepartment.BioMetricOdc;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteItDepartmentAsync(int itDepartmentId)
        {
            var itDepartment = await _context.ItDepartments.FindAsync(itDepartmentId);

            if (itDepartment == null)
            {
                return false;
            }

            _context.ItDepartments.Remove(itDepartment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}