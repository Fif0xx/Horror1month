using System;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    
    [Header("Allowing movement")]
    [SerializeField] private bool _canMove = true;
    [SerializeField] private bool _canJump = true;
    [SerializeField] private bool _canSprint = true;
    [SerializeField] private bool _canCrouch = true;
    [SerializeField] private bool _canSlideOnSlopes = true;
    [SerializeField] private bool _canInteract = true;
    
    [Header("Movement settings")]
    [SerializeField] private float  moveSpeed;
    [SerializeField] private float slopeSpeed = 8f;
    [SerializeField] private float jumpHeight; 
    
    [Header("Sprint settings")]
    [SerializeField] private float sprintSpeed = 2f;
    [SerializeField] private bool _isSprinting = false;

    
    
    private CharacterController _charController;
    private Vector3 _velocity;
    private Vector2 _moveDir;
    private float _gravity = -19f;
    private bool _isJumping;
    private bool _isGrounded;
    
    [Header("Crouch settings")]
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCentre = new Vector3(0, 0.5f, 0);
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private Vector3 standingCentre = new Vector3(0, 0, 0);
    [SerializeField] private bool duringCrouchAnimation;
    private bool _isCrouching;
    private Camera _playerCam;
    private float _currentSpeed;
    
    [Header("Headbob Settings")]
    [SerializeField] private float idleBobSpeed = 1f;
    [SerializeField] private float idleBobAmountY = 0.01f;
    [SerializeField] private float idleBobAmountX = 0.01f;

    [SerializeField] private float walkBobSpeed = 10f;
    [SerializeField] private float walkBobAmountY = 0.04f;
    [SerializeField] private float walkBobAmountX = 0.06f;

    [SerializeField] private float sprintBobSpeed = 14f;
    [SerializeField] private float sprintBobAmountY = 0.08f;
    [SerializeField] private float sprintBobAmountX = 0.1f;

    [SerializeField] private float crouchBobSpeed = 6f;
    [SerializeField] private float crouchBobAmountY = 0.02f;
    [SerializeField] private float crouchBobAmountX = 0.03f;

    [SerializeField] private float bobSmoothness = 8f;

    private float bobTimer = 0f;
    private Vector3 defaultCamLocalPos;
    private Vector3 hitPointNormal;

    private void Awake()
    {
        _charController = GetComponent<CharacterController>();
        _playerCam = GetComponentInChildren<Camera>();
        _currentSpeed = moveSpeed;
        
        defaultCamLocalPos = _playerCam.transform.localPosition;

        
    }
    
    private void HandleHeadBob()
    {
        if (!_charController.isGrounded)
            return;

        // Используем _moveDir напрямую — он уже Vector2 (X: strafe, Y: forward)
        bool isMoving = _moveDir.sqrMagnitude > 0.01f;

        float bobSpeed, bobAmountY, bobAmountX;

        if (!isMoving)
        {
            bobSpeed = idleBobSpeed;
            bobAmountY = idleBobAmountY;
            bobAmountX = idleBobAmountX;
        }
        else if (_isCrouching)
        {
            bobSpeed = crouchBobSpeed;
            bobAmountY = crouchBobAmountY;
            bobAmountX = crouchBobAmountX;
        }
        else if (_isSprinting)
        {
            bobSpeed = sprintBobSpeed;
            bobAmountY = sprintBobAmountY;
            bobAmountX = sprintBobAmountX;
        }
        else
        {
            bobSpeed = walkBobSpeed;
            bobAmountY = walkBobAmountY;
            bobAmountX = walkBobAmountX;
        }

        bobTimer += Time.deltaTime * bobSpeed;

        float bobOffsetY = Mathf.Sin(bobTimer) * bobAmountY;
        float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * bobAmountX;

        Vector3 targetPosition = new Vector3(
            defaultCamLocalPos.x + bobOffsetX,
            defaultCamLocalPos.y + bobOffsetY,
            defaultCamLocalPos.z
        );

        _playerCam.transform.localPosition = Vector3.Lerp(
            _playerCam.transform.localPosition,
            targetPosition,
            Time.deltaTime * bobSmoothness
        );
    }

    private void HandleInteractionCheck()
    {
        
    }

    private void HandleInteractionInput()
    {
        
    }


    
    private bool Sliding()
    {
        if (_charController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
        {
            hitPointNormal = slopeHit.normal;
            return Vector3.Angle(hitPointNormal, Vector3.up) > _charController.slopeLimit;
        }
        else
        {
            return false;
        }
    }
    
    private void Update()
    {
        ApplyGravity();
        Jump();
        Move();
        HandleHeadBob();
        //Debug.Log(_currentSpeed);
    }

    private void HandleMove(Vector2 dir)
    {
        _moveDir = dir;
    }

    private void HandleJump()
    {
        if( !_canJump || _isCrouching)
            return;
        
        _isJumping = true;
    }
    private void HandleCrouch(bool isCrouching)
    {
        if (!_canCrouch || duringCrouchAnimation || !_canMove)
            return;
        
        if (isCrouching == _isCrouching) 
                return;
        
        StartCoroutine(Crouching());    
        
    }

    private IEnumerator Crouching()
    {
        if (_isCrouching && Physics.Raycast(_playerCam.transform.position, Vector3.up, 1f))
            yield break;
        
            
        duringCrouchAnimation = true;
        float timeElapsed = 0f;
        float targetHeight = _isCrouching ? standingHeight : crouchHeight;
        float currentHeight = _charController.height;
        Vector3 targetCenter = _isCrouching ? standingCentre : crouchingCentre;
        Vector3 currentCenter = _charController.center;
        
        while (timeElapsed < timeToCrouch)
        {
            _charController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            _charController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _charController.height = targetHeight;
        _charController.center = targetCenter;
        _isCrouching = !_isCrouching;
        duringCrouchAnimation = false;
        UpdateSpeed();


    }

    private void UpdateSpeed()
    {
        _currentSpeed = _isCrouching ? crouchSpeed : _isSprinting ? sprintSpeed : moveSpeed;
    }

    private void HandleSprint(bool isSprinting)
    {
        if (!_canSprint || _isCrouching)
            return;

        _isSprinting = isSprinting;
        UpdateSpeed();
    }
    
    private void Move()
    {
        if (!_canMove) return;

        Vector3 localMove = new Vector3(_moveDir.x, 0f, _moveDir.y);
        Vector3 worldMove = transform.TransformDirection(localMove) * _currentSpeed;

        if (_canSlideOnSlopes && Sliding())
        {
            // Скользящий вектор вниз по склону:
            Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, hitPointNormal).normalized;

            // Умножаем скольжение на фиксированную скорость скольжения, например slopeSpeed
            Vector3 slideVelocity = slideDirection * slopeSpeed;

            // Добавляем в итоговое движение
            worldMove += slideVelocity;
        }

        // Сохраняем вертикальную скорость (например гравитация и прыжок)
        worldMove.y = _velocity.y;

        _charController.Move(worldMove * Time.deltaTime);
    }
    

    private void Jump()
    {
        if (_isGrounded && _isJumping)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * _gravity ); 
            _isJumping = false;
        }
    }

    private void ApplyGravity()
    {
        _isGrounded = _charController.isGrounded;
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        else
        {
            _velocity.y += _gravity * Time.deltaTime;
        }
    }
    
    

    #region Enable/Disable methods

    private void OnEnable()
    {
        inputReader.MoveEvent += HandleMove;
        inputReader.SprintEvent += HandleSprint;
        inputReader.CrouchEvent += HandleCrouch;
        inputReader.JumpEvent += HandleJump;
    }
    private void OnDisable()
    {
        inputReader.MoveEvent -= HandleMove;
        inputReader.SprintEvent -= HandleSprint;
        inputReader.CrouchEvent -= HandleCrouch;
        inputReader.JumpEvent -= HandleJump;
    }    

    #endregion
    
}
