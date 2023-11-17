using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToyStoreOnlineWeb.Data;
using ToyStoreOnlineWeb.Models;

namespace ToyStoreOnlineWeb.Service
{
    public interface IProductViewedService
    {
        void AddProductViewByUser(int productID, int userID);
        void Delete(int userID);
    }
    public class ProductViewedService : IProductViewedService
    {
        private readonly UnitOfWork context;
        public ProductViewedService(UnitOfWork repositoryContext)
        {
            this.context = repositoryContext;
        }
        public void AddProductViewByUser(int productID, int userID)
        {
            try
            {
                ProductViewed productVieweds = context.ProductViewedRepository.GetAllData().Single(x => x.ProductId == productID && x.UserId == userID);
                if (productVieweds != null)
                {
                    productVieweds.Date = DateTime.Now;
                    context.ProductViewedRepository.Update(productVieweds);
                }
            }
            catch (Exception)
            {
                ProductViewed productViewed = new ProductViewed();
                productViewed.ProductId = productID;
                productViewed.UserId = userID;
                productViewed.Date = DateTime.Now;
                context.ProductViewedRepository.Insert(productViewed);
            }
        }

        public void Delete(int userID)
        {
            IEnumerable<ProductViewed> productViewed = context.ProductViewedRepository.GetAllData(x => x.UserId == userID);
            foreach (var item in productViewed)
            {
                context.ProductViewedRepository.Remove(item);
            }
        }
    }
}