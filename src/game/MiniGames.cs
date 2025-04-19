using System;
using System.Collections.Generic;
using UnityEngine;

namespace GezginDunya.Activities
{
    /// <summary>
    /// Eğitici aktiviteleri yöneten merkezi sistem.
    /// Çeşitli interaktif oyun ve görevleri başlatma, izleme ve ödüllendirme işlevlerini koordine eder.
    /// </summary>
    public class ActivityCenter : MonoBehaviour
    {
        [Serializable]
        public class ActivityData
        {
            public string activityKey;
            public string title;
            public string summary;
            public GameObject activityPrefab;
            public Sprite thumbnail;
            public int ageMin;
            public int ageMax;
            public float playTime; // Dakika olarak
            public int rewardValue = 15;
            public bool isFinished = false;
            public ActivityType type;
        }

        public enum ActivityType
        {
            Exploration,
            Cultural,
            Linguistic,
            Discovery,
            Logical,
            Entertainment
        }

        [Header("Aktivite Yapılandırması")]
        [SerializeField] private List<ActivityData> activityLibrary = new List<ActivityData>();
        [SerializeField] private Transform activityRoot;
        [SerializeField] private int userAge = 7;

        [Header("Bildirimler")]
        public event Action<ActivityData> ActivityLaunched;
        public event Action<ActivityData, int> ActivityFinished;
        public event Action<ActivityData> ActivityAborted;

        private ActivityData _activeActivity;
        private GameObject _activeInstance;
        private static ActivityCenter _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        public static ActivityCenter GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// Belirli bir aktivite türüne ve yaş seviyesine göre uygun aktiviteleri listeler.
        /// </summary>
        public List<ActivityData> ListActivities(ActivityType type, bool showCompleted = false)
        {
            List<ActivityData> results = new List<ActivityData>();

            foreach (var activity in activityLibrary)
            {
                bool ageMatch = userAge >= activity.ageMin && (activity.ageMax == 0 || userAge <= activity.ageMax);
                bool typeMatch = type == activity.type || type == ActivityType.Entertainment;
                bool statusMatch = showCompleted || !activity.isFinished;

                if (ageMatch && typeMatch && statusMatch)
                    results.Add(activity);
            }

            return results;
        }

        /// <summary>
        /// Belirtilen aktiviteyi başlatır
        /// </summary>
        public bool LaunchActivity(string activityKey)
        {
            // Varsa önceki aktiviteyi kapat
            if (_activeInstance != null)
            {
                CloseCurrentActivity();
            }

            // Aktivite bilgisini bul
            ActivityData data = activityLibrary.Find(a => a.activityKey == activityKey);
            if (data == null || data.activityPrefab == null)
            {
                Debug.LogWarning($"Aktivite bulunamadı veya prefab eksik: {activityKey}");
                return false;
            }

            // Yaş kontrolü
            if (userAge < data.ageMin || (data.ageMax > 0 && userAge > data.ageMax))
            {
                Debug.LogWarning($"Bu aktivite yaş grubunuza uygun değil: {activityKey}");
                return false;
            }

            // Aktiviteyi başlat
            _activeActivity = data;
            _activeInstance = Instantiate(data.activityPrefab, activityRoot);

            // Bildirimi gönder
            ActivityLaunched?.Invoke(data);

            Debug.Log($"Aktivite başlatıldı: {data.title}");
            return true;
        }

        /// <summary>
        /// Uygun aktivitelerden rastgele birini başlatır
        /// </summary>
        public bool LaunchRandomActivity(ActivityType type = ActivityType.Entertainment)
        {
            List<ActivityData> options = ListActivities(type);
            if (options.Count == 0)
            {
                Debug.LogWarning($"Bu kategoride uygun aktivite bulunamadı: {type}");
                return false;
            }

            int random = UnityEngine.Random.Range(0, options.Count);
            return LaunchActivity(options[random].activityKey);
        }

        /// <summary>
        /// Aktif aktiviteyi kapatır
        /// </summary>
        public void CloseCurrentActivity()
        {
            if (_activeInstance != null)
            {
                Destroy(_activeInstance);
                _activeInstance = null;

                ActivityAborted?.Invoke(_activeActivity);
                Debug.Log($"Aktivite kapatıldı: {_activeActivity.title}");

                _activeActivity = null;
            }
        }

        /// <summary>
        /// Aktif aktiviteyi tamamlandı olarak işaretler
        /// </summary>
        public void CompleteActivity(int result = 0)
        {
            if (_activeActivity != null)
            {
                _activeActivity.isFinished = true;

                ActivityFinished?.Invoke(_activeActivity, result);
                Debug.Log($"Aktivite tamamlandı: {_activeActivity.title}, sonuç: {result}");

                CloseCurrentActivity();
            }
        }

        /// <summary>
        /// Kullanıcı yaşını günceller
        /// </summary>
        public void UpdateUserAge(int newAge)
        {
            userAge = Mathf.Max(3, newAge);
            Debug.Log($"Kullanıcı yaşı güncellendi: {userAge}");
        }
    }

    /// <summary>
    /// Tüm aktivitelerin uygulaması gereken arayüz
    /// </summary>
    public interface IActivity
    {
        void Setup(Dictionary<string, object> config);
        void Begin();
        void Pause();
        void Resume();
        void Finish();
        int GetResult();
        float GetCompletion();
        event Action<int> ResultChanged;
        event Action<float> CompletionChanged;
        event Action ActivityCompleted;
    }

    /// <summary>
    /// Aktiviteler için temel sınıf. Tüm interaktif aktiviteler bu sınıftan türetilmelidir.
    /// </summary>
    public abstract class ActivityBase : MonoBehaviour, IActivity
    {
        [SerializeField] protected string activityKey;
        [SerializeField] protected string activityTitle;
        [SerializeField] protected int maximumResult = 100;

        protected int currentResult = 0;
        protected float completionRate = 0f;
        protected bool isActive = false;
        protected bool isPaused = false;
        protected Dictionary<string, object> configuration;

        public event Action<int> ResultChanged;
        public event Action<float> CompletionChanged;
        public event Action ActivityCompleted;

        public virtual void Setup(Dictionary<string, object> config)
        {
            configuration = config ?? new Dictionary<string, object>();
            ResetActivity();
        }

        public virtual void Begin()
        {
            if (!isActive)
            {
                isActive = true;
                isPaused = false;

                Debug.Log($"Aktivite başlatıldı: {activityTitle}");
            }
        }

        public virtual void Pause()
        {
            if (isActive && !isPaused)
            {
                isPaused = true;

                Debug.Log($"Aktivite duraklatıldı: {activityTitle}");
            }
        }

        public virtual void Resume()
        {
            if (isActive && isPaused)
            {
                isPaused = false;

                Debug.Log($"Aktivite devam ediyor: {activityTitle}");
            }
        }

        public virtual void Finish()
        {
            if (isActive)
            {
                isActive = false;
                isPaused = false;

                ActivityCompleted?.Invoke();
                Debug.Log($"Aktivite tamamlandı: {activityTitle}, sonuç: {currentResult}");
            }
        }

        public virtual int GetResult()
        {
            return currentResult;
        }

        public virtual float GetCompletion()
        {
            return completionRate;
        }

        protected virtual void ResetActivity()
        {
            currentResult = 0;
            completionRate = 0f;
            isActive = false;
            isPaused = false;

            ResultChanged?.Invoke(currentResult);
            CompletionChanged?.Invoke(completionRate);

            Debug.Log($"Aktivite sıfırlandı: {activityTitle}");
        }

        protected virtual void UpdateResult(int newResult)
        {
            currentResult = Mathf.Clamp(newResult, 0, maximumResult);
            ResultChanged?.Invoke(currentResult);
        }

        protected virtual void UpdateCompletion(float newRate)
        {
            completionRate = Mathf.Clamp01(newRate);
            CompletionChanged?.Invoke(completionRate);

            // Tamamlanma %100'e ulaştığında aktiviteyi bitir
            if (Math.Abs(completionRate - 1f) < 0.001f && isActive)
            {
                Finish();
            }
        }
    }

    /// <summary>
    /// Örnek bir aktivite: Dünya Kâşifi
    /// Çocukların harita üzerinde gizli yerleri keşfetmesi gereken interaktif bir görev.
    /// </summary>
    public class WorldExplorerActivity : ActivityBase
    {
        [Serializable]
        public class SecretLocation
        {
            public string locationName;
            public Vector2 coordinates;
            public float discoveryRange = 60f;
            public int value = 15;
            public bool discovered = false;
            public string clue;
        }

        [Header("Aktivite Ayarları")]
        [SerializeField] private List<SecretLocation> secretLocations = new List<SecretLocation>();
        [SerializeField] private float timeLimitSeconds = 180f;
        [SerializeField] private int availableClues = 3;

        private float _elapsedTime = 0f;
        private int _cluesUsed = 0;
        private int _discoveriesCount = 0;

        public override void Begin()
        {
            base.Begin();

            // Konfigürasyon parametrelerini oku
            if (configuration != null)
            {
                if (configuration.TryGetValue("timeLimit", out object time))
                    timeLimitSeconds = (float)time;

                if (configuration.TryGetValue("clues", out object clues))
                    availableClues = (int)clues;
            }

            // Aktiviteyi hazırla
            _elapsedTime = 0f;
            _cluesUsed = 0;
            _discoveriesCount = 0;

            foreach (var location in secretLocations)
                location.discovered = false;

            UpdateCompletion(0f);
            UpdateResult(0);
        }

        private void Update()
        {
            if (isActive && !isPaused)
            {
                // Süreyi takip et
                _elapsedTime += Time.deltaTime;

                // İlerleme durumunu güncelle
                UpdateCompletion(_elapsedTime / timeLimitSeconds);

                // Süre doldu mu?
                if (_elapsedTime >= timeLimitSeconds)
                {
                    Finish();
                }
            }
        }

        /// <summary>
        /// Verilen koordinatları kontrol eder ve eğer gizli bir konum varsa keşfeder.
        /// </summary>
        public bool InvestigateLocation(Vector2 coordinates)
        {
            if (!isActive || isPaused)
                return false;

            foreach (var location in secretLocations)
            {
                if (!location.discovered && Vector2.Distance(coordinates, location.coordinates) <= location.discoveryRange)
                {
                    // Keşfedildi!
                    location.discovered = true;
                    _discoveriesCount++;

                    // Puanı güncelle
                    UpdateResult(currentResult + location.value);

                    // Tüm lokasyonlar keşfedildi mi?
                    if (_discoveriesCount >= secretLocations.Count)
                    {
                        Finish();
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// İpucu kullanır ve keşfedilmemiş bir lokasyon hakkında bilgi verir.
        /// </summary>
        public string GetClue()
        {
            if (!isActive || isPaused || _cluesUsed >= availableClues)
                return null;

            // Keşfedilmemiş lokasyonları bul
            var undiscoveredLocations = secretLocations.FindAll(loc => !loc.discovered);
            if (undiscoveredLocations.Count == 0)
                return null;

            // Rastgele bir lokasyon seç
            int index = UnityEngine.Random.Range(0, undiscoveredLocations.Count);
            SecretLocation chosen = undiscoveredLocations[index];

            _cluesUsed++;

            return chosen.clue;
        }

        public override void Finish()
        {
            if (!isActive)
                return;

            // Keşfedilmeyen lokasyonlar için puan kır
            int missed = secretLocations.Count - _discoveriesCount;
            int penalty = missed * 7;
            UpdateResult(Mathf.Max(0, currentResult - penalty));

            base.Finish();
        }

        // API Metodları

        public int GetRemainingClues()
        {
            return availableClues - _cluesUsed;
        }

        public int GetDiscoveryCount()
        {
            return _discoveriesCount;
        }

        public int GetTotalLocationCount()
        {
            return secretLocations.Count;
        }

        public float GetRemainingTime()
        {
            return Mathf.Max(0, timeLimitSeconds - _elapsedTime);
        }
    }
}