using ToyStoreOnlineWeb.Data;
using ToyStoreOnlineWeb.Models;

namespace ToyStoreOnlineWeb.Service
{
    public interface IAgeService
    {
        IEnumerable<Age> GetAgeList();
        Age GetAgeByID(int ID);
    }
    public class AgeService : IAgeService
    {
        private readonly UnitOfWork context;
        public AgeService(UnitOfWork repositoryContext)
        {
            this.context = repositoryContext;
        }
        public IEnumerable<Age> GetAgeList()
        {
            IEnumerable<Age> listAge = this.context.AgeRepository.GetAllData();
            return listAge;
        }

        public Age GetAgeByID(int ID)
        {
            return this.context.AgeRepository.GetDataByID(ID);
        }
    }
}
