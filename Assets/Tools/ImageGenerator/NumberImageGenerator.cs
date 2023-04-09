using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Crossoverse.Toolkit.Utility.ImageGenerator
{
    public class NumberImageGenerator : MonoBehaviour
    {
        [SerializeField] private Text _statusText;
        [SerializeField] private Text _textureText;
        [SerializeField] private RenderTexture _renderTexture;
        
        private readonly WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
        
        private string _saveDirectoryPath;
        private int _saveImageCount;
        private bool _finished;
        private bool _saving;
        private bool _captured;
        private byte[] _pngImageBytes;
        private Texture2D _texture;
        
        void Start()
        {
            var desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
            _saveDirectoryPath = $"{desktopPath}/ImageGenerator_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            Directory.CreateDirectory(_saveDirectoryPath);
            
            _texture = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.ARGB32, false);
            
            var numberText = _saveImageCount.ToString("d2");
            _textureText.text = numberText;
        }
        
        async void Update()
        {
            if (_saveImageCount > 99) _finished = true;
            
            if (!_finished && !_saving)
            {
                var numberText = _saveImageCount.ToString("d2");
                _textureText.text = numberText;
                
                _statusText.text = $"Status: Saving image {_saveImageCount}.";
                _saving = true;
                
                StartCoroutine(CaptureCoroutine());

                await UniTask.WaitUntil(() => _captured);
                await SavePngImage();
                
                _saveImageCount++;
                _saving = false;
                _statusText.text = "Status: Saved.";
            }
            
            if (_finished) _statusText.text = "Status: Finished.";
        }
        
        private IEnumerator CaptureCoroutine()
        {
            _captured = false;
            yield return _waitForEndOfFrame;
            
            RenderTexture.active = _renderTexture;
            _texture.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
            _texture.Apply();
            
            _pngImageBytes = _texture.EncodeToPNG();

            _captured = true;
        }
        
        private async Task SavePngImage()
        {
            var savePath = $"{_saveDirectoryPath}/Image_{_saveImageCount}.png";
            using (var sourceStream = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 8192, true))
            {
                await sourceStream.WriteAsync(_pngImageBytes, 0, _pngImageBytes.Length);
            }
        }
    }
}
