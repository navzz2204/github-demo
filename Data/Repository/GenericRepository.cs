using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ToyStoreOnlineWeb.Service;
using ToyStoreOnlineWeb.Models;

namespace ToyStoreOnlineWeb.Data.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private ToyStoreDbContext DbContext;
        private DbSet<T> dbEntity;

        public GenericRepository(ToyStoreDbContext dbContext)
        {
            this.DbContext = dbContext;
            dbEntity = dbContext.Set<T>();
        }

        public void Insert(T model)
        {
            dbEntity.Add(model);
            DbContext.SaveChanges();
        }

        public IEnumerable<T> GetAllData()
        {        
            return dbEntity.ToList();
        }

        public IEnumerable<T> GetAllData(Expression<Func<T, bool>> where)
        {
            return dbEntity.Where(where).ToList();
        }

        public T GetDataByID(int ID)
        {
            return dbEntity.Find(ID);
        }

        public void Update(T model)
        {
            dbEntity.Attach(model);
            DbContext.Entry(model).State = EntityState.Modified;
            DbContext.SaveChanges();
        }
        
        public void Remove(T model)
        {
            dbEntity.Remove(model);
            DbContext.SaveChanges();
        }

        public void Save()
        {
            DbContext.SaveChanges();
        }
    }
}
