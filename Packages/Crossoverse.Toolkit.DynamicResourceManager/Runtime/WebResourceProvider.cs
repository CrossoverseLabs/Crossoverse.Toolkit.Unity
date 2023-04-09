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
        private readonly HttpClient _httpClient;
        
        public WebResourceProvider(IResourceRepository repository)
        {
            _cacheDirectory = Path.Combine(Application.temporaryCachePath, "ResourceProvider");
            _repository = repository;
            _httpClient = new HttpClient();
            Debug.Log($"[{nameof(WebResourceProvider)}] Cache directory: {_cacheDirectory}");
        }
        
        public void Dispose()
        {
            _httpClient.Dispose();
        }
        
        public async UniTask<Resource> LoadImageAsync(string basePath, string filename, bool useCache = true)
        {
            await UniTask.SwitchToThreadPool();
            
            var path = Path.Combine(basePath, filename);
            var key = ComputeHash(path).Replace("-", "");
            
            // Loaded resource
            if (_repository.TryGet(key, out var loadedResource))
            {
                var loadedResourceType = loadedResource.Object.GetType();
                if (loadedResourceType == typeof(Texture2D))
                {
                    Debug.Log($"[{nameof(WebResourceProvider)}] In-memory repository hit for {path}");
                    return loadedResource;
                }
                Debug.LogError($"[{nameof(WebResourceProvider)}] In-memory repository error. ResourceType: {loadedResourceType}, Path: {path}");
                return null;
            }
            
            Resource resource;
            Texture2D texture;
            
            // Cached resource
            var cacheFilePath = Path.Combine(_cacheDirectory, key);
            if (File.Exists(cacheFilePath))
            {
                if (useCache)
                {
                    Debug.Log($"[{nameof(WebResourceProvider)}] Cache hit for {path}");
                    
                    var cacheFileBytes = await File.ReadAllBytesAsync(cacheFilePath);
                    
                    await UniTask.SwitchToMainThread();
                    
                    texture = new Texture2D(2, 2);
                    if (texture.LoadImage(cacheFileBytes))
                    {
                        resource = new Resource(key, texture);
                        _repository.Add(key, resource);
                        return resource;
                    }
                }
                else
                {
                    File.Delete(cacheFilePath);
                }
            }
            
            await UniTask.SwitchToThreadPool();
            
            // Download resource
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            var response = await _httpClient.SendAsync(request);
            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            
            await UniTask.SwitchToMainThread();
            
            texture = new Texture2D(2, 2);
            if (!texture.LoadImage(imageBytes))
            {
                return null;
            }
            
            await UniTask.SwitchToMainThread();
            
            if (useCache)
            {
                if (!Directory.Exists(_cacheDirectory))
                {
                    System.IO.Directory.CreateDirectory(_cacheDirectory);
                }
                await File.WriteAllBytesAsync(cacheFilePath, imageBytes);
            }
            
            resource = new Resource(key, texture);
            _repository.Add(key, resource);
            
            return resource;
        }
        
        public void UnloadResource(string key)
        {
            if (_repository.TryRemove(key, out var loadedResource))
            {
                // Can't unload prefabs: https://forum.unity.com/threads/393385.
                // if (resource.Object is GameObject || resource.Object is Component) return;
                Resources.UnloadAsset(loadedResource.Object);
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