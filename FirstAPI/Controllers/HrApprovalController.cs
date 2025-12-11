using FirstAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HrApprovalController : ControllerBase
    {
        static private List<HrApproval> HrApprovals = new List<HrApproval>
        {
            new HrApproval
            {
                HrId = 1,
                NoticePayStatus = "Payable",
                NoticePayDaysPayable = 10,
                NoticePayDaysRecoverable = 0,
                MediclaimOrMealCardStatus = "Applicable",
                IncomeTaxProofStatus = "Yes",
                ExitInterviewFormAttached = "Yes",
                ResignationLetterAttached = "Yes"
            }
        };
        [HttpGet]
        public ActionResult<List<HrApproval>> getHrApproval()
        {
            return Ok(HrApprovals);
        }
        [HttpGet("{noticePayStatus}")]
        public ActionResult<HrApproval> getHrApprovalbyId(string noticePayStatus)
        {
            var hrApprovalsbyId = HrApprovals.FirstOrDefault(adminObjectbyid => adminObjectbyid.NoticePayStatus == noticePayStatus);
            if (hrApprovalsbyId == null)
            {
                return NotFound();
            }
            return Ok(hrApprovalsbyId);
        }
        [HttpPost]
        public ActionResult<HrApproval> AddHrApproval(HrApproval hrApproval)
        {
            if (hrApproval == null)
            {
                return BadRequest();
            }
            HrApprovals.Add(hrApproval);
            return CreatedAtAction(nameof(getHrApprovalbyId), new { id = hrApproval.NoticePayStatus }, hrApproval);
        }
    }
}
