using UnityEngine;

namespace Crossoverse.Toolkit.DynamicResourceManager.Samples
{
    public class ThirdPersonCharacterController : MonoBehaviour
    {
        [SerializeField] CharacterController playerCharacterController;
        [SerializeField] Camera playerCamera;
        [SerializeField] float moveSpeed = 3.0f;
        [SerializeField] float rotationSmoothTime = 0.12f;

        private float _targetRotation;
        private float _rotationVelocity;
        
        void Update()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            
            var move = new Vector3(horizontal, 0, vertical).normalized;
            if (move != Vector3.zero)
            {
                var playerTransform = playerCharacterController.transform;
                
                _targetRotation = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(playerTransform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);
                playerTransform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
                playerCharacterController.Move(targetDirection * (moveSpeed * Time.deltaTime));
            }
        }
    }
}