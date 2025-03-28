using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int? Role { get; set; }

    public int? CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }
}
