namespace FirstAPI.Models
{
    public class EmployeeOffboard
    {
        public int EmployeeId { get; set; }
        public string employeeName { get; set; }
        public string employeeEmail { get; set; }
        public string employeeCode { get; set; }
        public string designation { get; set; }
        public string project { get; set; }
        public DateOnly dateOfJoining { get; set; }
        public string location { get; set; }
        public DateOnly resignationSubmittedDate { get; set; }
        public DateOnly lastWorkingDay { get; set; }
        public string panCardNumber { get; set; }
        public string bankAccountNumber { get; set; }
        public string employeeAddress { get; set; }
        public string contactNumberResidence { get; set; }
        public string mobileNumber { get; set; }
        public string employmentStatus { get; set; }
    }
}
