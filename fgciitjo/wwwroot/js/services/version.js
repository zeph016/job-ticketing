const CACHE_VERSION = 1.0;
console.log('Cache Version: ' + CACHE_VERSION);

const cacheAssets = [
    '../../index.html',
    '../../css/app.css',
    '../../css/tables/tables.styles.css',
    '../../css/dashboard/dashboard.styles.css',
    '../../css/createticket/ticket.styles.css'
];

self.addEventListener('install', (e) => {
    console.log('Service Worker: Installed');
    e.waitUntil(
        caches
            .open(CACHE_VERSION)
            .then(cache => {
                console.log('Service Worker: caching files');
                cache.addAll(cacheAssets);
            })
            .then(() => self.skipWaiting())
    );
});

self.addEventListener('activate', (e) => {
    console.log('Service Worker: Activated');
    e.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cache => {
                    if(cache !== CACHE_VERSION) {
                        console.log('Service Worker: Clearing Cache');
                        return caches.delete(cache);
                    }
                })
            )
        })
    );
});