using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Crossoverse.Toolkit.DynamicResourceManager
{
    public class WebResourceProvider : IDisposable
    {
        private readonly string _cacheDirectory;
        private readonly IResourceRepository _repository;
        private readonly ImageProcessing.IDecoder _imageDecoder;
        private readonly HttpClient _httpClient;
        
        public WebResourceProvider(IResourceRepository repository, ImageProcessing.IDecoder imageDecoder)
        {
            _cacheDirectory = Path.Combine(Application.temporaryCachePath, "ResourceProvider");
            _repository = repository;
            _imageDecoder = imageDecoder;
            _httpClient = new HttpClient();
            Debug.Log($"[{nameof(WebResourceProvider)}] Cache directory: {_cacheDirectory}");
        }
        
        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public void ClearAllCachedData()
        {
            if (Directory.Exists(_cacheDirectory))
            {
                var filePaths = Directory.GetFiles(_cacheDirectory);
                foreach (var filePath in filePaths)
                {
                    File.Delete(filePath);
                }
            }
        }

        public void ClearCachedData(string baseUrl, string filename)
        {
            var url = $"{baseUrl}/{filename}";
            var key = ComputeHash(url).Replace("-", "");
            ClearCachedData(key);
        }

        public void ClearCachedData(string key)
        {
            var cachedFilePath = Path.Combine(_cacheDirectory, key);
            if (File.Exists(cachedFilePath))
            {
                File.Delete(cachedFilePath);
            }
        }
        
        public async UniTask<Resource> LoadImageAsync(string baseUrl, string filename, bool useCache = true)
        {
            var url = $"{baseUrl}/{filename}";
            var key = ComputeHash(url).Replace("-", "");
            
            // Loaded resource
            if (_repository.TryGet(key, out var loadedResource))
            {
                var loadedResourceType = loadedResource.Object.GetType();
                if (loadedResourceType == typeof(Texture2D))
                {
                    Debug.Log($"[{nameof(WebResourceProvider)}] In-memory repository hit for {url}");
                    return loadedResource;
                }
                Debug.LogError($"[{nameof(WebResourceProvider)}] In-memory repository error. ResourceType: {loadedResourceType}, Url: {url}");
                return null;
            }
            
            Resource resource;
            Texture2D texture;

            await UniTask.SwitchToThreadPool();

            // Cached resource
            var cacheFilePath = Path.Combine(_cacheDirectory, key);
            if (File.Exists(cacheFilePath))
            {
                if (useCache)
                {
                    Debug.Log($"[{nameof(WebResourceProvider)}] Cache hit for {url}");
                    
                    var cacheFileBytes = await File.ReadAllBytesAsync(cacheFilePath);
                    var image1 = _imageDecoder.Decode(cacheFileBytes);

                    await UniTask.SwitchToMainThread();

                    var textureFormat1 = image1.Format switch
                    {
                        ImageProcessing.ColorFormat.RGBA32 => TextureFormat.RGBA32,
                        ImageProcessing.ColorFormat.RGB24 => TextureFormat.RGB24,
                        _ => TextureFormat.RGBA32,
                    };
                    
                    texture = new Texture2D(image1.Width, image1.Height, textureFormat1, false);
                    texture.LoadRawTextureData(image1.Data);
                    texture.Apply();

                    resource = new Resource(key, texture);
                    _repository.Add(key, resource);                    
                    return resource;
                }
                else
                {
                    File.Delete(cacheFilePath);
                }
            }

            // await UniTask.SwitchToThreadPool();
            
            // Download resource
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _httpClient.SendAsync(request);
            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            var image = _imageDecoder.Decode(imageBytes);

            if (useCache)
            {
                if (!Directory.Exists(_cacheDirectory))
                {
                    System.IO.Directory.CreateDirectory(_cacheDirectory);
                }
                await File.WriteAllBytesAsync(cacheFilePath, imageBytes);
            }

            await UniTask.SwitchToMainThread();

            var textureFormat = image.Format switch
            {
                ImageProcessing.ColorFormat.RGBA32 => TextureFormat.RGBA32,
                ImageProcessing.ColorFormat.RGB24 => TextureFormat.RGB24,
                _ => TextureFormat.RGBA32,
            };
            
            texture = new Texture2D(image.Width, image.Height, textureFormat, false);
            texture.LoadRawTextureData(image.Data);
            texture.Apply();
            
            resource = new Resource(key, texture);
            _repository.Add(key, resource);
            return resource;
        }
        
        public void UnloadResource(string key)
        {
            if (_repository.TryRemove(key, out var loadedResource))
            {
                GameObject.Destroy(loadedResource.Object);
            }
        }
        
        private string ComputeHash(string value)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value));
            return BitConverter.ToString(hash);
        }
    }
}