namespace BlazorDeconnected.Services
{
    public interface IConnectivityService
    {
        bool IsOnline { get; }

        event EventHandler<bool>? StatusChanged;

        void NotifyStatusChanged(bool isOnline);

    }
}
