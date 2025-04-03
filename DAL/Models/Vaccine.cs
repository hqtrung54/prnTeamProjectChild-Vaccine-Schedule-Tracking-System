using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Vaccine
{
    public int VaccineId { get; set; }

    public string VaccineName { get; set; } = null!;

    public string Manufacturer { get; set; } = null!;

    public string? Description { get; set; }

    public string? AgeGroup { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<PostVaccinationRecord> PostVaccinationRecords { get; set; } = new List<PostVaccinationRecord>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
