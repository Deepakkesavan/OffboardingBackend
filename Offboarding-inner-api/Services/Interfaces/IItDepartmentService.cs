using Offboarding_inner_api.Model;

namespace Offboarding_inner_api.Services.Interfaces
{
    public interface IItDepartmentService
    {
        Task<List<ItDepartment>> GetAllItDepartmentsAsync();
        Task<ItDepartment?> GetItDepartmentByIdAsync(int itDepartmentId);
        Task<ItDepartment> AddItDepartmentAsync(ItDepartment itDepartment);
        Task<bool> UpdateItDepartmentAsync(int itDepartmentId, ItDepartment itDepartment);
        Task<bool> DeleteItDepartmentAsync(int itDepartmentId);
    }
}