using ToyStoreOnlineWeb.Models;
using ToyStoreOnlineWeb.Service;
using ToyStoreOnlineWeb.Data;

namespace ToyStoreOnlineWeb.ViewModels
{
    public class MenuViewModel
    {
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
        public IEnumerable<Producer> Producers { get; set; }
        public IEnumerable<Age> Ages { get; set; }
        public IEnumerable<ProductCategoryParent> ProductCategoryParents { get; set; }
        public IEnumerable<Gender> Genders { get; set; }
    }
}
