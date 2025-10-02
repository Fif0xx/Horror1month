using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Input/InputReader")]
public class InputReader : ScriptableObject
{
    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> LookEvent;
    public event Action JumpEvent;
    public event Action InteractEventStarted;
    public event Action InteractEventCanceled;
    public event Action<bool> SprintEvent;
    public event Action<bool> CrouchEvent;
    public event Action PauseEvent;
    public event Action ResumeEvent;
    
    private PlayerInputSystem _inputSystem;

    private void OnEnable()
    {
        InitializeInputSystem();
        Debug.Log("Input Reader Enabled");
        ActivateGameplay();
    }
    private void OnDisable()
    {
        UnsubscribeAll();
        _inputSystem.Player.Disable();
        _inputSystem.UI.Disable();
    }

    private void InitializeInputSystem()
    {
        if (_inputSystem != null)
            return;

        _inputSystem = new PlayerInputSystem();
        // Subscribe to Player actions
        _inputSystem.Player.Move.performed += OnMove;
        _inputSystem.Player.Move.canceled += OnMove;
        _inputSystem.Player.Look.performed += OnLook;
        _inputSystem.Player.Jump.performed += OnJump;    
        _inputSystem.Player.Interact.performed += OnInteractPerfomed;
        _inputSystem.Player.Interact.canceled += OnInteractCanceled;
        _inputSystem.Player.Sprint.performed += OnSprint;
        _inputSystem.Player.Sprint.canceled += OnSprint;
        _inputSystem.Player.Crouch.performed += OnCrouch;
        _inputSystem.Player.Crouch.canceled += OnCrouch;
        _inputSystem.Player.Pause.performed += OnPause;

        // Subscribe to UI actions
        _inputSystem.UI.ResumePause.performed += OnResumePause;
    }
    
    private void UnsubscribeAll()
    {
        if (_inputSystem == null)
            return;
        // Player actions
        _inputSystem.Player.Move.performed -= OnMove;
        _inputSystem.Player.Move.canceled -= OnMove;
        _inputSystem.Player.Look.performed -= OnLook;
        _inputSystem.Player.Jump.performed -= OnJump;
        _inputSystem.Player.Interact.performed -= OnInteractPerfomed;
        _inputSystem.Player.Interact.canceled -= OnInteractCanceled;
        _inputSystem.Player.Sprint.performed -= OnSprint;
        _inputSystem.Player.Sprint.canceled -= OnSprint;
        _inputSystem.Player.Crouch.performed -= OnCrouch;
        _inputSystem.Player.Crouch.canceled -= OnCrouch;
        _inputSystem.Player.Pause.performed -= OnPause;

        // UI actions
        _inputSystem.UI.ResumePause.performed -= OnResumePause;
    }
    public void ActivateGameplay()
    {
        _inputSystem.UI.Disable();
        _inputSystem.Player.Enable();
        
        Debug.Log("Gameplay enabled");
    }
    public void ActivateUI()
    {
        _inputSystem.Player.Disable();
        _inputSystem.UI.Enable();
        Debug.Log("UI enabled");
    }
    private void OnJump(InputAction.CallbackContext context) => JumpEvent?.Invoke();
    private void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
            SprintEvent?.Invoke(true);
        else if (context.canceled)
            SprintEvent?.Invoke(false);
    }
    private void OnInteractPerfomed(InputAction.CallbackContext context) => InteractEventStarted?.Invoke();
    private void OnInteractCanceled(InputAction.CallbackContext context) => InteractEventCanceled?.Invoke();
    private void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
            CrouchEvent?.Invoke(true);
        else if (context.canceled)
            CrouchEvent?.Invoke(false);
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        LookEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }
    
    private void OnPause(InputAction.CallbackContext context)
    {
        PauseEvent?.Invoke();
        ActivateUI();
    }

    private void OnResumePause(InputAction.CallbackContext context)
    {
        ResumeEvent?.Invoke();
        ActivateGameplay();
    }

}
