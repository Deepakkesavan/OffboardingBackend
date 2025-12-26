using Offboarding_inner_api.Model;
using Offboarding_inner_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Offboarding_inner_api.Services.Implementations
{
    public class EmployeeOffboardingService : IEmployeeOffboardingService
    {
        private readonly EmployeeDetailsContext _empDetailsContext;

        public EmployeeOffboardingService(EmployeeDetailsContext employeeDetailsContext)
        {
            _empDetailsContext = employeeDetailsContext;
        }

        public async Task<List<EmployeeOffboard>> GetAllEmployeeOffboardingsAsync()
        {
            return await _empDetailsContext.EmployeeOffboards.ToListAsync();
        }

        public async Task<EmployeeOffboard?> GetEmployeeOffboardingByIdAsync(int employeeId)
        {
            return await _empDetailsContext.EmployeeOffboards.FindAsync(employeeId);
        }

        public async Task<EmployeeOffboard> AddEmployeeOffboardingAsync(EmployeeOffboard employeeOffboard)
        {
            _empDetailsContext.EmployeeOffboards.Add(employeeOffboard);
            await _empDetailsContext.SaveChangesAsync();
            return employeeOffboard;
        }

        public async Task<bool> UpdateEmployeeOffboardingAsync(int employeeId, EmployeeOffboard employeeOffboard)
        {
            var employeeOffboardings = await _empDetailsContext.EmployeeOffboards.FindAsync(employeeId);

            if (employeeOffboardings == null)
            {
                return false;
            }

            employeeOffboardings.EmployeeName = employeeOffboard.EmployeeName;
            employeeOffboardings.EmployeeEmail = employeeOffboard.EmployeeEmail;
            employeeOffboardings.EmployeeCode = employeeOffboard.EmployeeCode;
            employeeOffboardings.Designation = employeeOffboard.Designation;
            employeeOffboardings.Project = employeeOffboard.Project;
            employeeOffboardings.DateOfJoining = employeeOffboard.DateOfJoining;
            employeeOffboardings.Location = employeeOffboard.Location;
            employeeOffboardings.ResignationSubmittedDate = employeeOffboard.ResignationSubmittedDate;
            employeeOffboardings.LastWorkingDay = employeeOffboard.LastWorkingDay;
            employeeOffboardings.PanCardNumber = employeeOffboard.PanCardNumber;
            employeeOffboardings.BankAccountNumber = employeeOffboard.BankAccountNumber;
            employeeOffboardings.MobileNumber = employeeOffboard.MobileNumber;
            employeeOffboardings.ContactNumberResidence = employeeOffboard.ContactNumberResidence;
            employeeOffboardings.EmployeeAddress = employeeOffboard.EmployeeAddress;
            employeeOffboardings.EmploymentStatus = employeeOffboard.EmploymentStatus;

            await _empDetailsContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEmployeeOffboardingAsync(int employeeId)
        {
            var employeeOffboardings = await _empDetailsContext.EmployeeOffboards.FindAsync(employeeId);

            if (employeeOffboardings == null)
            {
                return false;
            }

            _empDetailsContext.EmployeeOffboards.Remove(employeeOffboardings);
            await _empDetailsContext.SaveChangesAsync();
            return true;
        }
    }
    }
