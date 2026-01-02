
using Microsoft.JSInterop;

namespace BlazorDeconnected.Services
{
    public class NetworkStatusService : IAsyncDisposable
    {
        private readonly IJSRuntime _js;
        private DotNetObjectReference<NetworkStatusService>? _objRef;

        public event Action<bool>? Changed;
        public NetworkStatusService(IJSRuntime js) => _js = js;

        // Méthode d'instance invocable depuis JS
        [JSInvokable("Notify")]
        public void Notify(bool online) => Changed?.Invoke(online);

        public async Task InitializeAsync()
        {
            // Crée une référence .NET vers CETTE instance
            _objRef = DotNetObjectReference.Create(this);

            // Passe l'instance au JS (fonction globale sous "MYAPP")
            await _js.InvokeVoidAsync("MYAPP.watchStatusWithInstance", _objRef);
        }

        public async Task<bool> IsOnlineAsync()
            => await _js.InvokeAsync<bool>("MYAPP.isOnline");

        public ValueTask DisposeAsync()
        {
            _objRef?.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
