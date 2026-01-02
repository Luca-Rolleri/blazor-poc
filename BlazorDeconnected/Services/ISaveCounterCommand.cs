namespace BlazorDeconnected.Services
{
    public interface ISaveCounterCommand
    {
        Task ExecuteAsync(int counterValue, CancellationToken cancellationToken = default);
    }
}
