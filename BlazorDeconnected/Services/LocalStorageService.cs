using Microsoft.JSInterop;

namespace BlazorDeconnected.Services
{
    public class LocalStorageService(IJSRuntime _js) : ILocalStorageService
    {
        public async Task<string?> GetItemAsync(string key)
        {
            return await _js.InvokeAsync<string?>("localStorage.getItem", key);
        }

        public async Task<bool> ContainKeyAsync(string key)
        {
            var keyExists = await _js.InvokeAsync<string?>("localStorage.getItem", key);
            return !string.IsNullOrEmpty(keyExists);
        }

        public async Task SetItemAsync(string key, string value)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", key, value);
        }

        public async Task RemoveItemAsync(string key)
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }
}
