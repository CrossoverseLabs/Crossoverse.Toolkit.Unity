using UnityEngine;

namespace Crossoverse.Toolkit.DynamicResourceManager.Samples
{
    public class ImageObjectView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Texture2D _defaultTexture;
        [SerializeField] private Texture2D _requestedStatusTexture;
        [SerializeField] private Texture2D _errorStatusTexture;
        
        void Awake()
        {
            _meshRenderer.material.mainTexture = _defaultTexture;
        }
        
        public bool SetTexture2D(Texture2D texture)
        {
            _meshRenderer.material.mainTexture = texture;
            return true;
        }

        public void SetDefault()
        {
            _meshRenderer.material.mainTexture = _defaultTexture;
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