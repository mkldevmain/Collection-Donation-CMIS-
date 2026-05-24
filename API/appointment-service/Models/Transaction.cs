using System;
using System.Collections.Generic;

namespace appointment_service.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public string TransactionCode { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int? BudgetAllocationId { get; set; }

    public string? BudgetLabel { get; set; }

    public decimal Amount { get; set; }

    public int RecordedById { get; set; }

    public DateOnly TransactionDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Profile RecordedBy { get; set; } = null!;

    public virtual BudgetAllocation? Allocation { get; set; }
}
