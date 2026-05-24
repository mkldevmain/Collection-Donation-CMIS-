using System.ComponentModel.DataAnnotations.Schema;

namespace CMIS.Models;

public partial class Budget
{
    public int BudgetId { get; set; }

    public string Name { get; set; } = null!;

    public string Level { get; set; } = null!;

    public int StartYear { get; set; }

    public int EndYear { get; set; }

    [NotMapped]
    public string FiscalYear => $"{StartYear}–{EndYear}";

    public decimal TotalAmount { get; set; }

    public int? ChurchId { get; set; }

    public int? DistrictId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation
    public virtual Church? Church { get; set; }
    public virtual District? District { get; set; }
    public virtual ICollection<BudgetAllocation> Allocations { get; set; } = new List<BudgetAllocation>();
}
