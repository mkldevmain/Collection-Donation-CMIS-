namespace CMIS.Components.Pages.Ministry_Head.Collection_Donation;

public record DonationEntry(string Id, string DonorName, string Type, decimal Amount, DateTime Date);
public record ExpenseEntry(string Id, string Title, string Category, decimal Amount, string RefNo, DateTime Date);
public record RecurringEntry(
    int Id,
    int MemberId,
    string MemberName,
    decimal ExpectedAmount,
    string Frequency,
    DateTime StartDate,
    DateTime NextDue,
    DateTime? EndDate,
    decimal? TargetAmount,
    decimal EstimatedPaid,
    decimal RemainingBalance,
    string Status);
