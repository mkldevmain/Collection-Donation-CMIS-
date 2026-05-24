using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appointment_service.Models;
using appointment_service.DTOs;

namespace appointment_service.Controllers;

[ApiController]
[Route("[controller]")]
public class BudgetController : ControllerBase
{
    private readonly CmisContext _context;

    public BudgetController(CmisContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BudgetReadDto>>> GetAll(
        [FromQuery] int? churchId = null,
        [FromQuery] int? districtId = null,
        [FromQuery] string? level = null)
    {
        var query = _context.Budgets
            .Include(b => b.Church)
            .Include(b => b.District)
            .AsQueryable();

        if (churchId.HasValue) query = query.Where(b => b.ChurchId == churchId);
        if (districtId.HasValue) query = query.Where(b => b.DistrictId == districtId);
        if (!string.IsNullOrEmpty(level)) query = query.Where(b => b.Level == level);

        var budgets = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
        return Ok(budgets.Select(ToReadDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BudgetReadDto>> GetById(int id)
    {
        var budget = await _context.Budgets
            .Include(b => b.Church)
            .Include(b => b.District)
            .FirstOrDefaultAsync(b => b.BudgetId == id);

        if (budget == null) return NotFound();
        return Ok(ToReadDto(budget));
    }

    [HttpPost]
    public async Task<ActionResult<BudgetReadDto>> Create([FromBody] BudgetWriteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Level))
            return BadRequest(new { message = "Name and Level are required." });

        var budget = new Budget
        {
            Name = dto.Name,
            Level = dto.Level,
            StartYear = dto.StartYear,
            EndYear = dto.EndYear,
            TotalAmount = dto.TotalAmount,
            ChurchId = dto.ChurchId,
            DistrictId = dto.DistrictId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        await _context.Entry(budget).Reference(b => b.Church).LoadAsync();
        await _context.Entry(budget).Reference(b => b.District).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = budget.BudgetId }, ToReadDto(budget));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BudgetReadDto>> Update(int id, [FromBody] BudgetWriteDto dto)
    {
        var budget = await _context.Budgets.FindAsync(id);
        if (budget == null) return NotFound();

        budget.Name = dto.Name;
        budget.Level = dto.Level;
        budget.StartYear = dto.StartYear;
        budget.EndYear = dto.EndYear;
        budget.TotalAmount = dto.TotalAmount;
        budget.ChurchId = dto.ChurchId;
        budget.DistrictId = dto.DistrictId;

        await _context.SaveChangesAsync();

        await _context.Entry(budget).Reference(b => b.Church).LoadAsync();
        await _context.Entry(budget).Reference(b => b.District).LoadAsync();

        return Ok(ToReadDto(budget));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var budget = await _context.Budgets.FindAsync(id);
        if (budget == null) return NotFound();

        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static BudgetReadDto ToReadDto(Budget b) => new()
    {
        BudgetId = b.BudgetId,
        Name = b.Name,
        Level = b.Level,
        StartYear = b.StartYear,
        EndYear = b.EndYear,
        FiscalYear = b.FiscalYear,
        TotalAmount = b.TotalAmount,
        ChurchId = b.ChurchId,
        ChurchName = b.Church?.ChurchName,
        DistrictId = b.DistrictId,
        DistrictName = b.District?.DistrictName,
        CreatedAt = b.CreatedAt
    };
}
