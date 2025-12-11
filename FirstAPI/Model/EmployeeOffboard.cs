using System;
using System.Collections.Generic;

namespace FirstAPI.Model;

public partial class EmployeeOffboard
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public string EmployeeEmail { get; set; } = null!;

    public string EmployeeCode { get; set; } = null!;

    public string? Designation { get; set; }

    public string? Project { get; set; }

    public DateOnly DateOfJoining { get; set; }

    public string? Location { get; set; }

    public DateOnly ResignationSubmittedDate { get; set; }

    public DateOnly LastWorkingDay { get; set; }

    public string? PanCardNumber { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? EmployeeAddress { get; set; }

    public string? ContactNumberResidence { get; set; }

    public string? MobileNumber { get; set; }

    public string? EmploymentStatus { get; set; }
}
