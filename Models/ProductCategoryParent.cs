using System;
using System.Collections.Generic;

namespace ToyStoreOnlineWeb.Models;

public partial class ProductCategoryParent
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? SeoKeyword { get; set; }

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}
