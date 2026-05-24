using Fluxor;
using CMIS.Data;
using CMIS.Models;
using CMIS.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMIS.Store.Audit;

public class AuditEffects
{
    private readonly IFinancialService _financialService;

    public AuditEffects(IFinancialService financialService)
    {
        _financialService = financialService;
    }

    [EffectMethod]
    public async Task HandleLoadAuditDataAction(LoadAuditDataAction action, IDispatcher dispatcher)
    {
        try
        {
            // 1. Get Live Summary (Always include this so new entries reflect immediately)
            var live = await _financialService.GetLiveSummaryAsync();
            var liveSummary = new FinancialSummary
            {
                Id = "SUM-LIVE",
                Period = live.SummaryPeriod,
                TotalIncome = live.TotalIncome,
                TotalExpenses = live.TotalExpenses,
                NetBalance = live.NetBalance,
                GeneratedBy = "System (Live)",
                FinalizationDate = live.GeneratedDate,
                Status = "Live"
            };

            // 2. Load historical summaries
            var dbSummaries = await _financialService.GetSummariesAsync();
            var summaries = new List<FinancialSummary> { liveSummary };

            if (dbSummaries.Any())
            {
                summaries.AddRange(dbSummaries.Select(s => new FinancialSummary
                {
                    Id = $"SUM-{s.SummaryId}",
                    Period = s.SummaryPeriod,
                    TotalIncome = s.TotalIncome,
                    TotalExpenses = s.TotalExpenses,
                    NetBalance = s.NetBalance,
                    GeneratedBy = s.Generator != null ? $"{s.Generator.FirstName} {s.Generator.LastName}" : "System",
                    FinalizationDate = s.GeneratedDate,
                    Status = "Finalized"
                }));
            }

            // Load real audit logs from service
            var dbAuditLogs = await _financialService.GetAuditLogsAsync(20);

            var auditLogs = dbAuditLogs.Select(a => new AuditLogEntry
            {
                LogId = $"LOG-{a.AuditId}",
                ActionType = a.ActionType,
                Category = "Financial",
                ModifiedBy = a.User != null ? $"{a.User.FirstName} {a.User.LastName}" : "System",
                Timestamp = a.ActionDate,
                PreviousValue = "N/A",
                UpdatedValue = a.ActionDescription,
                Source = "Financial Module",
                ReferenceTable = a.ReferenceTable,
                IntegrityStatus = a.Status
            }).ToList();

            var metrics = new ComplianceMetrics
            {
                AuditCompletionRate = auditLogs.Any()
                    ? (double)auditLogs.Count(l => l.IntegrityStatus == "Successful") / auditLogs.Count * 100
                    : 100,
                RecordIntegrityVerified = auditLogs.All(l => l.IntegrityStatus == "Successful"),
                TransparencyStatus = "High",
                SummaryFrequencyDays = 30,
                LastAuditDate = dbAuditLogs.Any() ? dbAuditLogs.Max(a => a.ActionDate) : DateTime.Now
            };

            dispatcher.Dispatch(new LoadAuditDataSuccessAction(summaries, auditLogs, metrics));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadAuditDataFailureAction(ex.Message));
        }
    }
}
