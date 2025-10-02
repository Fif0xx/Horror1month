using UnityEngine;

[CreateAssetMenu(fileName = "InteractionData", menuName = "InteractionSystem/Interaction Data")]
public class InteractionData : ScriptableObject
{
    private InteractableBase _interactable;

    public InteractableBase Interactable
    {
        get { return _interactable; }
        set { _interactable = value; }
    }

    public void Interact()
    {
        _interactable.OnInteract();
        ResetData();
    }

    public bool IsSameInteractable(InteractableBase newInteractable)
    {
        return _interactable == newInteractable;
    }
    public bool IsEmpty() => _interactable == null;
    public void ResetData() => _interactable = null;
}
