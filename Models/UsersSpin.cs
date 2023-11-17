using System;
using System.Collections.Generic;

namespace ToyStoreOnlineWeb.Models;

public partial class UsersSpin
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? NumberOfSpins { get; set; }

    public virtual User? User { get; set; }
}
