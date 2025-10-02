using UnityEngine;

public class MimicChildTransform : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 localPosition = new Vector3(0.65f, -0.2f, 2.2f);
    private Vector3 localEulerAngles = Vector3.zero;

    private void Awake()
    {
        if (cameraTransform == null) Debug.LogError("No camera transform assigned");
        else
            // Форсируем позицию и поворот сразу при старте
            UpdateTransform();
    }

    private void LateUpdate()
    {
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        // Позиция: локальное смещение в мировые координаты
        transform.position = cameraTransform.position +
                             cameraTransform.rotation * localPosition;

        // Поворот: локальный поворот относительно камеры
        transform.rotation = cameraTransform.rotation * Quaternion.Euler(localEulerAngles);
    }
}