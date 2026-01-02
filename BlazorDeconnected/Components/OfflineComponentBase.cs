using BlazorDeconnected.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorDeconnected.Components
{
    public abstract class OfflineComponentBase : ComponentBase
    {
        [Inject] 
        protected IJSRuntime JS { get; set; } = default!;

        [Inject] 
        protected IConnectivityService Connectivity { get; set; } = default!;

        protected bool IsOnline => Connectivity.IsOnline;

        protected override async Task OnAfterRenderAsync(bool firstrender)
        {
            if (firstrender)
            {
                await JS.InvokeVoidAsync("connectionStatus.registerOnlineOfflineEvents", DotNetObjectReference.Create(this));
                var initial = await JS.InvokeAsync<bool>("connectionStatus.isOnline");

                Connectivity.NotifyStatusChanged(initial);

            }
        }

        protected virtual Task OnlineStatusChanged(bool isOnline) => Task.CompletedTask;


        [JSInvokable]
        public async Task OnOnline()
        {
            Connectivity.NotifyStatusChanged(true);
            await OnlineStatusChanged(true);
            StateHasChanged();
        }


        [JSInvokable]
        public async Task OnOffline()
        {
            Connectivity.NotifyStatusChanged(false);
            await OnlineStatusChanged(false);
            StateHasChanged();
        }
    }
}
