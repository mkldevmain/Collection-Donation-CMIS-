namespace appointment_service.DTOs;

public class BudgetProposalReadDto
{
    public int ProposalId { get; set; }
    public string ProposalCode { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? MinistryId { get; set; }
    public string? MinistryName { get; set; }
    public int? ChurchId { get; set; }
    public string? ChurchName { get; set; }
    public int? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public string Level { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public int SubmittedById { get; set; }
    public string? SubmittedByFirstName { get; set; }
    public string? SubmittedByLastName { get; set; }
    public string? SubmittedByName { get; set; }
    public int? ReviewedById { get; set; }
    public string? ReviewedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class BudgetProposalCreateDto
{
    public string ProposalCode { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? MinistryId { get; set; }
    public int? ChurchId { get; set; }
    public int? DistrictId { get; set; }
    public string Level { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int SubmittedById { get; set; }
}

public class BudgetProposalUpdateDto
{
    public string? Purpose { get; set; }
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
}

public class BudgetProposalStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int ReviewedById { get; set; }
    public string? RejectionReason { get; set; }
}
