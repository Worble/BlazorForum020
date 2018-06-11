self.addEventListener('fetch', function (event) {
    event.respondWith(caches.match(event.request).then(function (response) {
        // caches.match() always resolves
        // but in case of success response will have value
        if (response !== undefined) {
            console.log("retrieving " + event.request + " from cache")
            return response;
        } else {
            console.log(event.request + " not in cache, downloading...")
            return fetch(event.request).then(function (response) {
                console.log(event.request + " downloaded, placing in cache")
                // response may be used only once
                // we need to save clone to put one copy in cache
                // and serve second one
                let responseClone = response.clone();

                caches.open('v1').then(function (cache) {
                    console.log(event.request + " placed in cache")
                    cache.put(event.request, responseClone);
                });
                return response;
            }).catch(function () {
                return undefined;
            });
        }
    }));
});