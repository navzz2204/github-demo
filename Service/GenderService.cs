using ToyStoreOnlineWeb.Data;
using ToyStoreOnlineWeb.Models;

namespace ToyStoreOnlineWeb.Service
{
    public interface IGenderService
    {
        IEnumerable<Gender> GetGenderList();
        Gender GetGenderByID(int ID);
    }
    public class GenderService : IGenderService
    {
        private readonly UnitOfWork context;
        public GenderService(UnitOfWork repositoryContext)
        {
            this.context = repositoryContext;
        }
        public IEnumerable<Gender> GetGenderList()
        {
            IEnumerable<Gender> listGender = this.context.GenderRepository.GetAllData();
            return listGender;
        }

        public Gender GetGenderByID(int ID)
        {
            return this.context.GenderRepository.GetDataByID(ID);
        }
    }
}
