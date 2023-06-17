using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Crossoverse.Toolkit.ResourceProvider
{
    public interface IResourceProvider
    {
        UniTask<Resource> LoadResourceAsync(string path);
        void UnloadResource(string path);
    }
}