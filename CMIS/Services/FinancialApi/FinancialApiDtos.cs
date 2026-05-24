namespace CMIS.Services.FinancialApi;

public class BudgetReadDto
{
    public int BudgetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public string FiscalYear { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int? ChurchId { get; set; }
    public string? ChurchName { get; set; }
    public int? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BudgetWriteDto
{
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public decimal TotalAmount { get; set; }
    public int? ChurchId { get; set; }
    public int? DistrictId { get; set; }
}

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

public class BudgetAllocationReadDto
{
    public int AllocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Allocated { get; set; }
    public decimal Spent { get; set; }
    public decimal RemainingBalance { get; set; }
    public double Utilization { get; set; }
    public int? BudgetId { get; set; }
    public string? BudgetName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BudgetAllocationWriteDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Allocated { get; set; }
    public decimal Spent { get; set; }
    public int? BudgetId { get; set; }
}

public class TransactionReadDto
{
    public int TransactionId { get; set; }
    public string TransactionCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int? BudgetAllocationId { get; set; }
    public string? AllocationName { get; set; }
    public string? AllocationBudgetName { get; set; }
    public string? BudgetLabel { get; set; }
    public decimal Amount { get; set; }
    public int RecordedById { get; set; }
    public string? RecordedByFirstName { get; set; }
    public string? RecordedByLastName { get; set; }
    public string? RecordedByName { get; set; }
    public DateOnly TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TransactionWriteDto
{
    public string TransactionCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int? BudgetAllocationId { get; set; }
    public string? BudgetLabel { get; set; }
    public decimal Amount { get; set; }
    public int RecordedById { get; set; }
    public DateOnly TransactionDate { get; set; }
}
