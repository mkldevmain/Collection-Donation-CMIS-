namespace appointment_service.DTOs;

public class BudgetReadDto
{
    public int BudgetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public string FiscalYear { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int? ChurchId { get; set; }
    public string? ChurchName { get; set; }
    public int? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BudgetWriteDto
{
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public decimal TotalAmount { get; set; }
    public int? ChurchId { get; set; }
    public int? DistrictId { get; set; }
}
