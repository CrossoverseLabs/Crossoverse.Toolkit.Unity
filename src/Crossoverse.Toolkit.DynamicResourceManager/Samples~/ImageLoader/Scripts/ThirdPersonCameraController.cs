using UnityEngine;

namespace Crossoverse.Toolkit.DynamicResourceManager.Samples
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float distanceFromPlayer = 5.0f;
        [SerializeField] private float heightOffset = 2.0f;
        [SerializeField] private float rotationSpeed = 3.0f;
        
        private float _currentX;
        private float _currentY;
        
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        void Update()
        {
            _currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            _currentY += Input.GetAxis("Mouse Y") * rotationSpeed;
            _currentY = Mathf.Clamp(_currentY, -90, 90);
        }
        
        void LateUpdate()
        {
            var playerPosition = playerTransform.position;
            var direction = new Vector3(0, heightOffset, -distanceFromPlayer);
            var rotation = Quaternion.Euler(-_currentY, _currentX, 0);
            cameraTransform.position = playerPosition + rotation * direction;
            cameraTransform.LookAt(playerPosition + Vector3.up * heightOffset);
        }
    }
}