using System;

namespace CMIS.Models;

/// <summary>
/// Maps to the audit_logs table.
/// Immutable log of all Insert, Update, and Delete actions on financial records.
/// </summary>
public class AuditLog
{
    public int AuditId { get; set; }

    /// <summary>FK to Profile (the staff member who performed the action)</summary>
    public int UserId { get; set; }

    /// <summary>"Insert", "Update", or "Delete"</summary>
    public string ActionType { get; set; } = null!;

    /// <summary>Human-readable description of what changed</summary>
    public string ActionDescription { get; set; } = null!;

    /// <summary>ID of the affected FinancialIncome or FinancialExpense record</summary>
    public int ReferenceId { get; set; }

    /// <summary>Name of the affected table</summary>
    public string ReferenceTable { get; set; } = null!;

    public DateTime ActionDate { get; set; }

    /// <summary>"Successful" or "Failed"</summary>
    public string Status { get; set; } = "Successful";

    // Navigation
    public virtual Profile User { get; set; } = null!;
}
