namespace Api.Services
{
    public interface ICounterRepository
    {
        int GetCount();
        void SaveCount(int count);
    }
}
