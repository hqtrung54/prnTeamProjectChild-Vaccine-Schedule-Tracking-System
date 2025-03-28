using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Child
{
    public int ChildId { get; set; }

    public int CustomerId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string Gender { get; set; } = null!;

    public string? MedicalHistory { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<PostVaccinationRecord> PostVaccinationRecords { get; set; } = new List<PostVaccinationRecord>();
}
