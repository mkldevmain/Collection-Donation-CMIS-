using System;

namespace CMIS.Models;

/// <summary>
/// Maps to the expenses table.
/// Tracks all church expenditures with category, description, and reference number.
/// </summary>
public class FinancialExpense
{
    public int ExpenseId { get; set; }

    /// <summary>"Utilities", "Salaries", "Maintenance", "Events"</summary>
    public string Category { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Description { get; set; } = null!;

    public DateOnly DateSpent { get; set; }

    /// <summary>FK to Profile (the staff member who recorded this expense)</summary>
    public int RecordedBy { get; set; }

    /// <summary>Physical receipt or voucher ID (e.g., "OR-2026-001")</summary>
    public string ReferenceNumber { get; set; } = null!;

    // Navigation
    public virtual Profile Recorder { get; set; } = null!;
}
