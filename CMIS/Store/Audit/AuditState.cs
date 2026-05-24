using Fluxor;
using System;
using System.Collections.Generic;

namespace CMIS.Store.Audit
{
    [FeatureState]
    public class AuditState
    {
        public bool IsLoading { get; init; }
        public IEnumerable<FinancialSummary> Summaries { get; init; }
        public IEnumerable<AuditLogEntry> AuditLogs { get; init; }
        public ComplianceMetrics Metrics { get; init; }

        public AuditState() 
        {
            IsLoading = false;
            Summaries = new List<FinancialSummary>();
            AuditLogs = new List<AuditLogEntry>();
            Metrics = new ComplianceMetrics();
        }

        public AuditState(bool isLoading, IEnumerable<FinancialSummary> summaries, IEnumerable<AuditLogEntry> auditLogs, ComplianceMetrics metrics)
        {
            IsLoading = isLoading;
            Summaries = summaries;
            AuditLogs = auditLogs;
            Metrics = metrics;
        }
    }

    public class FinancialSummary
    {
        public string Id { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetBalance { get; set; }
        public bool RecurringTithesIncluded { get; set; }
        public string GeneratedBy { get; set; } = string.Empty;
        public DateTime FinalizationDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class AuditLogEntry
    {
        public string LogId { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string PreviousValue { get; init; } = string.Empty;
        public string UpdatedValue { get; init; } = string.Empty;
        public string Source { get; init; } = string.Empty;
        public string ReferenceTable { get; init; } = string.Empty;
        public string IntegrityStatus { get; init; } = string.Empty;
    }

    public class ComplianceMetrics
    {
        public double AuditCompletionRate { get; set; }
        public bool RecordIntegrityVerified { get; set; }
        public string TransparencyStatus { get; set; } = string.Empty;
        public int SummaryFrequencyDays { get; set; }
        public DateTime LastAuditDate { get; set; }
    }
}
