using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appointment_service.Models;
using appointment_service.DTOs;

namespace appointment_service.Controllers;

[ApiController]
[Route("[controller]")]
public class BudgetAllocationController : ControllerBase
{
    private readonly CmisContext _context;

    public BudgetAllocationController(CmisContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BudgetAllocationReadDto>>> GetAll(
        [FromQuery] int? budgetId = null)
    {
        var query = _context.BudgetAllocations.Include(a => a.Budget).AsQueryable();
        if (budgetId.HasValue) query = query.Where(a => a.BudgetId == budgetId);

        var allocations = await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
        return Ok(allocations.Select(ToReadDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BudgetAllocationReadDto>> GetById(int id)
    {
        var allocation = await _context.BudgetAllocations
            .Include(a => a.Budget)
            .FirstOrDefaultAsync(a => a.AllocationId == id);

        if (allocation == null) return NotFound();
        return Ok(ToReadDto(allocation));
    }

    [HttpPost]
    public async Task<ActionResult<BudgetAllocationReadDto>> Create([FromBody] BudgetAllocationWriteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "Name is required." });

        var allocation = new BudgetAllocation
        {
            Name = dto.Name,
            Allocated = dto.Allocated,
            Spent = dto.Spent,
            BudgetId = dto.BudgetId,
            CreatedAt = DateTime.UtcNow
        };

        _context.BudgetAllocations.Add(allocation);
        await _context.SaveChangesAsync();

        await _context.Entry(allocation).Reference(a => a.Budget).LoadAsync();
        return CreatedAtAction(nameof(GetById), new { id = allocation.AllocationId }, ToReadDto(allocation));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BudgetAllocationReadDto>> Update(int id, [FromBody] BudgetAllocationWriteDto dto)
    {
        var allocation = await _context.BudgetAllocations.FindAsync(id);
        if (allocation == null) return NotFound();

        allocation.Name = dto.Name;
        allocation.Allocated = dto.Allocated;
        allocation.Spent = dto.Spent;
        allocation.BudgetId = dto.BudgetId;

        await _context.SaveChangesAsync();
        await _context.Entry(allocation).Reference(a => a.Budget).LoadAsync();

        return Ok(ToReadDto(allocation));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var allocation = await _context.BudgetAllocations.FindAsync(id);
        if (allocation == null) return NotFound();

        _context.BudgetAllocations.Remove(allocation);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static BudgetAllocationReadDto ToReadDto(BudgetAllocation a) => new()
    {
        AllocationId = a.AllocationId,
        Name = a.Name,
        Allocated = a.Allocated,
        Spent = a.Spent,
        RemainingBalance = a.RemainingBalance,
        Utilization = a.Utilization,
        BudgetId = a.BudgetId,
        BudgetName = a.Budget?.Name,
        CreatedAt = a.CreatedAt
    };
}
