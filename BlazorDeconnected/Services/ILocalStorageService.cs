namespace BlazorDeconnected.Services
{
    public interface ILocalStorageService
    {
        Task SetItemAsync(string key, string value);
        Task<string> GetItemAsync(string key);
        Task<bool> ContainKeyAsync(string key);
        Task RemoveItemAsync(string key);
    }
}
