using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Crossoverse.Toolkit.DynamicResourceManager
{
    public interface IResourceRepository
    {
        void Add(string key, Resource resource);
        bool TryGet(string key, [NotNullWhen(true)] out Resource resource);
        bool TryRemove(string key, [NotNullWhen(true)] out Resource resource);
    }
    
    public class ResourceRepository : IResourceRepository
    {
        private readonly ConcurrentDictionary<string, Resource> _loadedResources = new ConcurrentDictionary<string, Resource>();
        
        public void Add(string key, Resource resource)
        {
            _loadedResources.TryAdd(key, resource);
        }
        
        public bool TryGet(string key, out Resource resource)
        {
            return _loadedResources.TryGetValue(key, out resource);
        }
        
        public bool TryRemove(string key, out Resource resource)
        {
            return _loadedResources.TryRemove(key, out resource);
        }
    }
}