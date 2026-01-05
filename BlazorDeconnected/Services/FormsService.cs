using BlazorDeconnected.Data;
using BlazorDeconnected.Models;
using Microsoft.JSInterop;
using System.Text;
using System.Text.Json;

namespace BlazorDeconnected.Services
{
    public class FormsService
    {
        private readonly OfflineStore _store;
        private readonly NetworkStatusService _net;
        private readonly IJSRuntime _js;
        private readonly HttpClient _http;

        public FormsService(OfflineStore store, NetworkStatusService net, IJSRuntime js, HttpClient http)
        { _store = store; _net = net; _js = js; _http = http; }

        public async Task SubmitPersonAsync(PersonDto dto)
        {
            var body = JsonSerializer.Serialize(dto);
            var online = await _net.IsOnlineAsync();

            if (!online)
            {
                await _store.EnqueueAsync(new OutboxItem
                {
                    ClientKey = Guid.NewGuid().ToString("N"),
                    Endpoint = "/api/person",
                    Method = "POST",
                    JsonBody = body,
                    QueuedAt = DateTime.UtcNow
                });
                // Enregistre un Background Sync
                await _js.InvokeVoidAsync("navigator.serviceWorker.ready.then(r=>r.sync.register('sync-outbox'))");
                return;
            }

            var res = await _http.PostAsync("/api/person",
              new StringContent(body, Encoding.UTF8, "application/json"));

            if (!res.IsSuccessStatusCode)
            {
                await _store.EnqueueAsync(new OutboxItem
                {
                    ClientKey = Guid.NewGuid().ToString("N"),
                    Endpoint = "/api/person",
                    Method = "POST",
                    JsonBody = body,
                    QueuedAt = DateTime.UtcNow
                });
                await _js.InvokeVoidAsync("navigator.serviceWorker.ready.then(r=>r.sync.register('sync-outbox'))");
            }
        }
    }
}
