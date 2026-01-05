//export function isOnline() { return navigator.onLine; }

//export function watchStatus(watchStatus) {
//    const call = (b) => DotNet.invokeMethodAsync('MyApp.Client', dotnetCallback, b);
//    window.addEventListener('online', () => call(true));
//    window.addEventListener('offline', () => call(false));
//    call(navigator.onLine); // init
//}

//MYAPP = {
//    isOnline: function () {
//        return navigator.onLine;
//    },

//    watchStatusWithInstance: function (dotNetObjRef) {
//        const call = (b) => dotNetObjRef.invokeMethodAsync('Notify', b);
//        window.addEventListener('online', () => call(true));
//        window.addEventListener('offline', () => call(false));
//        // init immédiat
//        call(navigator.onLine);
//    }
//}
