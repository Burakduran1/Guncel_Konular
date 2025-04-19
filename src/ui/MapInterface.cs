using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GezginDunya.Navigation
{
    /// <summary>
    /// Üç boyutlu dünya haritası ve seyahat rotası görüntüleme sistemi.
    /// Ülkeleri, şehirleri ve önemli noktaları küresel harita üzerinde işaretler.
    /// </summary>
    public class WorldMapViewer : MonoBehaviour
    {
        [Serializable]
        public class Destination
        {
            public string id;
            public string title;
            public string regionCode;
            public Vector3 worldPosition;
            public string info;
            public Sprite markerIcon;
            public GameObject contentPrefab;
            public bool discovered = false;
            public List<string> trivia = new List<string>();
            [Range(0, 100)] public int scoreValue = 15;
        }

        [Serializable]
        public class TravelRoute
        {
            public string routeId;
            public Destination origin;
            public Destination target;
            public List<Destination> stopPoints = new List<Destination>();
            public Color lineColor = Color.cyan;
            public float journeyTime; // Dakika olarak
        }

        [Header("Harita Bileşenleri")]
        [SerializeField] private Transform globeParent;
        [SerializeField] private GameObject globeModelPrefab;
        [SerializeField] private GameObject destinationPinPrefab;
        [SerializeField] private GameObject routeLinePrefab;
        [SerializeField] private GameObject positionIndicatorPrefab;

        [Header("Destinasyonlar")]
        [SerializeField] private List<Destination> destinationDatabase = new List<Destination>();

        [Header("Rota Bilgileri")]
        [SerializeField] private TravelRoute activeRoute;
        [SerializeField] private float journeyProgress = 0f; // 0-1 arası değer
        [SerializeField] private bool simulateJourney = false;
        [SerializeField] private float timeScaleFactor = 1f; // Normal zamanın kaç katı hızda simülasyon yapılacak

        [Header("Arayüz Bileşenleri")]
        [SerializeField] private Text journeyInfoDisplay;
        [SerializeField] private Text destinationInfoDisplay;
        [SerializeField] private Image progressIndicator;
        [SerializeField] private RectTransform itineraryPanel;
        [SerializeField] private GameObject itineraryItemPrefab;

        [Header("Olaylar")]
        public event Action<Destination> DestinationSelected;
        public event Action<Destination> DestinationArrived;
        public event Action<float> JourneyProgressUpdated;
        public event Action<TravelRoute> JourneyCompleted;

        private GameObject _globeModel;
        private GameObject _positionIndicator;
        private readonly List<GameObject> _destinationPins = new List<GameObject>();
        private readonly List<GameObject> _routeSegments = new List<GameObject>();
        private readonly Dictionary<string, GameObject> _itineraryItems = new Dictionary<string, GameObject>();

        private float _journeyStartTimestamp;
        private float _previousProgress = 0f;

        #region Yaşam Döngüsü

        private void Start()
        {
            SetupGlobe();
            DrawActiveRoute();
            PlaceDestinationPins();
            BuildItineraryPanel();
            RefreshInterface();

            if (simulateJourney)
                BeginJourneySimulation();
        }

        private void Update()
        {
            if (simulateJourney)
                UpdateJourneySimulation();

            UpdatePositionIndicator();

            // İlerleme değişimini kontrol et
            if (Math.Abs(journeyProgress - _previousProgress) > 0.01f)
            {
                _previousProgress = journeyProgress;
                JourneyProgressUpdated?.Invoke(journeyProgress);
                RefreshInterface();

                // Destinasyona ulaşma kontrolü
                CheckDestinationArrival();
            }
        }

        #endregion

        #region Harita Kurulumu

        private void SetupGlobe()
        {
            if (globeParent == null || globeModelPrefab == null)
                return;

            // Dünya modelini oluştur
            _globeModel = Instantiate(globeModelPrefab, globeParent);
            _globeModel.transform.localPosition = Vector3.zero;

            Debug.Log("Dünya haritası oluşturuldu");
        }

        private void DrawActiveRoute()
        {
            if (activeRoute == null || routeLinePrefab == null)
                return;

            // Önceki rota çizgilerini temizle
            foreach (var segment in _routeSegments)
                Destroy(segment);

            _routeSegments.Clear();

            // Başlangıç noktası
            Vector3 previousPosition = activeRoute.origin.worldPosition;

            // Ara duraklar için çizgiler oluştur
            foreach (var stop in activeRoute.stopPoints)
            {
                CreateRouteSegment(previousPosition, stop.worldPosition);
                previousPosition = stop.worldPosition;
            }

            // Son durak için çizgi oluştur
            CreateRouteSegment(previousPosition, activeRoute.target.worldPosition);

            // Konum göstergesini oluştur
            if (positionIndicatorPrefab != null)
            {
                _positionIndicator = Instantiate(positionIndicatorPrefab, globeParent);
                _positionIndicator.transform.position = activeRoute.origin.worldPosition;
            }

            Debug.Log("Seyahat rotası çizildi");
        }

        private void CreateRouteSegment(Vector3 start, Vector3 end)
        {
            GameObject segment = Instantiate(routeLinePrefab, globeParent);
            LineRenderer lineRenderer = segment.GetComponent<LineRenderer>();

            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, start);
                lineRenderer.SetPosition(1, end);

                lineRenderer.startColor = activeRoute.lineColor;
                lineRenderer.endColor = activeRoute.lineColor;
            }

            _routeSegments.Add(segment);
        }

        private void PlaceDestinationPins()
        {
            if (destinationPinPrefab == null)
                return;

            // Önceki işaretleri temizle
            foreach (var pin in _destinationPins)
                Destroy(pin);

            _destinationPins.Clear();

            // Her destinasyon için işaret oluştur
            foreach (var destination in destinationDatabase)
            {
                GameObject pin = Instantiate(destinationPinPrefab, globeParent);
                pin.transform.position = destination.worldPosition;
                pin.name = $"Pin_{destination.id}";

                // İkonu ayarla
                if (destination.markerIcon != null)
                {
                    SpriteRenderer iconRenderer = pin.GetComponentInChildren<SpriteRenderer>();
                    if (iconRenderer != null)
                        iconRenderer.sprite = destination.markerIcon;
                }

                // Etkileşim bileşenini ayarla
                DestinationPin pinComponent = pin.GetComponent<DestinationPin>();
                if (pinComponent != null)
                {
                    pinComponent.Initialize(destination);
                    pinComponent.PinSelected += SelectDestination;
                }

                _destinationPins.Add(pin);
            }

            Debug.Log("Destinasyon işaretleri yerleştirildi");
        }

        private void BuildItineraryPanel()
        {
            if (itineraryPanel == null || itineraryItemPrefab == null || activeRoute == null)
                return;

            // Önceki öğeleri temizle
            foreach (Transform child in itineraryPanel)
                Destroy(child.gameObject);

            _itineraryItems.Clear();

            // Başlangıç noktası için öğe ekle
            CreateItineraryItem(activeRoute.origin, 0);

            // Ara duraklar için öğeler
            for (int i = 0; i < activeRoute.stopPoints.Count; i++)
                CreateItineraryItem(activeRoute.stopPoints[i], i + 1);

            // Hedef noktası için öğe
            CreateItineraryItem(activeRoute.target, activeRoute.stopPoints.Count + 1);

            Debug.Log("Rota paneli oluşturuldu");
        }

        private void CreateItineraryItem(Destination destination, int order)
        {
            GameObject itemObj = Instantiate(itineraryItemPrefab, itineraryPanel);
            itemObj.name = $"Item_{destination.id}";

            Button button = itemObj.GetComponent<Button>();
            Text itemLabel = itemObj.GetComponentInChildren<Text>();
            Image background = itemObj.GetComponent<Image>();

            if (itemLabel != null)
                itemLabel.text = destination.title;

            if (button != null)
                button.onClick.AddListener(() => SelectDestination(destination));

            // Keşfedilmiş noktaları vurgula
            if (background != null && destination.discovered)
                background.color = new Color(0.2f, 0.8f, 0.2f, 0.5f);

            _itineraryItems[destination.id] = itemObj;
        }

        #endregion

        #region Etkileşim ve Navigasyon

        private void SelectDestination(Destination destination)
        {
            if (destination == null)
                return;

            // Destinasyon bilgilerini göster
            if (destinationInfoDisplay != null)
                destinationInfoDisplay.text = $"{destination.title}\n{destination.info}";

            // Seçim olayını bildir
            DestinationSelected?.Invoke(destination);

            Debug.Log($"Destinasyon seçildi: {destination.title}");
        }

        private void BeginJourneySimulation()
        {
            _journeyStartTimestamp = Time.time;
            journeyProgress = 0f;

            Debug.Log("Seyahat simülasyonu başlatıldı");
        }

        private void UpdateJourneySimulation()
        {
            if (activeRoute == null || activeRoute.journeyTime <= 0)
                return;

            // İlerlemeyi güncelle
            float elapsedMinutes = (Time.time - _journeyStartTimestamp) * timeScaleFactor;
            journeyProgress = Mathf.Clamp01(elapsedMinutes / activeRoute.journeyTime);

            // Tamamlanma durumunu kontrol et
            if (journeyProgress >= 1.0f && !activeRoute.target.discovered)
            {
                activeRoute.target.discovered = true;
                JourneyCompleted?.Invoke(activeRoute);

                Debug.Log("Seyahat tamamlandı");
            }
        }

        private void UpdatePositionIndicator()
        {
            if (_positionIndicator == null || activeRoute == null)
                return;

            // İlerlemeye göre konum hesapla
            Vector3 position = CalculatePositionAtProgress(journeyProgress);
            _positionIndicator.transform.position = position;
        }

        private Vector3 CalculatePositionAtProgress(float progress)
        {
            if (activeRoute == null)
                return Vector3.zero;

            // Rota noktalarını listele
            List<Vector3> points = new List<Vector3>();
            points.Add(activeRoute.origin.worldPosition);

            foreach (var stop in activeRoute.stopPoints)
                points.Add(stop.worldPosition);

            points.Add(activeRoute.target.worldPosition);

            // Toplam mesafeyi hesapla
            float totalDistance = 0f;
            List<float> segmentLengths = new List<float>();

            for (int i = 1; i < points.Count; i++)
            {
                float length = Vector3.Distance(points[i - 1], points[i]);
                segmentLengths.Add(length);
                totalDistance += length;
            }

            // İlerleme mesafesini hesapla
            float targetDistance = totalDistance * progress;
            float traversedDistance = 0f;

            // Hangi segmentte olduğunu belirle
            for (int i = 0; i < segmentLengths.Count; i++)
            {
                if (traversedDistance + segmentLengths[i] >= targetDistance)
                {
                    // Segment içindeki ilerleme oranını hesapla
                    float segmentProgress = (targetDistance - traversedDistance) / segmentLengths[i];
                    return Vector3.Lerp(points[i], points[i + 1], segmentProgress);
                }

                traversedDistance += segmentLengths[i];
            }

            return activeRoute.target.worldPosition;
        }

        private void CheckDestinationArrival()
        {
            if (activeRoute == null)
                return;

            // Ara durakları kontrol et
            foreach (var stop in activeRoute.stopPoints)
            {
                // Bu durağın geçilip geçilmediğini kontrol et
                int stopIndex = activeRoute.stopPoints.IndexOf(stop);
                float progressAtStop = CalculateProgressAtStopPoint(stopIndex);

                if (journeyProgress >= progressAtStop && !stop.discovered)
                {
                    MarkDestinationAsDiscovered(stop);
                }
            }

            // Hedef noktayı kontrol et
            if (journeyProgress >= 1.0f && !activeRoute.target.discovered)
            {
                MarkDestinationAsDiscovered(activeRoute.target);
            }
        }

        private void MarkDestinationAsDiscovered(Destination destination)
        {
            // Keşfedildi olarak işaretle
            destination.discovered = true;

            // UI'ı güncelle
            if (_itineraryItems.TryGetValue(destination.id, out GameObject itemObj))
            {
                Image background = itemObj.GetComponent<Image>();
                if (background != null)
                    background.color = new Color(0.2f, 0.8f, 0.2f, 0.5f);
            }

            // Varış olayını bildir
            DestinationArrived?.Invoke(destination);

            Debug.Log($"Destinasyona varıldı: {destination.title}");
        }

        private float CalculateProgressAtStopPoint(int stopIndex)
        {
            if (activeRoute == null || stopIndex < 0 || stopIndex >= activeRoute.stopPoints.Count)
                return 0f;

            // Tüm noktaları listele
            List<Vector3> points = new List<Vector3>();
            points.Add(activeRoute.origin.worldPosition);

            foreach (var stop in activeRoute.stopPoints)
                points.Add(stop.worldPosition);

            points.Add(activeRoute.target.worldPosition);

            // Toplam mesafeyi hesapla
            float totalDistance = 0f;
            List<float> segmentLengths = new List<float>();

            for (int i = 1; i < points.Count; i++)
            {
                float length = Vector3.Distance(points[i - 1], points[i]);
                segmentLengths.Add(length);
                totalDistance += length;
            }

            // Durağa kadar olan mesafeyi hesapla
            float distanceToStop = 0f;
            for (int i = 0; i <= stopIndex; i++)
                distanceToStop += segmentLengths[i];

            // İlerleme yüzdesini hesapla
            return distanceToStop / totalDistance;
        }

        #endregion

        #region Arayüz ve API

        private void RefreshInterface()
        {
            // İlerleme göstergesini güncelle
            if (progressIndicator != null)
                progressIndicator.fillAmount = journeyProgress;

            // Seyahat bilgilerini güncelle
            if (journeyInfoDisplay != null && activeRoute != null)
            {
                float remainingTime = (1 - journeyProgress) * activeRoute.journeyTime;
                int remainingHours = Mathf.FloorToInt(remainingTime / 60);
                int remainingMinutes = Mathf.FloorToInt(remainingTime % 60);

                journeyInfoDisplay.text = $"Rota: {activeRoute.routeId}\n" +
                                      $"Çıkış: {activeRoute.origin.title}\n" +
                                      $"Varış: {activeRoute.target.title}\n" +
                                      $"Kalan Süre: {remainingHours}s {remainingMinutes}d";
            }
        }

        public void SetJourneyProgress(float progress)
        {
            journeyProgress = Mathf.Clamp01(progress);
        }

        public void ChangeActiveRoute(TravelRoute route)
        {
            activeRoute = route;
            journeyProgress = 0f;

            DrawActiveRoute();
            BuildItineraryPanel();
            RefreshInterface();

            Debug.Log($"Aktif rota değiştirildi: {route.routeId}");
        }

        public TravelRoute GetActiveRoute()
        {
            return activeRoute;
        }

        #endregion
    }

    /// <summary>
    /// Harita üzerindeki destinasyon işaretlerini yönetir
    /// </summary>
    public class DestinationPin : MonoBehaviour
    {
        public WorldMapViewer.Destination DestinationData { get; private set; }
        public event Action<WorldMapViewer.Destination> PinSelected;

        public void Initialize(WorldMapViewer.Destination data)
        {
            DestinationData = data;
        }

        private void OnMouseDown()
        {
            PinSelected?.Invoke(DestinationData);
        }
    }
}