using System;
using System.Collections.Generic;

namespace ToyStoreOnlineWeb.Models;

public partial class AccessTimesCount
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int AccessTimes { get; set; }
}
