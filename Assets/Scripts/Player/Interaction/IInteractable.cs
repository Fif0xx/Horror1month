public interface IInteractable
{
    public float HoldDuration { get; } 
    public bool HoldInteract { get; } 
    public bool MultipleUse { get; set; } 
    public bool IsInteractable { get; set; } 
    public string ToolTipMessage { get; }
    void OnInteract();    
}
