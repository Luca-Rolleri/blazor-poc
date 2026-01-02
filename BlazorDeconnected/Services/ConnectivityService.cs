namespace BlazorDeconnected.Services
{
    public sealed class ConnectivityService : IConnectivityService
    {
        public bool IsOnline { get; set; } = true;
        public event EventHandler<bool>? StatusChanged;

        public void NotifyStatusChanged(bool isOnlineStatus)
        {
            if (IsOnline == isOnlineStatus) return;
            IsOnline = isOnlineStatus;
            StatusChanged?.Invoke(this, isOnlineStatus);
        }
    }
}
