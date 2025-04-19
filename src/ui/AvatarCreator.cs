using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GezginDunya.Personalization
{
    /// <summary>
    /// Kullanıcı karakteri özelleştirme sistemi.
    /// Yüz, vücut, kıyafet ve aksesuarların seçimini ve renklendirilmesini sağlar.
    /// </summary>
    public class CharacterDesigner : MonoBehaviour
    {
        [Serializable]
        public class FeatureSet
        {
            public string featureId;
            public List<GameObject> variations = new List<GameObject>();
            public List<Sprite> thumbnails = new List<Sprite>();
            [HideInInspector] public int selectedIndex = 0;
        }

        [Serializable]
        public class PaletteSet
        {
            public string targetId;
            public List<Color> palette = new List<Color>();
            [HideInInspector] public int selectedIndex = 0;
        }

        [Header("Karakter Bileşenleri")]
        [SerializeField] private GameObject characterContainer;
        [SerializeField] private Transform previewLocation;

        [Header("Özelleştirme Grupları")]
        [SerializeField] private List<FeatureSet> featureSets = new List<FeatureSet>();
        [SerializeField] private List<PaletteSet> paletteSets = new List<PaletteSet>();

        [Header("Arayüz Elemanları")]
        [SerializeField] private Button forwardButton;
        [SerializeField] private Button backwardButton;
        [SerializeField] private Button completeButton;
        [SerializeField] private Transform tabContainer;
        [SerializeField] private GameObject tabButtonTemplate;
        [SerializeField] private Transform variantContainer;
        [SerializeField] private GameObject variantButtonTemplate;
        [SerializeField] private Transform colorPickerContainer;
        [SerializeField] private GameObject colorButtonTemplate;

        [Header("Olaylar")]
        public Action<Dictionary<string, int>> CharacterDesignCompleted;

        private Dictionary<string, GameObject> _activeComponents = new Dictionary<string, GameObject>();
        private Dictionary<string, Material> _appliedMaterials = new Dictionary<string, Material>();
        private int _activeTabIndex = 0;
        private bool _colorSelectionMode = false;

        private void Start()
        {
            InitializeInterface();
            SetupDefaultCharacter();
            ShowTab(0);
        }

        private void InitializeInterface()
        {
            // Gezinme butonlarını ayarla
            if (forwardButton != null)
                forwardButton.onClick.AddListener(NavigateForward);

            if (backwardButton != null)
                backwardButton.onClick.AddListener(NavigateBack);

            if (completeButton != null)
                completeButton.onClick.AddListener(FinalizeDesign);

            // Sekme butonlarını oluştur
            CreateTabButtons();
        }

        private void CreateTabButtons()
        {
            if (tabContainer == null || tabButtonTemplate == null)
                return;

            // Tüm özellik grupları için butonlar oluştur
            foreach (var featureSet in featureSets)
            {
                CreateTabButton(featureSet.featureId, false);
            }

            // Renk grupları için butonlar oluştur
            foreach (var paletteSet in paletteSets)
            {
                CreateTabButton(paletteSet.targetId + " Renk", true);
            }
        }

        private void CreateTabButton(string label, bool isColorTab)
        {
            GameObject tabObject = Instantiate(tabButtonTemplate, tabContainer);
            Button tabButton = tabObject.GetComponent<Button>();
            Text tabLabel = tabObject.GetComponentInChildren<Text>();

            if (tabLabel != null)
                tabLabel.text = label;

            if (tabButton != null)
            {
                if (isColorTab)
                {
                    int colorIndex = GetPaletteIndex(label.Replace(" Renk", ""));
                    tabButton.onClick.AddListener(() => ShowColorTab(colorIndex));
                }
                else
                {
                    int featureIndex = GetFeatureIndex(label);
                    tabButton.onClick.AddListener(() => ShowTab(featureIndex));
                }
            }
        }

        private int GetFeatureIndex(string featureId)
        {
            for (int i = 0; i < featureSets.Count; i++)
            {
                if (featureSets[i].featureId == featureId)
                    return i;
            }
            return 0;
        }

        private int GetPaletteIndex(string targetId)
        {
            for (int i = 0; i < paletteSets.Count; i++)
            {
                if (paletteSets[i].targetId == targetId)
                    return i;
            }
            return 0;
        }

        private void SetupDefaultCharacter()
        {
            if (characterContainer == null || previewLocation == null)
                return;

            // Her özellik grubu için ilk varyasyonu etkinleştir
            foreach (var feature in featureSets)
            {
                if (feature.variations.Count > 0)
                {
                    GameObject component = Instantiate(feature.variations[0], previewLocation);
                    _activeComponents[feature.featureId] = component;
                }
            }

            // Varsayılan renkleri uygula
            foreach (var palette in paletteSets)
            {
                if (palette.palette.Count > 0 && _activeComponents.TryGetValue(palette.targetId, out GameObject component))
                {
                    ApplyColor(component, palette.palette[0], palette.targetId);
                }
            }
        }

        private void ApplyColor(GameObject target, Color color, string materialKey)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material newMaterial = new Material(renderer.material);
                newMaterial.color = color;
                renderer.material = newMaterial;
                _appliedMaterials[materialKey] = newMaterial;
            }
        }

        private void ShowTab(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= featureSets.Count)
                return;

            _activeTabIndex = tabIndex;
            _colorSelectionMode = false;

            // UI'ı temizle
            ClearSelectionUI();

            // Renk seçiciyi gizle
            if (colorPickerContainer != null)
                colorPickerContainer.gameObject.SetActive(false);

            // Varyasyon seçiciyi göster
            if (variantContainer != null)
            {
                variantContainer.gameObject.SetActive(true);
                PopulateVariantButtons(tabIndex);
            }
        }

        private void ShowColorTab(int colorTabIndex)
        {
            if (colorTabIndex < 0 || colorTabIndex >= paletteSets.Count)
                return;

            _activeTabIndex = colorTabIndex;
            _colorSelectionMode = true;

            // UI'ı temizle
            ClearSelectionUI();

            // Varyasyon seçiciyi gizle
            if (variantContainer != null)
                variantContainer.gameObject.SetActive(false);

            // Renk seçiciyi göster
            if (colorPickerContainer != null)
            {
                colorPickerContainer.gameObject.SetActive(true);
                PopulateColorButtons(colorTabIndex);
            }
        }

        private void PopulateVariantButtons(int featureIndex)
        {
            if (variantContainer == null || variantButtonTemplate == null)
                return;

            FeatureSet feature = featureSets[featureIndex];

            // Her varyasyon için bir buton oluştur
            for (int i = 0; i < feature.variations.Count; i++)
            {
                CreateVariantButton(featureIndex, i, feature.thumbnails.Count > i ? feature.thumbnails[i] : null);
            }

            // Seçili varyasyonu vurgula
            HighlightSelectedVariant(feature.selectedIndex);
        }

        private void CreateVariantButton(int featureIndex, int variantIndex, Sprite thumbnail)
        {
            GameObject buttonObject = Instantiate(variantButtonTemplate, variantContainer);
            Button button = buttonObject.GetComponent<Button>();
            Image image = buttonObject.GetComponent<Image>();

            // Eğer varsa thumbnail'i göster
            if (image != null && thumbnail != null)
                image.sprite = thumbnail;

            // Tıklama olayını ekle
            button.onClick.AddListener(() => SelectVariant(featureIndex, variantIndex));

            // Varyasyon butonuna bir tanımlayıcı ekle
            buttonObject.name = $"Variant_{featureIndex}_{variantIndex}";
        }

        private void HighlightSelectedVariant(int index)
        {
            if (variantContainer == null)
                return;

            int childCount = variantContainer.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = variantContainer.GetChild(i);
                child.localScale = (i == index) ? new Vector3(1.2f, 1.2f, 1.2f) : Vector3.one;
            }
        }

        private void PopulateColorButtons(int paletteIndex)
        {
            if (colorPickerContainer == null || colorButtonTemplate == null)
                return;

            PaletteSet palette = paletteSets[paletteIndex];

            // Her renk için bir buton oluştur
            for (int i = 0; i < palette.palette.Count; i++)
            {
                CreateColorButton(paletteIndex, i, palette.palette[i]);
            }

            // Seçili rengi vurgula
            HighlightSelectedColor(palette.selectedIndex);
        }

        private void CreateColorButton(int paletteIndex, int colorIndex, Color color)
        {
            GameObject buttonObject = Instantiate(colorButtonTemplate, colorPickerContainer);
            Button button = buttonObject.GetComponent<Button>();
            Image image = buttonObject.GetComponent<Image>();

            // Rengi göster
            if (image != null)
                image.color = color;

            // Tıklama olayını ekle
            button.onClick.AddListener(() => SelectColor(paletteIndex, colorIndex));

            // Renk butonuna bir tanımlayıcı ekle
            buttonObject.name = $"Color_{paletteIndex}_{colorIndex}";
        }

        private void HighlightSelectedColor(int index)
        {
            if (colorPickerContainer == null)
                return;

            int childCount = colorPickerContainer.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = colorPickerContainer.GetChild(i);
                child.localScale = (i == index) ? new Vector3(1.2f, 1.2f, 1.2f) : Vector3.one;
            }
        }

        private void ClearSelectionUI()
        {
            // Varyasyon butonlarını temizle
            if (variantContainer != null)
            {
                foreach (Transform child in variantContainer)
                    Destroy(child.gameObject);
            }

            // Renk butonlarını temizle
            if (colorPickerContainer != null)
            {
                foreach (Transform child in colorPickerContainer)
                    Destroy(child.gameObject);
            }
        }

        private void SelectVariant(int featureIndex, int variantIndex)
        {
            if (featureIndex < 0 || featureIndex >= featureSets.Count)
                return;

            FeatureSet feature = featureSets[featureIndex];

            if (variantIndex < 0 || variantIndex >= feature.variations.Count)
                return;

            // Mevcut bileşeni kaldır
            if (_activeComponents.TryGetValue(feature.featureId, out GameObject currentComponent))
                Destroy(currentComponent);

            // Yeni bileşeni oluştur
            GameObject newComponent = Instantiate(feature.variations[variantIndex], previewLocation);
            _activeComponents[feature.featureId] = newComponent;

            // Seçimi kaydet
            feature.selectedIndex = variantIndex;

            // Eğer bu bileşen için bir renk paleti varsa, önceki rengi uygula
            foreach (var palette in paletteSets)
            {
                if (palette.targetId == feature.featureId)
                {
                    Color selectedColor = palette.palette[palette.selectedIndex];
                    ApplyColor(newComponent, selectedColor, feature.featureId);
                    break;
                }
            }

            // UI'ı güncelle
            HighlightSelectedVariant(variantIndex);
        }

        private void SelectColor(int paletteIndex, int colorIndex)
        {
            if (paletteIndex < 0 || paletteIndex >= paletteSets.Count)
                return;

            PaletteSet palette = paletteSets[paletteIndex];

            if (colorIndex < 0 || colorIndex >= palette.palette.Count)
                return;

            // Seçimi kaydet
            palette.selectedIndex = colorIndex;

            // Hedef bileşene rengi uygula
            if (_activeComponents.TryGetValue(palette.targetId, out GameObject targetComponent))
            {
                ApplyColor(targetComponent, palette.palette[colorIndex], palette.targetId);
            }

            // UI'ı güncelle
            HighlightSelectedColor(colorIndex);
        }

        private void NavigateForward()
        {
            if (_colorSelectionMode)
            {
                // Sonraki renk paletine git
                int nextIndex = _activeTabIndex + 1;
                if (nextIndex < paletteSets.Count)
                    ShowColorTab(nextIndex);
                else if (featureSets.Count > 0)
                    ShowTab(0); // İlk özellik grubuna dön
            }
            else
            {
                // Sonraki özellik grubuna git
                int nextIndex = _activeTabIndex + 1;
                if (nextIndex < featureSets.Count)
                    ShowTab(nextIndex);
                else if (paletteSets.Count > 0)
                    ShowColorTab(0); // İlk renk paletine geç
            }
        }

        private void NavigateBack()
        {
            if (_colorSelectionMode)
            {
                // Önceki renk paletine git
                int prevIndex = _activeTabIndex - 1;
                if (prevIndex >= 0)
                    ShowColorTab(prevIndex);
                else if (featureSets.Count > 0)
                    ShowTab(featureSets.Count - 1); // Son özellik grubuna git
            }
            else
            {
                // Önceki özellik grubuna git
                int prevIndex = _activeTabIndex - 1;
                if (prevIndex >= 0)
                    ShowTab(prevIndex);
                else if (paletteSets.Count > 0)
                    ShowColorTab(paletteSets.Count - 1); // Son renk paletine git
            }
        }

        private void FinalizeDesign()
        {
            // Tasarım tercihlerini sakla
            Dictionary<string, int> designChoices = new Dictionary<string, int>();

            // Özellik seçimlerini kaydet
            foreach (var feature in featureSets)
                designChoices[$"feature_{feature.featureId}"] = feature.selectedIndex;

            // Renk seçimlerini kaydet
            foreach (var palette in paletteSets)
                designChoices[$"color_{palette.targetId}"] = palette.selectedIndex;

            // Tasarım tamamlandı olayını bildir
            CharacterDesignCompleted?.Invoke(designChoices);

            Debug.Log("Karakter tasarımı tamamlandı.");
        }

        /// <summary>
        /// Mevcut karakter tasarımını verilen ID'lere göre günceller
        /// </summary>
        public void LoadDesign(Dictionary<string, int> savedDesign)
        {
            if (savedDesign == null)
                return;

            // Özellik seçimlerini yükle
            foreach (var feature in featureSets)
            {
                string key = $"feature_{feature.featureId}";
                if (savedDesign.TryGetValue(key, out int index) && index >= 0 && index < feature.variations.Count)
                {
                    feature.selectedIndex = index;
                    SelectVariant(GetFeatureIndex(feature.featureId), index);
                }
            }

            // Renk seçimlerini yükle
            foreach (var palette in paletteSets)
            {
                string key = $"color_{palette.targetId}";
                if (savedDesign.TryGetValue(key, out int index) && index >= 0 && index < palette.palette.Count)
                {
                    palette.selectedIndex = index;
                    SelectColor(GetPaletteIndex(palette.targetId), index);
                }
            }

            Debug.Log("Kayıtlı karakter tasarımı yüklendi.");
        }
    }
}