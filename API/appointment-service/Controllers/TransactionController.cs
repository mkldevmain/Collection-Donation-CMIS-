using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appointment_service.Models;
using appointment_service.DTOs;

namespace appointment_service.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionController : ControllerBase
{
    private readonly CmisContext _context;

    public TransactionController(CmisContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionReadDto>>> GetAll(
        [FromQuery] int? budgetAllocationId = null,
        [FromQuery] string? type = null,
        [FromQuery] string? fromDate = null,
        [FromQuery] string? toDate = null)
    {
        var query = _context.Transactions
            .Include(t => t.RecordedBy)
            .Include(t => t.Allocation)
                .ThenInclude(a => a!.Budget)
            .AsQueryable();

        if (budgetAllocationId.HasValue && budgetAllocationId.Value > 0)
            query = query.Where(t => t.BudgetAllocationId == budgetAllocationId);
        if (!string.IsNullOrEmpty(type)) query = query.Where(t => t.Type == type);

        if (!string.IsNullOrEmpty(fromDate) && DateOnly.TryParse(fromDate, out var from))
            query = query.Where(t => t.TransactionDate >= from);
        if (!string.IsNullOrEmpty(toDate) && DateOnly.TryParse(toDate, out var to))
            query = query.Where(t => t.TransactionDate <= to);

        var transactions = await query
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.TransactionId)
            .ToListAsync();
        return Ok(transactions.Select(ToReadDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionReadDto>> GetById(int id)
    {
        var transaction = await _context.Transactions
            .Include(t => t.RecordedBy)
            .Include(t => t.Allocation)
                .ThenInclude(a => a!.Budget)
            .FirstOrDefaultAsync(t => t.TransactionId == id);

        if (transaction == null) return NotFound();
        return Ok(ToReadDto(transaction));
    }

    [HttpPost]
    public async Task<ActionResult<TransactionReadDto>> Create([FromBody] TransactionWriteDto dto)
    {
        // Normalize 0 → null so 0 isn't treated as a real FK
        var allocId = dto.BudgetAllocationId is > 0 ? dto.BudgetAllocationId : null;

        var validationError = await ValidateAsync(dto, allocId);
        if (validationError != null) return validationError;

        var exists = await _context.Transactions.AnyAsync(t => t.TransactionCode == dto.TransactionCode);
        if (exists) return Conflict(new { message = "TransactionCode already exists." });

        var transaction = new Transaction
        {
            TransactionCode = dto.TransactionCode,
            Description = dto.Description ?? string.Empty,
            Type = dto.Type,
            BudgetAllocationId = allocId,
            BudgetLabel = dto.BudgetLabel,
            Amount = dto.Amount,
            RecordedById = dto.RecordedById,
            TransactionDate = dto.TransactionDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await ApplyAllocationDelta(transaction, oldAllocationId: null, oldAmount: 0, oldType: null);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(new { message = "Could not save transaction.", detail = ex.InnerException?.Message ?? ex.Message });
        }

        var saved = await _context.Transactions
            .Include(t => t.RecordedBy)
            .Include(t => t.Allocation)
                .ThenInclude(a => a!.Budget)
            .FirstAsync(t => t.TransactionId == transaction.TransactionId);

        return CreatedAtAction(nameof(GetById), new { id = saved.TransactionId }, ToReadDto(saved));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TransactionReadDto>> Update(int id, [FromBody] TransactionWriteDto dto)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null) return NotFound();

        var allocId = dto.BudgetAllocationId is > 0 ? dto.BudgetAllocationId : null;

        var validationError = await ValidateAsync(dto, allocId);
        if (validationError != null) return validationError;

        var codeConflict = await _context.Transactions
            .AnyAsync(t => t.TransactionCode == dto.TransactionCode && t.TransactionId != id);
        if (codeConflict) return Conflict(new { message = "TransactionCode already in use by another transaction." });

        var oldAllocationId = transaction.BudgetAllocationId;
        var oldAmount = transaction.Amount;
        var oldType = transaction.Type;

        transaction.TransactionCode = dto.TransactionCode;
        transaction.Description = dto.Description ?? string.Empty;
        transaction.Type = dto.Type;
        transaction.BudgetAllocationId = allocId;
        transaction.BudgetLabel = dto.BudgetLabel;
        transaction.Amount = dto.Amount;
        transaction.RecordedById = dto.RecordedById;
        transaction.TransactionDate = dto.TransactionDate;

        await ApplyAllocationDelta(transaction, oldAllocationId, oldAmount, oldType);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(new { message = "Could not update transaction.", detail = ex.InnerException?.Message ?? ex.Message });
        }

        var saved = await _context.Transactions
            .Include(t => t.RecordedBy)
            .Include(t => t.Allocation)
                .ThenInclude(a => a!.Budget)
            .FirstAsync(t => t.TransactionId == id);

        return Ok(ToReadDto(saved));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null) return NotFound();

        // Reverse the spent delta before deletion
        if (transaction.BudgetAllocationId.HasValue && transaction.Type == "Expense")
        {
            var allocation = await _context.BudgetAllocations.FindAsync(transaction.BudgetAllocationId.Value);
            if (allocation != null) allocation.Spent = Math.Max(0, allocation.Spent - transaction.Amount);
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<ActionResult?> ValidateAsync(TransactionWriteDto dto, int? allocId)
    {
        if (string.IsNullOrWhiteSpace(dto.TransactionCode))
            return BadRequest(new { message = "TransactionCode is required." });
        if (string.IsNullOrWhiteSpace(dto.Type) || (dto.Type != "Income" && dto.Type != "Expense"))
            return BadRequest(new { message = "Type must be 'Income' or 'Expense'." });
        if (dto.RecordedById <= 0)
            return BadRequest(new { message = "RecordedById is required." });
        if (dto.Amount <= 0)
            return BadRequest(new { message = "Amount must be greater than zero." });

        var profileExists = await _context.Profiles.AnyAsync(p => p.ProfileId == dto.RecordedById);
        if (!profileExists)
            return BadRequest(new { message = $"No profile found for RecordedById = {dto.RecordedById}." });

        if (allocId.HasValue)
        {
            var allocExists = await _context.BudgetAllocations.AnyAsync(a => a.AllocationId == allocId.Value);
            if (!allocExists)
                return BadRequest(new { message = $"No allocation found for BudgetAllocationId = {allocId.Value}." });
        }

        return null;
    }

    // Maintains BudgetAllocation.Spent so the UI reflects current usage.
    // Only "Expense" transactions linked to an allocation affect Spent.
    private async Task ApplyAllocationDelta(Transaction current, int? oldAllocationId, decimal oldAmount, string? oldType)
    {
        if (oldAllocationId.HasValue && oldType == "Expense")
        {
            var oldAlloc = await _context.BudgetAllocations.FindAsync(oldAllocationId.Value);
            if (oldAlloc != null) oldAlloc.Spent = Math.Max(0, oldAlloc.Spent - oldAmount);
        }

        if (current.BudgetAllocationId.HasValue && current.Type == "Expense")
        {
            var newAlloc = await _context.BudgetAllocations.FindAsync(current.BudgetAllocationId.Value);
            if (newAlloc != null) newAlloc.Spent += current.Amount;
        }
    }

    private static TransactionReadDto ToReadDto(Transaction t) => new()
    {
        TransactionId = t.TransactionId,
        TransactionCode = t.TransactionCode,
        Description = t.Description,
        Type = t.Type,
        BudgetAllocationId = t.BudgetAllocationId,
        AllocationName = t.Allocation?.Name,
        AllocationBudgetName = t.Allocation?.Budget?.Name,
        BudgetLabel = t.BudgetLabel,
        Amount = t.Amount,
        RecordedById = t.RecordedById,
        RecordedByFirstName = t.RecordedBy?.FirstName,
        RecordedByLastName = t.RecordedBy?.LastName,
        RecordedByName = t.RecordedBy != null
            ? $"{t.RecordedBy.FirstName} {t.RecordedBy.MiddleName} {t.RecordedBy.LastName}".Replace("  ", " ").Trim()
            : null,
        TransactionDate = t.TransactionDate,
        CreatedAt = t.CreatedAt
    };
}
