using System;
using System.Collections.Generic;

namespace appointment_service.Models;

public partial class BudgetAllocation
{
    public int AllocationId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Allocated { get; set; }

    public decimal Spent { get; set; }

    public int? BudgetId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Budget? Budget { get; set; }

    public decimal RemainingBalance => Allocated - Spent;

    public double Utilization => Allocated > 0 ? (double)(Spent / Allocated) * 100 : 0;
}
