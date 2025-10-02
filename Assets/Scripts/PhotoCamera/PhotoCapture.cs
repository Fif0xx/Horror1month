using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCapture : MonoBehaviour
{
    [Header("FlashEffect")]
    [SerializeField]private GameObject cameraFlash;
    [SerializeField]private float flashTime;
    
    [Header("Photo Taker")]
    [SerializeField]private Image photoDisplayArea;
    [SerializeField]private GameObject photoFrame;
    [SerializeField] private float fadeDuration = 0.75f;
    [SerializeField]private CanvasGroup photoDisplayArea_CanvasGroup;
    [SerializeField] private RenderTexture renderTexture;

    [Header("Photo Settings")] 
    [SerializeField] private PhotoStorage photoStorage;
    [SerializeField] private PhotoGalleryUI photoGallery;
    
    [Header("Photo Evaluation")]
    [SerializeField] private PhotoEvaluator photoEvaluator;
    private Renderer targetRendererToEvaluate;
    
    
    //TEMP
    [SerializeField] private Camera photoCam;
    
    private Texture2D screenCapture;
    private bool _isViewingPhoto;
    private float fadeTimer = 0f;
    private bool fading = false;

    private void Start()
    {
        screenCapture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
    }


    private void Update()
    {
        
            if (Input.GetMouseButtonDown(0))
            {
                if (!_isViewingPhoto)
                {
                    StartCoroutine(CapturePhoto());
                }
                else
                {
                    RemovePhoto();
                }

            }

            if (fading)
            {
                fadeTimer += Time.deltaTime;
                float t = fadeTimer / fadeDuration;
                photoDisplayArea_CanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

                if (t >= 1f)
                    fading = false;
            }

        }
        private void StartFadeOut()
        {
            fadeTimer = 0f;
            fading = true;
        }

        private IPhotoTarget FindClosestVisibleTarget()
        {
            IPhotoTarget[] allTargets = FindObjectsOfType<MonoBehaviour>().OfType<IPhotoTarget>().ToArray();
            IPhotoTarget closest = null;
            float closestDistance = float.MaxValue;

            foreach (IPhotoTarget target in allTargets)
            {
                Renderer rend = target.GetRenderer();
                if (rend == null) continue;
                
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(photoCam);
                if (!GeometryUtility.TestPlanesAABB(planes, rend.bounds))
                    continue;
                
                float distance = Vector3.Distance(photoCam.transform.position, rend.bounds.center);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = target;
                }

            }
            return closest;
        }

        IEnumerator CapturePhoto()
        {
            // ВАЖНАЯ ПОМЕТКА: ЗДЕСЬ НУЖНО ПРОИЗВЕСТИ ОПТИМИЗАЦИЮ ФОТОК ИБО ПИКИ В ПРОФАЙЛЕРЕ ПОКАЗЫВАЮТ ЧТО ДАЖЕ КОГДА НЕТУ МЕСТА ВСЕ РАВНО ФОТКА СОЗДАЕТСЯ И УДАЛЯЕТСЯ, ЧТО
            //ПОВЛИЯТЬ НА ПРОИЗВОДИТЕЛЬНОСТЬ.
            //CameraUI = false
            IPhotoTarget bestTarget = FindClosestVisibleTarget();
            if (bestTarget != null && photoEvaluator != null)
            {
                BasicPhotoTarget basicTarget = bestTarget as BasicPhotoTarget;
                if (basicTarget != null)
                {
                    PhotoResult result = photoEvaluator.EvaluatePhoto(basicTarget);
                    Debug.Log($"📷 Оценка фото цели [{basicTarget.TargetName}]: {result.Rating} — {result.Score} очков.");
                }
                else
                {
                    Debug.LogWarning("❌ Найденная цель не является BasicPhotoTarget.");
                }
            }
            
            _isViewingPhoto = true;
            StartCoroutine(CameraFlashEffect());
            
            yield return new WaitForEndOfFrame();

            // 📸 Используем заранее назначенный RenderTexture
            RenderTexture prevRT = RenderTexture.active;
            RenderTexture.active = renderTexture;

            photoCam.Render(); // вручную принудительно отрендерим

            // Чтение пикселей
            screenCapture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            screenCapture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            screenCapture.Apply();

            RenderTexture.active = prevRT;



            // Копия
            Texture2D photoCopy = Instantiate(screenCapture);
            photoCopy.SetPixels(screenCapture.GetPixels());
            photoCopy.Apply();

            if (photoStorage.SavePhoto(photoCopy))
            {
                photoGallery.AddLastPhotoToGallery();
                Debug.Log($"[PhotoCapture] 📸 Фото сохранено в памяти. Текущих фото: {photoStorage.CurrentPhotos}.");
            }
            else
            {
                Debug.LogWarning("[PhotoCapture] ❌ Фото не сохранено: лимит достигнут.");
            }


            ShowPhoto(photoCopy);

        }

        private void ShowPhoto(Texture2D photoTexture)
        {
            Sprite photoSprite = Sprite.Create(photoTexture, new Rect(0, 0, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100f);
            photoDisplayArea.sprite = photoSprite; 
            photoDisplayArea.preserveAspect = true; // Чтобы изображенпе не выглядело расстянутым. Можно в дальнейшим сделать опцию вкл выкл как захочет игрок
            photoFrame.SetActive(true);
            Debug.Log("[PhotoCapture]  Фото показано на экране.");

            StartFadeOut();
        }

        IEnumerator CameraFlashEffect()
        {
            //Audio
            cameraFlash.SetActive(true);
            yield return new WaitForSeconds(flashTime);
            cameraFlash.SetActive(false);
        }

        private void RemovePhoto()
        {
            _isViewingPhoto = false;
            photoFrame.SetActive(false);
            //CameraUI = true
            Debug.Log("Фотография скрыта");
        }
        
    

}
