using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("InputAsset")] [SerializeField]
    private InputReader inputReader;
    
    
    
    
    
    
    
    
    private void InteractionStart()
    {
        Debug.Log("Interaction Started");
        
    }
    private void InteractionCanceled()
    {
        Debug.Log("Interaction Canceled");
        
    }
    
    
    
    
    
    #region Enable/Disable methods

    private void OnEnable()

    {
        inputReader.InteractEventStarted += InteractionStart;
        inputReader.InteractEventCanceled += InteractionCanceled;
    }

    private void OnDisable()
    {
        inputReader.InteractEventStarted -= InteractionStart;
        inputReader.InteractEventCanceled -= InteractionCanceled;
    }    

    #endregion
}
