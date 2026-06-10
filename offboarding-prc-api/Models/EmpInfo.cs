using System.Security;

namespace offboarding_prc_api.Models
{
    public class EmpInfo
    {
        public Guid? EmpGuid { get; set; }

        public string? EmpId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public string? Grade { get; set; }

        public int? DesgId { get; set; }
        public int? ProjId { get; set; }
        public string? Desg { get; set; }
        public string? Project { get; set; }
        public Guid? DesgGuid { get; set; }
        public string? ManagerEmpCode { get; set; }
        public string? Email { get; set; } = null!;
        public byte[]? Profile { get; set; }
        public Guid? ProjGuid { get; set; }


        //Personal Details
        public DateTime? Dob { get; set; }

        public string? Age { get; set; }

        public string? Gender { get; set; }

        public string? EmergencyContact1 { get; set; }

        public string? PersonalPhoneNumber { get; set; }

        public string? EmergencyContactName1 { get; set; }

        public string? EmergencyContactName2 { get; set; }

        public string? EmergencyContact2 { get; set; }

        public string? PermanentAddress { get; set; }

        public string? PresentAddress { get; set; }
        //work info
        public string? SourceOfHire { get; set; }

        public DateTime? Doj { get; set; }

        public DateTime? Doc { get; set; }

        public string? Status { get; set; }

        public string? CurrExp { get; set; }

        public string? TotalExp { get; set; }
        //role
        public string? Role { get; set; }

        public string? EmailTriggerStatus { get; set; }

        public DateTime? TransferFromDate { get; set; }

        public int? TypeId { get; set; }

        public string? ReportingManager { get; set; }

        public string? ProjectName { get; set; }

        //identity info
        public string? Uan { get; set; }

        public string? Pan { get; set; }

        public long? Aadhar { get; set; }
        public string? Ifsc { get; set; }

    }
}
