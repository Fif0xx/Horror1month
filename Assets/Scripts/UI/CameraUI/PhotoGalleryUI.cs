using UnityEngine;
using UnityEngine.UI;
public class PhotoGalleryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform photoContainer;     // Контейнер под фотки (Content)
    [SerializeField] private GameObject visual;     // Контейнер под фотки (Content)
    [SerializeField] private GameObject photoItemPrefab;   // Префаб одного элемента галереи
    
    [Header("Source")]
    [SerializeField] private PhotoStorage photoStorage;
    

    private void Update()
    {
        //Временно
        if (Input.GetKeyDown(KeyCode.G))
        {
            visual.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            visual.SetActive(false);
        }
        
    }

    public void AddLastPhotoToGallery()
    {
        
        if (photoStorage.CurrentPhotos == 0)
        {
            Debug.LogWarning("[PhotoGalleryUI] Нет фотографий для добавления.");
            return;
        }

        Texture2D photo = photoStorage.GetPhoto(photoStorage.CurrentPhotos - 1);
        if (photo == null)
        {
            Debug.LogWarning("[PhotoGalleryUI] Последняя фотография недоступна.");
            return;
        }

        GameObject photoItem = Instantiate(photoItemPrefab, photoContainer);
        Image image = photoItem.GetComponentInChildren<Image>();
        if (image == null)
        {
            Debug.LogError("[PhotoGalleryUI] Префаб не содержит компонент Image.");
            return;
        }

        Sprite sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0.5f, 0.5f), 100f);
        image.sprite = sprite;

        Debug.Log("[PhotoGalleryUI] 🖼️ Фотография добавлена в галерею.");
    }
}
