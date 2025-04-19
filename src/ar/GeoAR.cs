using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GezginDunya.XR
{
    /// <summary>
    /// Konum tabanlı artırılmış gerçeklik deneyimleri yöneticisi.
    /// Seyahat rotasındaki önemli noktaları takip eder ve bu noktalara
    /// yaklaşıldığında ilgili dijital içerikleri görüntüler.
    /// </summary>
    public class LocationBasedXR : MonoBehaviour
    {
        public class GPSPoint
        {
            [SerializeField] public string pointID;
            [SerializeField] public string displayName;
            [SerializeField] public double gpsLatitude;
            [SerializeField] public double gpsLongitude;
            [SerializeField] public float triggerRadius = 150f; // Metre cinsinden
            [SerializeField] public GameObject xrContent;
            [SerializeField] public string infoText;
            [SerializeField] public int rewardPoints = 5;
            [NonSerialized] public bool isDiscovered = false;
        }

        [System.Serializable]
        public class PointOfInterest : GPSPoint { }

        // Olaylar
        public delegate void LocationEventHandler(PointOfInterest poi);
        public event LocationEventHandler OnPointEntered;
        public event LocationEventHandler OnPointExited;

        [Header("Nokta Veritabanı")]
        [SerializeField] private List<PointOfInterest> pointsOfInterest = new List<PointOfInterest>();

        [Header("Konum Ayarları")]
        [SerializeField] private float locationCheckFrequency = 3f;
        [SerializeField] private bool debugMode = false;
        [SerializeField] private Vector2 debugLocation = new Vector2(39.9334f, 32.8597f); // Ankara

        // Özel alanlar
        private readonly Dictionary<string, GameObject> _activeContent = new Dictionary<string, GameObject>();
        private Vector2 _deviceLocation;
        private bool _isLocationTrackingActive = false;
        private float _checkTimer = 0f;

        #region Yaşam Döngüsü

        private void Start()
        {
            InitializeLocationTracking();
        }

        private void Update()
        {
            _checkTimer += Time.deltaTime;

            if (_checkTimer >= locationCheckFrequency)
            {
                _checkTimer = 0f;
                RefreshDeviceLocation();
                ProcessLocationTriggers();
            }
        }

        private void OnDestroy()
        {
            if (_isLocationTrackingActive && !debugMode)
            {
                Input.location.Stop();
                Debug.Log("Konum izleyici: GPS servisi durduruldu");
            }

            // Aktif içerikleri temizle
            foreach (var content in _activeContent.Values)
            {
                if (content != null)
                    Destroy(content);
            }

            _activeContent.Clear();
        }

        #endregion

        #region Konum İzleme

        private async void InitializeLocationTracking()
        {
            if (debugMode)
            {
                _deviceLocation = debugLocation;
                Debug.Log($"Konum izleyici: Test modu aktif - Konum: {_deviceLocation}");
                return;
            }

            // Konum servisleri etkin mi kontrol et
            if (!Input.location.isEnabledByUser)
            {
                Debug.LogWarning("Konum izleyici: Konum servisleri kullanıcı tarafından etkinleştirilmemiş");
                return;
            }

            // Konum servisini başlat
            Input.location.Start(5f, 10f);

            try
            {
                // Servisin başlamasını bekle
                bool initialized = await WaitForLocationInitialization();

                if (!initialized)
                {
                    Debug.LogError("Konum izleyici: GPS servisi başlatılamadı");
                    return;
                }

                _isLocationTrackingActive = true;
                _deviceLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
                Debug.Log($"Konum izleyici: GPS başlatıldı - Mevcut konum: {_deviceLocation}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Konum izleyici: Hata oluştu - {e.Message}");
            }
        }

        private async Task<bool> WaitForLocationInitialization()
        {
            int maxWaitTime = 15; // saniye

            while (Input.location.status == LocationServiceStatus.Initializing && maxWaitTime > 0)
            {
                await Task.Delay(1000);
                maxWaitTime--;
            }

            return Input.location.status == LocationServiceStatus.Running;
        }

        private void RefreshDeviceLocation()
        {
            if (debugMode)
            {
                // Test modunda yapay konum hareketi eklenebilir
                return;
            }

            if (_isLocationTrackingActive && Input.location.status == LocationServiceStatus.Running)
            {
                _deviceLocation = new Vector2(
                    Input.location.lastData.latitude,
                    Input.location.lastData.longitude
                );
            }
        }

        #endregion

        #region Nokta İşlemleri

        private void ProcessLocationTriggers()
        {
            foreach (var poi in pointsOfInterest)
            {
                Vector2 poiLocation = new Vector2((float)poi.gpsLatitude, (float)poi.gpsLongitude);
                float distanceInKm = CalculateGpsDistance(_deviceLocation, poiLocation);
                float distanceInMeters = distanceInKm * 1000;

                bool isNearPoint = distanceInMeters <= poi.triggerRadius;
                bool hasActiveContent = _activeContent.ContainsKey(poi.pointID);

                // Noktaya yaklaştık ve henüz içerik gösterilmiyorsa
                if (isNearPoint && !hasActiveContent)
                {
                    ActivatePointContent(poi);

                    // İlk kez keşfedildi mi kontrol et
                    if (!poi.isDiscovered)
                    {
                        poi.isDiscovered = true;
                        // Burada puanlama sistemine entegrasyon yapılabilir
                        Debug.Log($"Konum izleyici: '{poi.displayName}' ilk kez keşfedildi! +{poi.rewardPoints} puan");
                    }
                }
                // Noktadan uzaklaştık ve içerik gösteriliyorsa
                else if (!isNearPoint && hasActiveContent)
                {
                    DeactivatePointContent(poi);
                }
            }
        }

        private void ActivatePointContent(PointOfInterest poi)
        {
            if (poi.xrContent != null)
            {
                GameObject instance = Instantiate(poi.xrContent);
                instance.name = $"POI_{poi.pointID}";
                _activeContent[poi.pointID] = instance;

                Debug.Log($"Konum izleyici: '{poi.displayName}' içeriği etkinleştirildi");
                OnPointEntered?.Invoke(poi);
            }
        }

        private void DeactivatePointContent(PointOfInterest poi)
        {
            if (_activeContent.TryGetValue(poi.pointID, out GameObject instance))
            {
                Destroy(instance);
                _activeContent.Remove(poi.pointID);

                Debug.Log($"Konum izleyici: '{poi.displayName}' içeriği kaldırıldı");
                OnPointExited?.Invoke(poi);
            }
        }

        #endregion

        #region Yardımcı Metotlar

        // İki GPS koordinatı arasındaki mesafeyi kilometre olarak hesaplar (Haversine formülü)
        private float CalculateGpsDistance(Vector2 from, Vector2 to)
        {
            const float EARTH_RADIUS = 6371.0f; // Dünya yarıçapı (km)

            float fromLat = from.x * Mathf.Deg2Rad;
            float fromLng = from.y * Mathf.Deg2Rad;
            float toLat = to.x * Mathf.Deg2Rad;
            float toLng = to.y * Mathf.Deg2Rad;

            float deltaLat = toLat - fromLat;
            float deltaLng = toLng - fromLng;

            // Haversine formülü
            float a = Mathf.Pow(Mathf.Sin(deltaLat / 2), 2) +
                      Mathf.Cos(fromLat) * Mathf.Cos(toLat) *
                      Mathf.Pow(Mathf.Sin(deltaLng / 2), 2);

            float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
            return EARTH_RADIUS * c;
        }

        #endregion

        #region Genel API

        // Tüm noktaların kopyasını döndürür
        public List<PointOfInterest> GetAllPoints()
        {
            return new List<PointOfInterest>(pointsOfInterest);
        }

        // Veritabanına yeni bir nokta ekler
        public void RegisterPoint(PointOfInterest newPoint)
        {
            if (newPoint != null && !string.IsNullOrEmpty(newPoint.pointID))
            {
                // Aynı ID'ye sahip nokta var mı kontrol et
                int index = pointsOfInterest.FindIndex(p => p.pointID == newPoint.pointID);

                if (index >= 0)
                {
                    pointsOfInterest[index] = newPoint;
                    Debug.Log($"Konum izleyici: '{newPoint.displayName}' güncellendi");
                }
                else
                {
                    pointsOfInterest.Add(newPoint);
                    Debug.Log($"Konum izleyici: '{newPoint.displayName}' eklendi");
                }
            }
        }

        // Test modunda konum simülasyonu için
        public void SetDebugLocation(float latitude, float longitude)
        {
            if (debugMode)
            {
                debugLocation = new Vector2(latitude, longitude);
                _deviceLocation = debugLocation;
                Debug.Log($"Konum izleyici: Test konumu güncellendi - {_deviceLocation}");
            }
        }

        #endregion
    }
}