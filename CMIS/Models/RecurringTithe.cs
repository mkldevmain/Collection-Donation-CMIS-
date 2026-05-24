using System;

namespace CMIS.Models;

/// <summary>
/// Maps to the recurring_tithes table.
/// Represents a scheduled recurring tithe commitment by a church member.
/// </summary>
public class RecurringTithe
{
    public int RecurringId { get; set; }

    /// <summary>FK to Profile (the church member with the recurring commitment)</summary>
    public int MemberId { get; set; }

    public decimal Amount { get; set; }

    /// <summary>"Weekly" or "Monthly"</summary>
    public string Frequency { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly NextDueDate { get; set; }
    
    public decimal? TargetAmount { get; set; }
    public DateOnly? EndDate { get; set; }

    /// <summary>"Active", "Paused", or "Cancelled"</summary>
    public string Status { get; set; } = "Active";

    /// <summary>FK to Profile (the staff member who created this schedule)</summary>
    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    // Navigation
    public virtual Profile Member { get; set; } = null!;
    public virtual Profile Creator { get; set; } = null!;
}
