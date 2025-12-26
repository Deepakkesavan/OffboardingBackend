using Offboarding_inner_api.Model;
using Offboarding_inner_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Offboarding_inner_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminApprovalService _adminApprovalService;

        public AdminController(IAdminApprovalService adminApprovalService)
        {
            _adminApprovalService = adminApprovalService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AdminApproval>>> GetAllAdminApprovals()
        {
            var approvals = await _adminApprovalService.GetAllAdminApprovalsAsync();
            return Ok(approvals);
        }

        [HttpGet("{adminId}")]
        public async Task<ActionResult<AdminApproval>> GetAdminApprovalById(int adminId)
        {
            var approval = await _adminApprovalService.GetAdminApprovalByIdAsync(adminId);

            if (approval == null)
            {
                return NotFound();
            }

            return Ok(approval);
        }

        [HttpPost]
        public async Task<ActionResult<AdminApproval>> AddAdminApproval(AdminApproval adminApproval)
        {
            if (adminApproval == null)
            {
                return BadRequest();
            }

            var addedApproval = await _adminApprovalService.AddAdminApprovalAsync(adminApproval);

            return CreatedAtAction(
                nameof(GetAdminApprovalById),
                new { adminId = addedApproval.AdminId },
                addedApproval
            );
        }

        [HttpPut("{adminId}")]
        public async Task<IActionResult> UpdateAdminApproval(int adminId, AdminApproval adminApproval)
        {
            var isUpdated = await _adminApprovalService.UpdateAdminApprovalAsync(adminId, adminApproval);

            if (!isUpdated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{adminId}")]
        public async Task<IActionResult> DeleteAdminApproval(int adminId)
        {
            var isDeleted = await _adminApprovalService.DeleteAdminApprovalAsync(adminId);

            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}