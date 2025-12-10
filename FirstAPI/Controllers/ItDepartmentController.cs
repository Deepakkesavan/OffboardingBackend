using FirstAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItDepartmentController : ControllerBase
    {
        static private List<ItDepartment> itApprovals = new List<ItDepartment>
        {
            new ItDepartment
            {
                LoginIdToBeDisabledFrom = 1,
                MailIdToBeDisabledFrom = 2,
                VDIAccess = "AccessRevoked",
                BioMetricODC = "AccessRetained"
            }
        };
        [HttpGet]
        public ActionResult<List<ItDepartment>> getItDepartment()
        {
            return Ok(itApprovals);
        }
        [HttpGet("{id}")]
        public ActionResult<ItDepartment> getItDepartmentbyId(int id)
        {
            var itDepartmentbyId = itApprovals.FirstOrDefault(itdepbyid => itdepbyid.LoginIdToBeDisabledFrom == id);
            if (itDepartmentbyId == null)
            {
                return NotFound();
            }
            return Ok(itDepartmentbyId);
        }
        [HttpPost]
        public ActionResult<ItDepartment> AddItDepartment(ItDepartment itdepartment)
        {
            if(itdepartment == null)
            {
                return BadRequest();
            }
            itApprovals.Add(itdepartment);
            return CreatedAtAction(nameof(getItDepartmentbyId), new { id = itdepartment.LoginIdToBeDisabledFrom }, itdepartment);
        }

    }
}
