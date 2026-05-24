using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appointment_service.Models;
using appointment_service.DTOs;

namespace appointment_service.Controllers;

[ApiController]
[Route("[controller]")]
public class BudgetProposalController : ControllerBase
{
    private readonly CmisContext _context;

    public BudgetProposalController(CmisContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BudgetProposalReadDto>>> GetAll(
        [FromQuery] int? ministryId = null,
        [FromQuery] int? churchId = null,
        [FromQuery] int? districtId = null,
        [FromQuery] string? status = null,
        [FromQuery] string? level = null)
    {
        var query = BaseQuery();

        if (ministryId.HasValue) query = query.Where(p => p.MinistryId == ministryId);
        if (churchId.HasValue) query = query.Where(p => p.ChurchId == churchId);
        if (districtId.HasValue) query = query.Where(p => p.DistrictId == districtId);
        if (!string.IsNullOrEmpty(status)) query = query.Where(p => p.Status == status);
        if (!string.IsNullOrEmpty(level)) query = query.Where(p => p.Level == level);

        var proposals = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return Ok(proposals.Select(ToReadDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BudgetProposalReadDto>> GetById(int id)
    {
        var proposal = await BaseQuery().FirstOrDefaultAsync(p => p.ProposalId == id);
        if (proposal == null) return NotFound();
        return Ok(ToReadDto(proposal));
    }

    [HttpPost]
    public async Task<ActionResult<BudgetProposalReadDto>> Create([FromBody] BudgetProposalCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ProposalCode) || string.IsNullOrWhiteSpace(dto.Purpose)
            || string.IsNullOrWhiteSpace(dto.Level) || dto.SubmittedById <= 0)
            return BadRequest(new { message = "ProposalCode, Purpose, Level, SubmittedById are required." });

        var exists = await _context.BudgetProposals.AnyAsync(p => p.ProposalCode == dto.ProposalCode);
        if (exists) return Conflict(new { message = "ProposalCode already exists." });

        var proposal = new BudgetProposal
        {
            ProposalCode = dto.ProposalCode,
            Purpose = dto.Purpose,
            Description = dto.Description,
            MinistryId = dto.MinistryId,
            ChurchId = dto.ChurchId,
            DistrictId = dto.DistrictId,
            Level = dto.Level,
            Amount = dto.Amount,
            Status = "Pending",
            SubmittedById = dto.SubmittedById,
            CreatedAt = DateTime.UtcNow
        };

        _context.BudgetProposals.Add(proposal);
        await _context.SaveChangesAsync();

        var saved = await BaseQuery().FirstAsync(p => p.ProposalId == proposal.ProposalId);
        return CreatedAtAction(nameof(GetById), new { id = saved.ProposalId }, ToReadDto(saved));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BudgetProposalReadDto>> Update(int id, [FromBody] BudgetProposalUpdateDto dto)
    {
        var proposal = await _context.BudgetProposals.FindAsync(id);
        if (proposal == null) return NotFound();

        if (proposal.Status != "Pending" && proposal.Status != "For Revision")
            return BadRequest(new { message = $"Cannot edit a proposal in status '{proposal.Status}'." });

        if (!string.IsNullOrWhiteSpace(dto.Purpose)) proposal.Purpose = dto.Purpose;
        if (dto.Description != null) proposal.Description = dto.Description;
        if (dto.Amount.HasValue) proposal.Amount = dto.Amount.Value;
        proposal.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        var saved = await BaseQuery().FirstAsync(p => p.ProposalId == id);
        return Ok(ToReadDto(saved));
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<BudgetProposalReadDto>> ChangeStatus(int id, [FromBody] BudgetProposalStatusDto dto)
    {
        var allowed = new[] { "Approved", "Disapproved", "For Revision", "Pending" };
        if (!allowed.Contains(dto.Status))
            return BadRequest(new { message = $"Status must be one of: {string.Join(", ", allowed)}." });

        var proposal = await _context.BudgetProposals.FindAsync(id);
        if (proposal == null) return NotFound();

        proposal.Status = dto.Status;
        proposal.ReviewedById = dto.ReviewedById;
        proposal.RejectionReason = dto.Status == "Disapproved" || dto.Status == "For Revision"
            ? dto.RejectionReason
            : null;
        proposal.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        var saved = await BaseQuery().FirstAsync(p => p.ProposalId == id);
        return Ok(ToReadDto(saved));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var proposal = await _context.BudgetProposals.FindAsync(id);
        if (proposal == null) return NotFound();

        _context.BudgetProposals.Remove(proposal);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private IQueryable<BudgetProposal> BaseQuery() => _context.BudgetProposals
        .Include(p => p.Ministry)
        .Include(p => p.Church)
        .Include(p => p.District)
        .Include(p => p.SubmittedBy)
        .Include(p => p.ReviewedBy);

    private static BudgetProposalReadDto ToReadDto(BudgetProposal p) => new()
    {
        ProposalId = p.ProposalId,
        ProposalCode = p.ProposalCode,
        Purpose = p.Purpose,
        Description = p.Description,
        MinistryId = p.MinistryId,
        MinistryName = p.Ministry?.MinistryName,
        ChurchId = p.ChurchId,
        ChurchName = p.Church?.ChurchName,
        DistrictId = p.DistrictId,
        DistrictName = p.District?.DistrictName,
        Level = p.Level,
        Amount = p.Amount,
        Status = p.Status,
        RejectionReason = p.RejectionReason,
        SubmittedById = p.SubmittedById,
        SubmittedByFirstName = p.SubmittedBy?.FirstName,
        SubmittedByLastName = p.SubmittedBy?.LastName,
        SubmittedByName = FullName(p.SubmittedBy),
        ReviewedById = p.ReviewedById,
        ReviewedByName = FullName(p.ReviewedBy),
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };

    private static string? FullName(Profile? profile)
    {
        if (profile == null) return null;
        var parts = new[] { profile.FirstName, profile.MiddleName, profile.LastName }
            .Where(s => !string.IsNullOrWhiteSpace(s));
        return string.Join(" ", parts);
    }
}
