using UnityEngine;

namespace Crossoverse.Toolkit.DynamicResourceManager.Samples
{
    public class ImageObjectView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Texture2D _defaultTexture;
        [SerializeField] private Texture2D _requestedStatusTexture;
        [SerializeField] private Texture2D _errorStatusTexture;
        
        private Texture2D _texture;
        
        void Awake()
        {
            _texture = new Texture2D(2, 2);
            _meshRenderer.material.mainTexture = _defaultTexture;
        }
        
        public void SetTexture2D(byte[] bytes)
        {
            _texture.LoadImage(bytes);
            _meshRenderer.material.mainTexture = _texture;
        }
        
        public void SetRequested()
        {
            _meshRenderer.material.mainTexture = _requestedStatusTexture;
        }
        
        public void SetError()
        {
            _meshRenderer.material.mainTexture = _errorStatusTexture;
        }
    }
}