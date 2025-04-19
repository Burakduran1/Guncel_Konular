using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace GezginDunya.XR
{
    /// <summary>
    /// Artırılmış gerçeklik sisteminin merkezi kontrol sınıfı. XR kamera, zemin tespiti, 
    /// marker yakalama ve diğer XR işlevlerini yönetir.
    /// </summary>
    public class XRController : MonoBehaviour
    {
        [Header("XR Bileşenleri")]
        [SerializeField] private ARSession xrSession;
        [SerializeField] private ARSessionOrigin xrSessionOrigin;
        [SerializeField] private ARCameraManager xrCameraController;
        [SerializeField] private ARRaycastManager xrRaycastController;
        [SerializeField] private ARPlaneManager xrSurfaceController;
        [SerializeField] private ARTrackedImageManager xrMarkerController;

        [Header("XR Ayarları")]
        [SerializeField] private bool surfaceDetectionEnabled = true;
        [SerializeField] private bool markerDetectionEnabled = true;
        [SerializeField] private float minSurfaceSize = 0.3f;

        // Tekil örnek
        public static XRController Main { get; private set; }

        private void Awake()
        {
            // Tekil örnek modeli
            if (Main != null && Main != this)
            {
                Destroy(gameObject);
                return;
            }

            Main = this;
            DontDestroyOnLoad(gameObject);

            // Bileşenlerin doğrulaması
            CheckComponents();
        }

        private void Start()
        {
            // XR altsistemlerini başlat
            SetupXRSubsystems();
        }

        private void CheckComponents()
        {
            // XR bileşenlerini kontrol et ve yoksa bul
            if (xrSession == null)
                xrSession = FindObjectOfType<ARSession>();

            if (xrSessionOrigin == null)
                xrSessionOrigin = FindObjectOfType<ARSessionOrigin>();

            if (xrCameraController == null && xrSessionOrigin != null)
                xrCameraController = xrSessionOrigin.camera.GetComponent<ARCameraManager>();

            if (xrRaycastController == null)
                xrRaycastController = FindObjectOfType<ARRaycastManager>();

            if (xrSurfaceController == null)
                xrSurfaceController = FindObjectOfType<ARPlaneManager>();

            if (xrMarkerController == null)
                xrMarkerController = FindObjectOfType<ARTrackedImageManager>();

            Debug.Log("XR Controller: Bileşenler doğrulandı");
        }

        private void SetupXRSubsystems()
        {
            // Yüzey tespiti ayarları
            if (xrSurfaceController != null)
            {
                xrSurfaceController.enabled = surfaceDetectionEnabled;
                xrSurfaceController.requestedDetectionMode = PlaneDetectionMode.Horizontal | PlaneDetectionMode.Vertical;
                Debug.Log("XR Controller: Yüzey tespiti hazırlandı");
            }

            // Marker tespiti ayarları
            if (xrMarkerController != null)
            {
                xrMarkerController.enabled = markerDetectionEnabled;
                xrMarkerController.requestedMaxNumberOfMovingImages = 3;
                Debug.Log("XR Controller: Marker tespiti hazırlandı");
            }
        }

        /// <summary>
        /// Belirtilen ekran konumunda XR yüzeyi olup olmadığını kontrol eder
        /// </summary>
        public bool TryGetSurfaceHit(Vector2 screenPoint, out ARRaycastHit hitResult)
        {
            if (xrRaycastController == null)
            {
                hitResult = default;
                return false;
            }

            var hitResults = new List<ARRaycastHit>();
            if (xrRaycastController.Raycast(screenPoint, hitResults, TrackableType.PlaneWithinPolygon))
            {
                hitResult = hitResults[0];
                return true;
            }

            hitResult = default;
            return false;
        }

        /// <summary>
        /// XR yüzey görsellerinin görünürlüğünü ayarlar
        /// </summary>
        public void SetSurfaceVisualization(bool isVisible)
        {
            if (xrSurfaceController != null)
            {
                foreach (var surface in xrSurfaceController.trackables)
                {
                    surface.gameObject.SetActive(isVisible);
                }
            }
        }

        /// <summary>
        /// Yüzey tespit sistemini açar veya kapatır
        /// </summary>
        public void SetSurfaceDetection(bool isEnabled)
        {
            if (xrSurfaceController != null)
            {
                surfaceDetectionEnabled = isEnabled;
                xrSurfaceController.enabled = isEnabled;
            }
        }

        /// <summary>
        /// Marker tespit sistemini açar veya kapatır
        /// </summary>
        public void SetMarkerDetection(bool isEnabled)
        {
            if (xrMarkerController != null)
            {
                markerDetectionEnabled = isEnabled;
                xrMarkerController.enabled = isEnabled;
            }
        }
    }
}