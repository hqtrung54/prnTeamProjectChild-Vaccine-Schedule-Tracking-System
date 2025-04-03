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

    public int VaccineId { get; set; } // Thêm VaccineID để phản ánh khóa ngoại

    // Liên kết với bảng Vaccine (Navigation Property)
    public virtual Vaccine? Vaccine { get; set; }

}
