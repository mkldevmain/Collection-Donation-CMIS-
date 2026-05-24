using System;

namespace CMIS.Models;

/// <summary>
/// Maps to the financial_income table.
/// Tracks all tithes and offerings recorded by ministry staff.
/// </summary>
public class FinancialIncome
{
    public int IncomeId { get; set; }

    /// <summary>FK to Profile (optional — general offerings may have no specific member)</summary>
    public int? MemberId { get; set; }

    /// <summary>"Tithe" or "Offering"</summary>
    public string IncomeType { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime EntryDate { get; set; }

    /// <summary>FK to Profile (the staff member who recorded this entry)</summary>
    public int RecordedBy { get; set; }

    /// <summary>"Finalized" — income entries are always finalized upon submission</summary>
    public string Status { get; set; } = "Finalized";

    // Navigation properties
    public virtual Profile? Member { get; set; }
    public virtual Profile Recorder { get; set; } = null!;
}
