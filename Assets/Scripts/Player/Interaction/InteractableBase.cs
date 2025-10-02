using System;
using UnityEngine;

public class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Interactable Settings")] 
    [SerializeField] private float holdDuraion;

    [SerializeField] private bool holdInteract;
    [SerializeField] private bool multipleUse;
    [SerializeField] private bool isInteractable;
    [SerializeField] private string tooltipMessage = "interact";
    
    
    
    
    public float HoldDuration => holdDuraion;
    public bool HoldInteract => holdInteract;
    public bool MultipleUse
    {
        get => multipleUse;
        set => multipleUse = value;
    }

    public bool IsInteractable
    {
        get => isInteractable;
        set => isInteractable = value;
    }

    public string ToolTipMessage => tooltipMessage;

    public void OnInteract()
    {
        Debug.Log("Interacted with: " + gameObject.name);
    }
}
