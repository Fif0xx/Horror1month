using System.Linq;
using UnityEngine;
public class PhotoEvaluator : MonoBehaviour
{
    [SerializeField] private Camera photoCamera;
    [SerializeField] private PhotoEvaluationSettings evaluationSettings;

    public PhotoResult EvaluatePhoto(BasicPhotoTarget target)
    {
        Renderer rend = target.GetRenderer();
        Vector3 worldCenter = rend.bounds.center;
        float visibleRatio = CalculateVisibleRatio(rend.bounds);
        float distance = Vector3.Distance(photoCamera.transform.position, worldCenter);

        var type = target.TargetType;

        // 1. Дистанция
        PhotoRating distanceRating = evaluationSettings.GetRatingByDistance(distance, type);
        int distanceScore = evaluationSettings.GetScoreByRating(distanceRating);
        
        //2. Как далеко от центра
        PhotoRating centerRating = evaluationSettings.GetRatingByCentering(worldCenter, photoCamera);
        int centerScore = evaluationSettings.GetScoreByRating(centerRating);
        
        
        // 3. Видимость(углы)
        PhotoRating visibilityRating = evaluationSettings.GetRatingByVisibility(visibleRatio);
        int visibilityScore = evaluationSettings.GetScoreByRating(visibilityRating);

        // 4. Центр + Видимость (усреднённое визуальное качество)
        int visualScore = Mathf.RoundToInt((centerScore + visibilityScore) / 2f);

        // 5. Общий результат
        int totalScore = distanceScore + visualScore;


        // Логи
        Debug.Log($"📏 Дистанция: {distance:0.00}м → [{distanceRating}] = {distanceScore} очков.");
        Debug.Log($"🎯 Центр: [{centerRating}] = {centerScore}");
        Debug.Log($"👁️ Видимость: {visibleRatio * 100f:0.0}% → [{visibilityRating}] = {visibilityScore}");
        Debug.Log($"🖼️ Визуальная оценка (среднее): {visualScore}");
        Debug.Log($"📷 Итоговая оценка: {totalScore}");

        return new PhotoResult
        {
            Rating = visibilityRating,
            Score = totalScore
        };
    }
    

    private float CalculateVisibleRatio(Bounds bounds)
    {
        Vector3[] corners = new Vector3[8];
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        corners[0] = new Vector3(min.x, min.y, min.z);
        corners[1] = new Vector3(max.x, min.y, min.z);
        corners[2] = new Vector3(min.x, max.y, min.z);
        corners[3] = new Vector3(max.x, max.y, min.z);
        corners[4] = new Vector3(min.x, min.y, max.z);
        corners[5] = new Vector3(max.x, min.y, max.z);
        corners[6] = new Vector3(min.x, max.y, max.z);
        corners[7] = new Vector3(max.x, max.y, max.z);
        int visible = 0;
        
        foreach (Vector3 corner in corners)
        {
            Vector3 viewportPoint = photoCamera.WorldToViewportPoint(corner);
            float marginX = 0.1f; // 10% слева и справа
            float marginY = 0.1f; // 10% сверху и снизу

            if (viewportPoint.z > 0 &&
                viewportPoint.x >= marginX && viewportPoint.x <= 1f - marginX &&
                viewportPoint.y >= marginY && viewportPoint.y <= 1f - marginY)
            {
                visible++;
            }
        }
        
        return visible / 8f;
    }
    private PhotoRating CombineRatings(params PhotoRating[] ratings)
    {
        int sum = ratings.Sum(r => (int)r);
        int avg = Mathf.RoundToInt(sum / (float)ratings.Length);
        return (PhotoRating)Mathf.Clamp(avg, 0, 4);
    }
    
    
}
