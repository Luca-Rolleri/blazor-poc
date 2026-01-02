// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [ /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/ ];
const offlineAssetsExclude = [ /^service-worker\.js$/ ];

// Replace with your base path if you are hosting on a subfolder. Ensure there is a trailing '/'.
const base = "/";
const baseUrl = new URL(base, self.origin);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

async function onInstall(event) {
    console.info('Service worker: Install');

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    let cachedResponse = null;
    if (event.request.method === 'GET') {
        // For all navigation requests, try to serve index.html from cache,
        // unless that request is for an offline resource.
        // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
        const shouldServeIndexHtml = event.request.mode === 'navigate'
            && !manifestUrlList.some(url => url === event.request.url);

        const request = shouldServeIndexHtml ? 'index.html' : event.request;
        const cache = await caches.open(cacheName);
        cachedResponse = await cache.match(request);
    }

    return cachedResponse || fetch(event.request);
}


// IndexedDB utils (SW-side)
function openDb() {
    return new Promise((resolve, reject) => {
        const req = indexedDB.open('MyOfflineDb', 1);
        req.onupgradeneeded = (ev) => {
            const db = ev.target.result;
            if (!db.objectStoreNames.contains('Outbox')) {
                db.createObjectStore('Outbox', { keyPath: 'Id', autoIncrement: true });
            }
            if (!db.objectStoreNames.contains('Drafts')) {
                db.createObjectStore('Drafts', { keyPath: 'Id', autoIncrement: true });
            }
        };
        req.onsuccess = () => resolve(req.result);
        req.onerror = () => reject(req.error);
    });
}

async function getOutboxItems() {
    const db = await openDb();
    return new Promise((resolve, reject) => {
        const tx = db.transaction('Outbox', 'readonly');
        const store = tx.objectStore('Outbox');
        const req = store.getAll();
        req.onsuccess = () => resolve(req.result || []);
        req.onerror = () => reject(req.error);
    });
}

async function updateOutboxItem(item) {
    const db = await openDb();
    return new Promise((resolve, reject) => {
        const tx = db.transaction('Outbox', 'readwrite');
        const store = tx.objectStore('Outbox');
        const req = store.put(item);
        req.onsuccess = () => resolve();
        req.onerror = () => reject(req.error);
    });
}

async function removeOutboxItem(id) {
    const db = await openDb();
    return new Promise((resolve, reject) => {
        const tx = db.transaction('Outbox', 'readwrite');
        const store = tx.objectStore('Outbox');
        const req = store.delete(id);
        req.onsuccess = () => resolve();
        req.onerror = () => reject(req.error);
    });
}

self.addEventListener('sync', event => {
    if (event.tag === 'sync-outbox') {
        event.waitUntil(flushOutbox());
    }
});

async function flushOutbox() {
    const items = (await getOutboxItems()).sort((a, b) => new Date(a.QueuedAt) - new Date(b.QueuedAt));
    for (const it of items) {
        try {
            const res = await fetch(it.Endpoint, {
                method: it.Method,
                headers: {
                    'Content-Type': 'application/json',
                    'X-Idempotency-Key': it.ClientKey // clé de déduplication côté API
                },
                body: it.JsonBody
            });
            if (res.ok) {
                await removeOutboxItem(it.Id);
            } else {
                it.Attempts = (it.Attempts || 0) + 1;
                await updateOutboxItem(it);
            }
        } catch {
            it.Attempts = (it.Attempts || 0) + 1;
            await updateOutboxItem(it);
        }
    }
}

