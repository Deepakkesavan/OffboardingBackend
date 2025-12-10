using FirstAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        static private List<AdminApproval> AdminObjects = new List<AdminApproval>
        {
            new AdminApproval
            {
                IdentityCardorAccesscard = "IdentityCardorAccesscard",
                Laptopwithallaccessories = "Laptopwithallaccessories",
                OfficeorDeskKeys = "AccessRevoked",
                BusinessCards = "AccessRetained",
                Companydocuments = "Companydocuments",
                CompanyBookorManuals = "CompanyBookorManuals"
            }
        };
        [HttpGet]
        public ActionResult<List<AdminApproval>> getAdminApproval()
        {
            return Ok(AdminObjects);
        }
        [HttpGet("{identityCardorAccesscard}")]
        public ActionResult<AdminApproval> getAdminApprovalbyId(string identityCardorAccesscard)
        {
            var adminApprovalbyId = AdminObjects.FirstOrDefault(adminObjectbyid => adminObjectbyid.IdentityCardorAccesscard == identityCardorAccesscard);
            if (adminApprovalbyId == null)
            {
                return NotFound();
            }
            return Ok(adminApprovalbyId);
        }
        [HttpPost]
        public ActionResult<AdminApproval> AddItDepartment(AdminApproval adminApproval)
        {
            if (adminApproval == null)
            {
                return BadRequest();
            }
            AdminObjects.Add(adminApproval);
            return CreatedAtAction(nameof(getAdminApprovalbyId), new { id = adminApproval.IdentityCardorAccesscard }, adminApproval);
        }
    }
}
