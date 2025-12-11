using FirstAPI.Model;

namespace FirstAPI.Services.Interfaces
{
    public interface IEmployeeOffboardingService
    {
        Task<List<EmployeeOffboard>> GetAllEmployeeOffboardingsAsync();
        Task<EmployeeOffboard?> GetEmployeeOffboardingByIdAsync(int employeeId);
        Task<EmployeeOffboard> AddEmployeeOffboardingAsync(EmployeeOffboard employeeOffboard);
        Task<bool> UpdateEmployeeOffboardingAsync(int employeeId, EmployeeOffboard employeeOffboard);
        Task<bool> DeleteEmployeeOffboardingAsync(int employeeId);
    }
}
