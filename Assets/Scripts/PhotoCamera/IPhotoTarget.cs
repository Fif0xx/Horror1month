using UnityEngine;

public interface IPhotoTarget
{
    Renderer GetRenderer(); // для GeometryUtillity
    string TargetName { get; } // имя обьекта(дебаг)
}
