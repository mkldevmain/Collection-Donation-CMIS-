using System.Collections.Generic;

namespace CMIS.Store.Audit
{
    public class LoadAuditDataAction { }

    public class LoadAuditDataSuccessAction
    {
        public IEnumerable<FinancialSummary> Summaries { get; }
        public IEnumerable<AuditLogEntry> AuditLogs { get; }
        public ComplianceMetrics Metrics { get; }

        public LoadAuditDataSuccessAction(IEnumerable<FinancialSummary> summaries, IEnumerable<AuditLogEntry> auditLogs, ComplianceMetrics metrics)
        {
            Summaries = summaries;
            AuditLogs = auditLogs;
            Metrics = metrics;
        }
    }

    public class LoadAuditDataFailureAction
    {
        public string ErrorMessage { get; }
        public LoadAuditDataFailureAction(string errorMessage) => ErrorMessage = errorMessage;
    }
}
