using System;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    
    [Header("Look Parameters")]
    [SerializeField, Range(0.1f, 10f)] private float mouseSensitivity = 1f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80f;
    
    private Camera _playerCam;
    private CharacterController _charController;
    private Vector2 _currentLookInput = Vector2.zero;
    private float _xRotation = 0f;
    
    private void Awake()
    {
        _playerCam = GetComponentInChildren<Camera>();
        _charController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void HandleMouseLook(Vector2 lookInput)
    {
        _currentLookInput = lookInput;
        ApplyCamRotation();
    }

    private void ApplyCamRotation()
    {
        float mouseX = _currentLookInput.x * mouseSensitivity;
        float mouseY = _currentLookInput.y * mouseSensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -lowerLookLimit, upperLookLimit);
        

        // Устанавливаем только X-поворот (вверх-вниз), НЕ трогая Z (наклон в стороны от headbob)
        _playerCam.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        // Поворот тела (влево-вправо)
        transform.Rotate(Vector3.up * mouseX);
    }

    private void OnEnable()
    {
        inputReader.LookEvent += HandleMouseLook;
    }
    private void OnDisable()
    {
        inputReader.LookEvent -= HandleMouseLook;
    }
}
