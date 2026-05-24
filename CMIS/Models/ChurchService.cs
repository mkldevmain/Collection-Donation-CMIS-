using System;
using System.Collections.Generic;

namespace CMIS.Models;

public class ChurchService
{
    public int ServiceId { get; set; }
    public int ChurchId { get; set; } // Foreign Key to Church
    public string DayOfWeek { get; set; } = null!; // e.g., "Sunday"
    public TimeOnly ServiceTime { get; set; } // Standard Time type
    
    public virtual Church Church { get; set; } = null!;
}