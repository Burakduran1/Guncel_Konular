using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace GezginDunya.XR
{
    /// <summary>
    /// Artırılmış gerçeklik için marker tanıma ve içerik gösterme sistemi.
    /// Kitap kapakları, sergiler, poster ve görsel öğeler gibi hedefleri
    /// tespit ederek 3B modelleri yerleştirmekten sorumludur.
    /// </summary>
    public class MarkerDetectionSystem : MonoBehaviour
    {
        [Serializable]
        public class MarkerContent
        {
            public string markerLabel;
            public GameObject contentModel;
            public Vector3 modelScale = Vector3.one;
            public Vector3 positionAdjustment = Vector3.zero;
            public Vector3 rotationAdjustment = Vector3.zero;
        }

        [Header("Bağlantılar")]
        [SerializeField] private ARTrackedImageManager markerManager;

        [Header("Marker İçerikleri")]
        [SerializeField] private List<MarkerContent> markerDatabase = new List<MarkerContent>();

        private readonly Dictionary<string, GameObject> _activeModels = new Dictionary<string, GameObject>();

        private void Awake()
        {
            if (markerManager == null)
                markerManager = FindObjectOfType<ARTrackedImageManager>();
        }

        private void OnEnable()
        {
            if (markerManager != null)
            {
                markerManager.trackedImagesChanged += HandleMarkerChanges;
                Debug.Log("Marker Sistemi: Takip aktifleştirildi");
            }
        }

        private void OnDisable()
        {
            if (markerManager != null)
            {
                markerManager.trackedImagesChanged -= HandleMarkerChanges;
                Debug.Log("Marker Sistemi: Takip devre dışı bırakıldı");
            }
        }

        private void HandleMarkerChanges(ARTrackedImagesChangedEventArgs eventData)
        {
            // Yeni tespit edilen markerlar için
            foreach (var marker in eventData.added)
            {
                SpawnModelForMarker(marker);
            }

            // Durumu değişen markerlar için
            foreach (var marker in eventData.updated)
            {
                UpdateModelPosition(marker);
            }

            // Kaybolan markerlar için
            foreach (var marker in eventData.removed)
            {
                RemoveModel(marker);
            }
        }

        private void SpawnModelForMarker(ARTrackedImage marker)
        {
            string markerName = marker.referenceImage.name;

            // Markera ait bir içerik var mı kontrol et
            MarkerContent content = markerDatabase.Find(item => item.markerLabel == markerName);

            if (content != null && content.contentModel != null)
            {
                // İçerik modelini oluştur
                GameObject model = Instantiate(content.contentModel);
                model.name = $"Model_{markerName}";

                // Yerleşim ayarları
                model.transform.position = marker.transform.position + content.positionAdjustment;
                model.transform.rotation = marker.transform.rotation * Quaternion.Euler(content.rotationAdjustment);
                model.transform.localScale = content.modelScale;

                // Kaydı tut
                _activeModels[markerName] = model;

                Debug.Log($"Marker Sistemi: '{markerName}' için model yerleştirildi");
            }
        }

        private void UpdateModelPosition(ARTrackedImage marker)
        {
            string markerName = marker.referenceImage.name;

            // Bu marker için bir model varsa
            if (_activeModels.TryGetValue(markerName, out GameObject model))
            {
                // Modelin görünürlüğünü takip durumuna göre ayarla
                bool isVisible = marker.trackingState == TrackingState.Tracking;
                model.SetActive(isVisible);

                if (isVisible)
                {
                    // Marker pozisyonu değiştikçe modeli güncelle
                    MarkerContent content = markerDatabase.Find(item => item.markerLabel == markerName);

                    if (content != null)
                    {
                        model.transform.position = marker.transform.position + content.positionAdjustment;
                        model.transform.rotation = marker.transform.rotation * Quaternion.Euler(content.rotationAdjustment);
                    }
                }
            }
        }

        private void RemoveModel(ARTrackedImage marker)
        {
            string markerName = marker.referenceImage.name;

            // Bu marker için bir model varsa
            if (_activeModels.TryGetValue(markerName, out GameObject model))
            {
                // Modeli kaldır
                Destroy(model);
                _activeModels.Remove(markerName);

                Debug.Log($"Marker Sistemi: '{markerName}' için model kaldırıldı");
            }
        }

        /// <summary>
        /// Çalışma zamanında yeni bir marker ve içerik tanımı ekler.
        /// </summary>
        public void RegisterMarkerContent(MarkerContent content)
        {
            if (content != null && !string.IsNullOrEmpty(content.markerLabel))
            {
                // Aynı etiketli marker varsa güncelle, yoksa ekle
                int existingIndex = markerDatabase.FindIndex(item => item.markerLabel == content.markerLabel);

                if (existingIndex >= 0)
                    markerDatabase[existingIndex] = content;
                else
                    markerDatabase.Add(content);

                Debug.Log($"Marker Sistemi: '{content.markerLabel}' veritabanına eklendi");
            }
        }
    }
}