using FirstAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingManagerController : ControllerBase
    {
        static private List<ReportingManagerApproval> reportingmangerApproval = new List<ReportingManagerApproval>
        {
            new ReportingManagerApproval
            {
                documentId = 1,
                documentName = "Files"
            }
        };
        [HttpGet]
        public ActionResult<List<ReportingManagerApproval>> getRmApproval()
        {
            return Ok(reportingmangerApproval);
        }
        [HttpGet("{id}")]
        public ActionResult<ReportingManagerApproval> getRmApprovalbyId(int id)
        {
            var rmApproval = reportingmangerApproval.FirstOrDefault(rmApproval => rmApproval.documentId == id);
            if (rmApproval == null)
            {
                return NotFound();
            }
            return Ok(rmApproval);
        }
        [HttpPost]
        public ActionResult<ReportingManagerApproval> AddRmApproval(ReportingManagerApproval rmApproval)
        {
            if (rmApproval == null)
            {
                return NotFound();
            }
            reportingmangerApproval.Add(rmApproval);
            return CreatedAtAction(nameof(getRmApprovalbyId), new { id = rmApproval.documentId}, rmApproval );
        }



    }
}
