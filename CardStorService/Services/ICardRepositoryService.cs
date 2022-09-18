using CardStorageService.Data;

namespace CardStorService.Services
{
    public interface ICardRepositoryService : IRepository<Card, string>
    {
        IList<Card> GetByClientId(string id);    
    }
}
