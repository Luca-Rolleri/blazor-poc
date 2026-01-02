using Blazor.IndexedDB;
using BlazorDeconnected;
using BlazorDeconnected.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<FormsService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7137/") });
builder.Services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<ISaveCounterCommand, SaveCounterCommand>();
builder.Services.AddScoped<IConnectivityService, ConnectivityService>();

builder.Services.AddScoped<OfflineStore>();
builder.Services.AddScoped<NetworkStatusService>();


await builder.Build().RunAsync();
