using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public string TargetGroup { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }
}
