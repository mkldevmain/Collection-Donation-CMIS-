namespace appointment_service.DTOs;

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
