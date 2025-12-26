using Offboarding_inner_api.Model;
using Offboarding_inner_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Offboarding_inner_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingManagerController : ControllerBase
    {
        private readonly IReportingManagerService _reportingManagerService;

        public ReportingManagerController(IReportingManagerService reportingManagerService)
        {
            _reportingManagerService = reportingManagerService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReportingManagerApproval>>> GetAllRmApprovals()
        {
            var approvals = await _reportingManagerService.GetAllReportingManagerApprovalsAsync();
            return Ok(approvals);
        }

        [HttpGet("{documentId}")]
        public async Task<ActionResult<ReportingManagerApproval>> GetRmApprovalById(int documentId)
        {
            var approval = await _reportingManagerService.GetReportingManagerApprovalByIdAsync(documentId);

            if (approval == null)
            {
                return NotFound();
            }

            return Ok(approval);
        }

        [HttpPost]
        public async Task<ActionResult<ReportingManagerApproval>> AddRmApproval(ReportingManagerApproval rmApproval)
        {
            if (rmApproval == null)
            {
                return BadRequest();
            }

            var addedApproval = await _reportingManagerService.AddReportingManagerApprovalAsync(rmApproval);

            return CreatedAtAction(
                nameof(GetRmApprovalById),
                new { documentId = addedApproval.DocumentId },
                addedApproval
            );
        }

        [HttpPut("{documentId}")]
        public async Task<IActionResult> UpdateRmApproval(int documentId, ReportingManagerApproval rmApproval)
        {
            var isUpdated = await _reportingManagerService.UpdateReportingManagerApprovalAsync(documentId, rmApproval);

            if (!isUpdated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteRmApproval(int documentId)
        {
            var isDeleted = await _reportingManagerService.DeleteReportingManagerApprovalAsync(documentId);

            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}