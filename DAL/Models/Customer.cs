using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string CustomerFullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Address { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Child> Children { get; set; } = new List<Child>();
}
