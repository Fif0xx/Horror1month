using System;
using UnityEngine;

public class BasicPhotoTarget : MonoBehaviour, IPhotoTarget
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private string targetName;
    [SerializeField] private PhotoTargetType targetType;

    public Renderer GetRenderer() => targetRenderer;
    public PhotoTargetType TargetType => targetType;
    public string TargetName => targetName;

    private void Awake()
    {
        
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();
        
    }
}
