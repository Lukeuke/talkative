export const cacheImage = async (imageUrl) => {
  const cache = await caches.open("images-cache");
  const response = await fetch(imageUrl);
  if (response.ok) {
    cache.put(imageUrl, response.clone());
  }
};

export const getCachedImage = async (imageUrl) => {
  const cache = await caches.open("images-cache");
  const cachedResponse = await cache.match(imageUrl);
  if (cachedResponse) {
    return URL.createObjectURL(await cachedResponse.blob());
  }
  return null;
};