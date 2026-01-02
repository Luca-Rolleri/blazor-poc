connectionStatus = {
    isOnline: () => navigator.onLine,

    registerOnlineOfflineEvents: (dotNetObjRef) => {
        window.addEventListener('online', () => dotNetObjRef.invokeMethodAsync('OnOnline'));
        window.addEventListener('offline', () => dotNetObjRef.invokeMethodAsync('OnOffline'));
    }
}