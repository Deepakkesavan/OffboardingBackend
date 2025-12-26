using System;
using System.Collections.Generic;

namespace Offboarding_inner_api.Model;

public partial class ReportingManagerApproval
{
    public int DocumentId { get; set; }

    public string DocumentName { get; set; } = null!;
}
