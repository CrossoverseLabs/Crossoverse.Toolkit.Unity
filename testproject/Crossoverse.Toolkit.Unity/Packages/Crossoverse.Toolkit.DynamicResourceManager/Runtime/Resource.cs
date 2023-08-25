using System;
using Cysharp.Threading.Tasks;

namespace Crossoverse.Toolkit.DynamicResourceManager
{
    public class Resource
    {
        public readonly string Key;
        public readonly UnityEngine.Object Object;
        
        public Resource(string key, UnityEngine.Object obj)
        {
            Key = key;
            Object = obj;
        }
        
        public override string ToString () => $"Resource<{Object.GetType().Name}>@{Key}";
        
        public TResource As<TResource>() where TResource : UnityEngine.Object
        {
            var castedObject = Object as TResource;

            if (castedObject is null)
            {
                throw new Exception($"Resource `{Key}` is not of type `{typeof(TResource).FullName}`.");
            }

            return castedObject;
        }
    }
    
    public static class ResourceExtension
    {
        public static async UniTask<TResource> As<TResource>(this UniTask<Resource> task) where TResource : UnityEngine.Object
        {
            var resource = await task;
            return resource.As<TResource>();
        }
    }
}