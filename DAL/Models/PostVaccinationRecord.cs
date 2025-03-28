using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class PostVaccinationRecord
{
    public int RecordId { get; set; }

    public int ChildId { get; set; }

    public int VaccineId { get; set; }

    public string? ReactionDescription { get; set; }

    public DateTime? ReportDate { get; set; }

    public virtual Child Child { get; set; } = null!;

    public virtual Vaccine Vaccine { get; set; } = null!;
}
