namespace CMIS.Models;

public class Attendance
{
    public int AttendanceId { get; set; }
    public int ProfileId { get; set; }
    public int ChurchId { get; set; }
    public DateOnly AttendanceDate { get; set; }

    public virtual Profile Profile { get; set; } = null!;
    public virtual Church Church { get; set; } = null!;
}