using System;
using System.Collections.Generic;

namespace FirstAPI.Model;

public partial class ItDepartment
{
    public int ItDepartmentId { get; set; }

    public int LoginIdToBeDisabledFrom { get; set; }

    public int MailIdToBeDisabledFrom { get; set; }

    public string? Vdiaccess { get; set; }

    public string? BioMetricOdc { get; set; }
}
