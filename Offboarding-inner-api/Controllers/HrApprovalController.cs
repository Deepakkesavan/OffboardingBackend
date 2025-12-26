using Offboarding_inner_api.Model;
using Offboarding_inner_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Offboarding_inner_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HrApprovalController : ControllerBase
    {
        private readonly IHrApprovalService _hrApprovalService;

        public HrApprovalController(IHrApprovalService hrApprovalService)
        {
            _hrApprovalService = hrApprovalService;
        }

        [HttpGet]
        public async Task<ActionResult<List<HrApproval>>> GetAllHrApprovals()
        {
            var approvals = await _hrApprovalService.GetAllHrApprovalsAsync();
            return Ok(approvals);
        }

        [HttpGet("{hrId}")]
        public async Task<ActionResult<HrApproval>> GetHrApprovalById(int hrId)
        {
            var approval = await _hrApprovalService.GetHrApprovalByIdAsync(hrId);

            if (approval == null)
            {
                return NotFound();
            }

            return Ok(approval);
        }

        [HttpPost]
        public async Task<ActionResult<HrApproval>> AddHrApproval(HrApproval hrApproval)
        {
            if (hrApproval == null)
            {
                return BadRequest();
            }

            var addedApproval = await _hrApprovalService.AddHrApprovalAsync(hrApproval);

            return CreatedAtAction(
                nameof(GetHrApprovalById),
                new { hrId = addedApproval.HrId },
                addedApproval
            );
        }

        [HttpPut("{hrId}")]
        public async Task<IActionResult> UpdateHrApproval(int hrId, HrApproval hrApproval)
        {
            var isUpdated = await _hrApprovalService.UpdateHrApprovalAsync(hrId, hrApproval);

            if (!isUpdated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{hrId}")]
        public async Task<IActionResult> DeleteHrApproval(int hrId)
        {
            var isDeleted = await _hrApprovalService.DeleteHrApprovalAsync(hrId);

            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}