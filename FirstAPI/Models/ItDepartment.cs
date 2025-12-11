namespace FirstAPI.Models
{
    public class ItDepartment
    {
        public int ItDepartmentId { get; set; }
        public int LoginIdToBeDisabledFrom { get; set; }
        public int MailIdToBeDisabledFrom { get; set; }
        public string VDIAccess { get; set; }
        public string BioMetricODC { get; set; }
    }
}
