using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ToyStoreOnlineWeb.Data.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        void Insert(T Model); // 
        IEnumerable<T> GetAllData(); // R

        IEnumerable<T> GetAllData(Expression<Func<T, bool>> where); // R

        T GetDataByID(int ID); // R

        void Update(T Model); //U

        void Remove(T Model); //D

        void Save();
    }
}
