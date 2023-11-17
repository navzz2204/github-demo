using System;
using System.Collections.Generic;

namespace ToyStoreOnlineWeb.Models;

public partial class Message
{
    public int Id { get; set; }

    public int? FromUserId { get; set; }

    public int? ToUserId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? Sent { get; set; }

    public virtual User? FromUser { get; set; }

    public virtual User? ToUser { get; set; }
}
