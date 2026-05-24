using CMIS.Models;
using CMIS.Services;
using Microsoft.AspNetCore.Mvc;

namespace CMIS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FinancialController : ControllerBase
{
    private readonly IFinancialService _financialService;

    public FinancialController(IFinancialService financialService)
    {
        _financialService = financialService;
    }

    [HttpGet("incomes")]
    public async Task<ActionResult<List<FinancialIncome>>> GetIncomes()
    {
        var incomes = await _financialService.GetIncomesAsync();
        return Ok(incomes);
    }

    [HttpPost("incomes")]
    public async Task<ActionResult<FinancialIncome>> RecordIncome([FromBody] FinancialIncomeRequest request)
    {
        var income = new FinancialIncome
        {
            MemberId = request.MemberId,
            IncomeType = request.IncomeType,
            Amount = request.Amount,
            EntryDate = request.EntryDate ?? DateTime.Now,
            RecordedBy = request.UserId,
            Status = "Finalized"
        };

        var result = await _financialService.RecordIncomeAsync(income, request.UserId, request.DonorName);
        return Ok(result);
    }

    [HttpGet("expenses")]
    public async Task<ActionResult<List<FinancialExpense>>> GetExpenses()
    {
        var expenses = await _financialService.GetExpensesAsync();
        return Ok(expenses);
    }

    [HttpGet("audit-logs")]
    public async Task<ActionResult<List<AuditLog>>> GetAuditLogs([FromQuery] int limit = 20)
    {
        var logs = await _financialService.GetAuditLogsAsync(limit);
        return Ok(logs);
    }

    [HttpGet("summary/live")]
    public async Task<ActionResult<RecordSummary>> GetLiveSummary()
    {
        var summary = await _financialService.GetLiveSummaryAsync();
        return Ok(summary);
    }
    [HttpPost("recurring")]
    public async Task<ActionResult<RecurringTithe>> RecordRecurringTithe([FromBody] RecurringTitheRequest request)
    {
        var tithe = new RecurringTithe
        {
            MemberId = request.MemberId,
            Amount = request.Amount,
            Frequency = request.Frequency,
            StartDate = request.StartDate ?? DateOnly.FromDateTime(DateTime.Today),
            NextDueDate = request.StartDate ?? DateOnly.FromDateTime(DateTime.Today),
            CreatedBy = request.UserId,
            CreatedDate = DateTime.Now,
            Status = "Active"
        };

        var result = await _financialService.RecordRecurringTitheAsync(tithe, request.UserId);
        return Ok(result);
    }
}

public class FinancialIncomeRequest
{
    public int? MemberId { get; set; }
    public string DonorName { get; set; } = "";
    public string IncomeType { get; set; } = "Tithe";
    public decimal Amount { get; set; }
    public DateTime? EntryDate { get; set; }
    public int UserId { get; set; }
}

public class RecurringTitheRequest
{
    public int MemberId { get; set; }
    public decimal Amount { get; set; }
    public string Frequency { get; set; } = "Monthly";
    public DateOnly? StartDate { get; set; }
    public int UserId { get; set; }
}
