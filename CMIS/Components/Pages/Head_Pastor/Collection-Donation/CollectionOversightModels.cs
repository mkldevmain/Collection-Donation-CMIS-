namespace CMIS.Components.Pages.Head_Pastor.Collection_Donation;

public class FinancialRecord
{
    public string Id { get; set; } = "";
    public string Ministry { get; set; } = "";
    public string Type { get; set; } = "";
    public string Category { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = "";
}

public class MinistryActivity
{
    public string Name { get; set; } = "";
    public decimal Donations { get; set; }
    public decimal Expenses { get; set; }
}

public class RecurringEntry
{
    public string MemberName { get; set; } = "";
    public string Frequency { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime NextDueDate { get; set; }
    public string Status { get; set; } = "";
}
