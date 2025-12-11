namespace FirstAPI.Models
{
    public class HrApproval
    {
            public int HrId { get; set; }
            public string NoticePayStatus { get; set; }

            public int NoticePayDaysPayable { get; set; }

            public int NoticePayDaysRecoverable { get; set; }

            public string MediclaimOrMealCardStatus { get; set; }

            public string IncomeTaxProofStatus { get; set; }

            public string ExitInterviewFormAttached { get; set; }

            public string ResignationLetterAttached { get; set; }
        
    }
}
