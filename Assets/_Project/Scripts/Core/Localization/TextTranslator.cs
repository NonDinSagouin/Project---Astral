using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Core.Event;

namespace Core.Localization
{
    /// <summary>
    /// Traduit automatiquement le texte d'un composant Text en fonction de la langue actuelle.
    /// </summary>
    public class TextTranslator : MonoBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private TextMeshProUGUI TMPText;
        [SerializeField] private string key;

        [SerializeField] private bool startRefresh = true;

        void Start()
        {
            if (startRefresh)
            {
                RefreshKey();
                RefreshText();
            }

            EventBus.Subscribe<OnLanguageChangeEvent>(OnLanguageChange);
        }
        
        private void OnLanguageChange(OnLanguageChangeEvent evt)
        {
            if (text == null && TMPText == null)
            {
                throw new System.Exception("Text component not found.");
            }

            if (!StaticLocalization.TryGet(key, out string translatedText))
            {
                throw new System.Exception($"Translation not found for key: {key}");
            }

            if (text != null)
            {
                text.text = translatedText;
            }
            else if (TMPText != null)
            {
                TMPText.text = translatedText;
            }
        }

        /// <summary>
        /// Rafraîchit la clé de traduction en fonction du texte actuel du composant.
        /// </summary>
        public void RefreshKey()
        {
            text = GetComponent<Text>();
            TMPText = GetComponent<TextMeshProUGUI>();

            key = text != null ? text.text : TMPText != null ? TMPText.text : null;
        }

        /// <summary>
        /// Rafraîchit le texte du composant en fonction de la langue actuelle.
        /// </summary>
        public void RefreshText()
        {
            if (text == null && TMPText == null)
            {
                throw new System.Exception("Text component not found. Ensure that the GameObject has either a Text or TextMeshProUGUI component !");
            }

            OnLanguageChange(new OnLanguageChangeEvent());
        }   
    }
}
