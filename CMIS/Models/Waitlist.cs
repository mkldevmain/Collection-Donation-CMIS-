namespace CMIS.Models;

public class Waitlist
{
    public int WaitlistId { get; set; }
    public int ProfileId { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }

    public virtual Profile Profile { get; set; } = null!;
}