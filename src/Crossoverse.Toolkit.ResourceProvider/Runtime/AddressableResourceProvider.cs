#if ADDRESSABLES

using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

namespace Crossoverse.Toolkit.ResourceProvider
{
    public class AddressableResourceProvider : IResourceProvider
    {
        private readonly Dictionary<string, Resource> _loadedResources = new Dictionary<string, Resource>();

        public async UniTask LoadContentCatalogAsync(string catalogPath)
        {
            await Addressables.LoadContentCatalogAsync(catalogPath, true, null);
        }
        
        public async UniTask<Resource> LoadResourceAsync(string path)
        {
            if (ResourceLoaded(path)) { return _loadedResources[path]; }

            var addressable = await Addressables.LoadAssetAsync<UnityEngine.Object>(path);
            var resource = new Resource(path, addressable);

            _loadedResources[path] = resource;

            return resource;
        }
        
        public void UnloadResource(string path)
        {
            if(!ResourceLoaded(path)) { return; }

            var resource = _loadedResources[path];
            _loadedResources.Remove(path);

            DisposeResource(resource);
        }
        
        private bool ResourceLoaded(string path)
        {
            return _loadedResources.ContainsKey(path);
        }
        
        private void DisposeResource(Resource resource)
        {
            Addressables.Release(resource.Object);
        }
    }
}

#endif