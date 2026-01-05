namespace Api.Services
{
    public class CounterRepository : ICounterRepository
    {
        private int _count = 0;

        public int GetCount()
        {
            return _count;
        }

        public void SaveCount(int count)
        {
            _count = count;
        }
    }
}
