namespace appointment_service.DTOs;

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
