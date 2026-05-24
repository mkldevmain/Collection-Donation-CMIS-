using CMIS.Models;

namespace CMIS.Services;

public interface IFinancialService
{
    // Income
    Task<List<FinancialIncome>> GetIncomesAsync();
    Task<FinancialIncome> RecordIncomeAsync(FinancialIncome income, int userId, string donorName);
    
    // Expenses
    Task<List<FinancialExpense>> GetExpensesAsync();
    Task<FinancialExpense> RecordExpenseAsync(FinancialExpense expense, int userId);
    
    // Recurring
    Task<List<RecurringTithe>> GetRecurringTithesAsync();
    Task<RecurringTithe> RecordRecurringTitheAsync(RecurringTithe tithe, int userId);
    Task<RecurringTithe> UpdateRecurringTitheAsync(RecurringTithe tithe, int userId);
    
    // Profiles/Members
    Task<List<Profile>> GetMembersAsync();
    
    // Audit & Summary
    Task<List<AuditLog>> GetAuditLogsAsync(int limit = 20);
    Task<List<RecordSummary>> GetSummariesAsync();
    Task<RecordSummary> GetLiveSummaryAsync();
}
