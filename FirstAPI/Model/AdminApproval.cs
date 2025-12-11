using System;
using System.Collections.Generic;

namespace FirstAPI.Model;

public partial class AdminApproval
{
    public int AdminId { get; set; }

    public string? IdentityCardorAccesscard { get; set; }

    public string? Laptopwithallaccessories { get; set; }

    public string? OfficeorDeskKeys { get; set; }

    public string? BusinessCards { get; set; }

    public string? Companydocuments { get; set; }

    public string? CompanyBookorManuals { get; set; }
}
