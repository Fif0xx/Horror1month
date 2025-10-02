using System;
using UnityEngine;
using UnityEngine.Serialization;

public class InteractionController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private InputReader playerInputReader;
    private bool inputInteracting;

    [SerializeField] private InteractionData interactionData;
        
    [Space]
    [Header("RaySettings")]
    [SerializeField] private float rayDistance;
    [SerializeField] private float raySphereRadius;
    [SerializeField] private LayerMask interactionLayer;

    [SerializeField]private Camera _cam;
     
    [Space,Header("UI")]
    [SerializeField] private InteractionUIPanel uiPanel;

    private bool currInteracting;
    private float holdTimer;

    private void Awake()
    {
        
    }

    private void Update()
    {
        CheckForInteractable();
        CheckForInteractableInput();
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
        RaycastHit hitInfo;

        bool isHit = Physics.SphereCast(ray, raySphereRadius, out hitInfo, rayDistance, interactionLayer);
        if (isHit)
        {
            InteractableBase interactable = hitInfo.collider.GetComponent<InteractableBase>();
            if (interactable != null)
            {
                if (interactionData.IsEmpty())
                {
                    interactionData.Interactable = interactable;
                    uiPanel.SetToolTip(interactable.ToolTipMessage);
                }
                else
                {
                    if (!interactionData.IsSameInteractable(interactable))
                    {
                        interactionData.Interactable = interactable;
                        uiPanel.SetToolTip(interactable.ToolTipMessage);
                    }

                }
            }
        }
        else
        {
            uiPanel.ResetUI();
            interactionData.ResetData();
        }
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, isHit ? Color.green : Color.red);
    }
    private void CheckForInteractableInput()
    {
        if(interactionData.IsEmpty())
            return;
        

        if (currInteracting)
        {
            if (interactionData.Interactable == null || !interactionData.Interactable.IsInteractable )
                return;
            if (interactionData.Interactable.HoldInteract)
            {
                holdTimer += Time.deltaTime;
                float heldPercent = holdTimer / interactionData.Interactable.HoldDuration;
                uiPanel.UpdateProgressBar(heldPercent);
                
                if (heldPercent > 1)
                {
                    uiPanel.ResetUI();
                    interactionData.Interact();
                    currInteracting = false;
                }
            }
            else
            {
                interactionData.Interact();
                currInteracting = false;
            }
        }
    }
    
    private void InteractionStart()
    {
        inputInteracting = true;
        currInteracting = true;
        holdTimer = 0;
        Debug.Log("начал действовать");
    }
    private void InteractionCanceled()
    {
        inputInteracting = false;
        currInteracting = false;
        holdTimer = 0;
        uiPanel.UpdateProgressBar(0f);
        Debug.Log("закончил действовать");
    }
    
    #region Enable/Disable methods

    private void OnEnable()

    {
        playerInputReader.InteractEventStarted += InteractionStart;
        playerInputReader.InteractEventCanceled += InteractionCanceled;
    }
    

    private void OnDisable()
    {
        playerInputReader.InteractEventStarted -= InteractionStart;
        playerInputReader.InteractEventCanceled -= InteractionCanceled;
    }

    

    #endregion

}
