    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "PhotoEvaluationSettings", menuName ="PhotoSystem/Evaluation Settings")]
    public class PhotoEvaluationSettings : ScriptableObject
    {
        
        [System.Serializable]
        public class VisibilityRatingThreshold
        {
            public string Name;
            [Range(0, 1)] public float minVisibility;
            public PhotoRating rating;
        }
        [System.Serializable]
        public class TargetTypeSettings
        {
            public PhotoTargetType type;
            public float minDistance = 2f;
            public float maxDistance = 15f;
        }
        
        public VisibilityRatingThreshold[] visibilityThresholds;
        public TargetTypeSettings[] targetTypeSettings;
        
        #region Variables
        
        [Header("Estimation Settings")]
        [SerializeField] private int terribleScore = 0;
        [SerializeField] private int badScore = 125;
        [SerializeField] private int normalScore = 250;
        [SerializeField] private int goodScore = 350;
        [Tooltip("Maximum must be 500")]
        [SerializeField] private int perfectScore = 500;

        #endregion

        public int GetScoreByRating(PhotoRating rating)
        {
            return rating switch
            {
                PhotoRating.Perfect => perfectScore,
                PhotoRating.Good => goodScore,
                PhotoRating.Normal => normalScore,
                PhotoRating.Bad => badScore,
                _ => terribleScore
            };
        }

        public PhotoRating GetRatingByVisibility(float ratio)
        {
            foreach (var threshold in visibilityThresholds.OrderByDescending(t => t.minVisibility))
            {
                if (ratio >= threshold.minVisibility)
                    return threshold.rating;
            }
            return PhotoRating.Terrible;
        }
        public TargetTypeSettings GetSettingsForType(PhotoTargetType type)
        {
            var settings = targetTypeSettings.FirstOrDefault(s => s.type == type);
            if (settings == null)
            {
                Debug.LogWarning($"[PhotoEvaluationSettings] No settings found for type: {type}. Using default.");
                return new TargetTypeSettings();
            }
            return settings;
        }
        public PhotoRating GetRatingByDistance(float distance, PhotoTargetType type)
        {
            var settings = GetSettingsForType(type);
            float min = settings.minDistance;
            float max = settings.maxDistance;
            float range = max - min;

            float delta = 0.3f * range;

            if (distance >= min && distance <= max)
                return PhotoRating.Perfect;
            if (distance >= min - delta && distance <= max + delta)
                return PhotoRating.Good;
            if (distance >= min - 2 * delta && distance <= max + 2 * delta)
                return PhotoRating.Normal;
            if (distance >= min - 3 * delta && distance <= max + 3 * delta)
                return PhotoRating.Bad;

            return PhotoRating.Terrible;
        }
        public PhotoRating GetRatingByCentering(Vector3 worldPos, Camera cam)
        {
            Vector3 viewportPoint = cam.WorldToViewportPoint(worldPos);
            if (viewportPoint.z < 0)
                return PhotoRating.Terrible;

            Vector2 screenCenter = new Vector2(0.5f, 0.5f);
            Vector2 objectPos = new Vector2(viewportPoint.x, viewportPoint.y);
            float distanceFromCenter = Vector2.Distance(screenCenter, objectPos);

            if (distanceFromCenter <= 0.1f) return PhotoRating.Perfect;
            if (distanceFromCenter <= 0.15f) return PhotoRating.Good;
            if (distanceFromCenter <= 0.25f) return PhotoRating.Normal;
            if (distanceFromCenter <= 0.35f) return PhotoRating.Bad;
            return PhotoRating.Terrible;
        }
        
    }
