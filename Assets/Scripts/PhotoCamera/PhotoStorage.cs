using UnityEngine;
using System.Collections.Generic;

public class PhotoStorage : MonoBehaviour
{
    [SerializeField] private int maxPhotos = 10;
    private readonly List<Texture2D> _savedPhotos = new();
    
    

    public bool SavePhoto(Texture2D photo)
    {
        if (photo == null || _savedPhotos.Count >= maxPhotos)
            return false; // не сохранилось

        _savedPhotos.Add(photo);
        return true; // успешно
    }
    public bool DeletePhotoAt(int index)
    {
        if (index >= 0 && index < _savedPhotos.Count)
        {
            Destroy(_savedPhotos[index]); // чистим память
            _savedPhotos.RemoveAt(index);
            return true;
        }

        return false;
    }
    
    public Texture2D GetPhoto(int index)
    {
        // можно позже написать вместо всех проверок: return (index >= 0 && index < _savedPhotos.Count) ? _savedPhotos[index] : null;
        if (index < 0)
        {
            Debug.LogWarning($"[PhotoMemory] ❌ Индекс {index} меньше нуля. Возврат null.");
            return null;
        }

        if (index >= _savedPhotos.Count)
        {
            Debug.LogWarning($"[PhotoMemory] ❌ Индекс {index} выходит за пределы. Фото всего: {_savedPhotos.Count}.");
            return null;
        }
        Debug.Log($"[PhotoMemory] ✅ Фото по индексу {index} успешно получено.");
        return _savedPhotos[index];
        
    }
    
    public IReadOnlyList<Texture2D> GetAllPhotos() => _savedPhotos.AsReadOnly();
    public int CurrentPhotos => _savedPhotos.Count;
    public int MaxCount => maxPhotos;
}
