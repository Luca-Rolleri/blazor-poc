using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace BlazorDeconnected.Services
{
    public class SaveCounterCommand: ISaveCounterCommand, IDisposable
    {


        private const string CurrentKey = "counter";
        private const string PendingSingleKey = "pending:counter:last";

        private readonly HttpClient _http;
        private readonly ILocalStorageService _storage;
        private readonly IConnectivityService _connectivity;

        public SaveCounterCommand(HttpClient http,
                                  ILocalStorageService storage,
                                  IConnectivityService connectivity)
        {
            _http = http;
            _storage = storage;
            _connectivity = connectivity;

            // S’abonner pour rejouer automatiquement quand on redevient online
            _connectivity.StatusChanged += OnConnectivityChangedAsync;
        }
        public async Task ExecuteAsync(int counterValue, CancellationToken ct = default)
        {
            // Toujours maintenir la valeur courante pour l’UI
            await _storage.SetItemAsync(CurrentKey, counterValue.ToString());

            if (_connectivity.IsOnline)
            {
                // Online → save API
                await _http.PostAsJsonAsync<int>("api/counter", counterValue, ct);
                await _storage.RemoveItemAsync(PendingSingleKey);
            }
            else
            {
                await _storage.SetItemAsync(PendingSingleKey, counterValue.ToString());
            }
        }

        private async void OnConnectivityChangedAsync(object? sender, bool isOnline)
        {
            if (!isOnline) return;
            if (!await _storage.ContainKeyAsync(PendingSingleKey)) return;

            // Rejouer la file dès qu’on redevient online
            try
            {
                var value = await _storage.GetItemAsync(PendingSingleKey);
                await _http.PostAsJsonAsync<int>("api/counter", int.Parse(value));
                await _storage.RemoveItemAsync(PendingSingleKey);

            }
            catch
            {
                // En cas d’erreur API, on ne purge pas → re-essai sur prochaine reconnexion
            }
        }

        public void Dispose()
        {
            _connectivity.StatusChanged -= OnConnectivityChangedAsync;
        }
    }
}
