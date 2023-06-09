using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Crossoverse.Toolkit.ImageProcessing;

namespace Crossoverse.Toolkit.DynamicResourceManager.Samples
{
    [Serializable]
    public class ImageObject
    {
        public ImageObjectView View;
        public string BaseUrl;
        public string Filename;
        public string ResourceKey;
        internal bool IsRequested;
    }
    
    public class ImageLoaderSample : MonoBehaviour
    {
        [SerializeField] private ImageObjectView _imageObjectPrefab;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private float _viewDistance = 5f;
        [SerializeField] private float _horizontalViewAngleDegree = 120f;
        
        [Header("Image Files")]
        [SerializeField] private string _baseUrl;
        [SerializeField] private List<string> _imageFileNames = new List<string>();
        
        private readonly List<ImageObject> _imageObjects = new List<ImageObject>();
        private readonly int _unloadPriority = -10;
        
        private AsyncTaskDispatcher<ImageObject> _asyncTaskDispatcher;
        private AsyncTaskDispatcher<ImageObject> _unloadTaskDispatcher;
        private HttpClient _httpClient;
        private WebResourceProvider _resourceProvider;
        
        void OnDestroy()
        {
            _unloadTaskDispatcher?.Dispose();
            _asyncTaskDispatcher?.Dispose();
            _httpClient?.Dispose();
            _resourceProvider?.Dispose();
        }
        
        void Awake()
        {
            Debug.Log($"[ImageLoaderSample] Awake");

            _httpClient = new HttpClient();
            _resourceProvider = new WebResourceProvider(new ResourceRepository(), new StbImageDecoder());
            
            // _resourceProvider.ClearAllCachedData();

            _asyncTaskDispatcher = new AsyncTaskDispatcher<ImageObject>();
            _asyncTaskDispatcher.AsyncTaskEvent += async (sender, eventArgs, cancellationToken) =>
            {
                // Debug.Log($"[AsyncTaskEvent] Beginning of AsyncTaskEvent");
                // Debug.Log($"[AsyncTaskEvent] ThreadId: {System.Environment.CurrentManagedThreadId}");
                
                var priority = eventArgs.Priority;
                var data = eventArgs.Data;
                
                // Debug.Log($"[AsyncTaskEvent] ThreadId: {System.Environment.CurrentManagedThreadId}");
                // Debug.Log($"[AsyncTaskEvent] Priority: {priority}, Image: {data.Filename}");
                
                var url = $"{data.BaseUrl}/{data.Filename}";
                
                await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken: cancellationToken);

                try
                {
                    var resource = await _resourceProvider.LoadImageAsync(data.BaseUrl, data.Filename);

                    // Debug.Log($"[AsyncTaskEvent] ThreadId: {System.Environment.CurrentManagedThreadId}");
                    
                    await UniTask.SwitchToMainThread();

                    if (resource is not null)
                    {
                        data.ResourceKey = resource.Key;
                        data.View.SetTexture2D(resource.As<Texture2D>());
                    }
                }
                catch (Exception e)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken: cancellationToken);
                    Debug.LogError($"[AsyncTaskEvent] {e.GetType()}: {e.Message}");
                    return false;
                }

                // Debug.Log($"[AsyncTaskEvent] End of AsyncTaskEvent");
                return true;
            };

            _unloadTaskDispatcher = new AsyncTaskDispatcher<ImageObject>();
            _unloadTaskDispatcher.AsyncTaskEvent += async (sender, eventArgs, cancellationToken) =>
            {
                var priority = eventArgs.Priority;
                var data = eventArgs.Data;

                await UniTask.SwitchToMainThread();

                data.View.SetDefault();
                _resourceProvider.UnloadResource(data.ResourceKey);

                data.IsRequested = false;

                return true;
            };
            
            // Instantiate image objects
            var count = 0;
            for (int z = 0; z < 5; z++)
            {
                for (int x = 0; x < 5; x++)
                {
                    var px = x * 5 - 10;
                    var pz = z * 5 - 10;
                    var instance = GameObject.Instantiate(_imageObjectPrefab.gameObject, new Vector3(px, 0, pz), Quaternion.identity, this.transform);
                    
                    var view = instance.GetComponent<ImageObjectView>();
                    var baseUrl = _baseUrl;
                    var filename = _imageFileNames[count++ % _imageFileNames.Count];
                    
                    _imageObjects.Add(new ImageObject()
                    {
                        View = view,
                        BaseUrl = baseUrl,
                        Filename = filename,
                    });
                }
            }
        }
        
        void Update()
        {
            foreach (var imageObject in _imageObjects)
        {
                var imageObjectPosition = imageObject.View.transform.position;
                var playerPosition = _playerTransform.position;
                var playerForwardDirection = _playerTransform.forward;
                
                var priority = CalculatePriority(imageObjectPosition, playerPosition, playerForwardDirection);

                if (!imageObject.IsRequested)
                {
                    imageObject.IsRequested = true;
                    
                    if (priority >= 0)
                    {
                        imageObject.View.SetRequested();
                        _asyncTaskDispatcher.Enqueue(priority, imageObject);
                    }
                    else
                    {
                        imageObject.IsRequested = false;
                    }
                }
                else
                {
                    if (priority == _unloadPriority)
                    {
                        _unloadTaskDispatcher.Enqueue(priority, imageObject);
                        imageObject.IsRequested = false;
                    }
                }
            }
        }
        
        private int CalculatePriority(Vector3 imageObjectPosition, Vector3 playerPosition, Vector3 playerForwardDirection)
        {
            var imageObjectPositionXZ = new Vector2(imageObjectPosition.x, imageObjectPosition.z);
            var playerPositionXZ = new Vector2(playerPosition.x, playerPosition.z);
            var playerForwardXZ = new Vector2(playerForwardDirection.x, playerForwardDirection.z);
            
            var directionXZ = imageObjectPositionXZ - playerPositionXZ;
            
            directionXZ.Normalize();
            playerForwardXZ.Normalize();
            
            var distance = Vector2.Distance(imageObjectPositionXZ, playerPositionXZ);
            var dot = Vector3.Dot(directionXZ, playerForwardXZ);
            var cosine = Mathf.Cos(_horizontalViewAngleDegree / 2 * Mathf.Deg2Rad);
            
            if (distance < _viewDistance && dot > cosine)
            {
                var score = Mathf.RoundToInt(distance * 1000 + dot * 100);
                return score;
            }
            else if (distance > _viewDistance * 2)
            {
                return _unloadPriority;
            }
            
            return -1;
        }
    }
}