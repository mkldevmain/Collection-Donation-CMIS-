using CMIS.Data;
using CMIS.Models;
using Microsoft.EntityFrameworkCore;

namespace CMIS.Services;

public class FinancialService : IFinancialService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public FinancialService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<FinancialIncome>> GetIncomesAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.FinancialIncomes
            .Include(i => i.Member)
            .AsNoTracking()
            .OrderByDescending(i => i.EntryDate)
            .ToListAsync();
    }

    public async Task<FinancialIncome> RecordIncomeAsync(FinancialIncome income, int userId, string donorName)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.FinancialIncomes.Add(income);
        await db.SaveChangesAsync();

        // Automatic Audit Logging
        db.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            ActionType = "INSERT",
            ActionDescription = $"Recorded {income.IncomeType} income of ₱{income.Amount:N0} from {donorName}",
            ReferenceId = income.IncomeId,
            ReferenceTable = "financial_income",
            ActionDate = DateTime.Now,
            Status = "Successful"
        });
        await db.SaveChangesAsync();

        return income;
    }

    public async Task<List<FinancialExpense>> GetExpensesAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.FinancialExpenses
            .AsNoTracking()
            .OrderByDescending(e => e.DateSpent)
            .ToListAsync();
    }

    public async Task<FinancialExpense> RecordExpenseAsync(FinancialExpense expense, int userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.FinancialExpenses.Add(expense);
        await db.SaveChangesAsync();

        // Automatic Audit Logging
        db.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            ActionType = "INSERT",
            ActionDescription = $"Recorded expense of ₱{expense.Amount:N0} under {expense.Category}: {expense.Description}",
            ReferenceId = expense.ExpenseId,
            ReferenceTable = "expenses",
            ActionDate = DateTime.Now,
            Status = "Successful"
        });
        await db.SaveChangesAsync();

        return expense;
    }

    public async Task<List<RecurringTithe>> GetRecurringTithesAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.RecurringTithes
            .Include(r => r.Member)
            .AsNoTracking()
            .OrderBy(r => r.NextDueDate)
            .ToListAsync();
    }

    public async Task<RecurringTithe> RecordRecurringTitheAsync(RecurringTithe tithe, int userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.RecurringTithes.Add(tithe);
        await db.SaveChangesAsync();

        db.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            ActionType = "INSERT",
            ActionDescription = $"Created new recurring schedule for Member ID {tithe.MemberId} at {tithe.Frequency}",
            ReferenceId = tithe.RecurringId,
            ReferenceTable = "recurring_tithes",
            ActionDate = DateTime.Now,
            Status = "Successful"
        });
        await db.SaveChangesAsync();

        return tithe;
    }

    public async Task<RecurringTithe> UpdateRecurringTitheAsync(RecurringTithe tithe, int userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.RecurringTithes.Update(tithe);
        await db.SaveChangesAsync();

        db.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            ActionType = "UPDATE",
            ActionDescription = $"Updated recurring schedule {tithe.RecurringId} (Status: {tithe.Status})",
            ReferenceId = tithe.RecurringId,
            ReferenceTable = "recurring_tithes",
            ActionDate = DateTime.Now,
            Status = "Successful"
        });
        await db.SaveChangesAsync();

        return tithe;
    }

    public async Task<List<Profile>> GetMembersAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Profiles
            .Where(p => p.ProfileStatus == "Active")
            .AsNoTracking()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<List<AuditLog>> GetAuditLogsAsync(int limit = 20)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.AuditLogs
            .Include(a => a.User)
            .AsNoTracking()
            .OrderByDescending(a => a.ActionDate)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<RecordSummary>> GetSummariesAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.RecordSummaries
            .Include(s => s.Generator)
            .AsNoTracking()
            .OrderByDescending(s => s.GeneratedDate)
            .ToListAsync();
    }

    public async Task<RecordSummary> GetLiveSummaryAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var totalIncome = await db.FinancialIncomes.SumAsync(i => (decimal?)i.Amount) ?? 0;
        var totalExpenses = await db.FinancialExpenses.SumAsync(e => (decimal?)e.Amount) ?? 0;

        return new RecordSummary
        {
            SummaryId = 0,
            SummaryPeriod = DateTime.Now.ToString("MMMM yyyy"),
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            NetBalance = totalIncome - totalExpenses,
            GeneratedDate = DateTime.Now
        };
    }
}
