using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Crossoverse.Toolkit.ResourceProvider
{
    public interface IResourceProvider
    {
        UniTask<long> GetDownloadSizeAsync(string path);
        UniTask<Resource> LoadResourceAsync(string path, IProgress<float> progress = null);
        void UnloadResource(string path);
    }
}
