using System;
using System.Linq;
using UnityEngine;

using Core;
using Core.Event;
using Core.Localization;

namespace Core.Manager
{ 
    /// <summary>
    /// Représente les langues disponibles pour la localisation.
    /// </summary>
    public enum Language
    {
        En, // English
        Fr  // French
    }

    /// <summary>
    /// Fournit un accès statique aux traductions localisées et à la gestion de la langue actuelle.
    /// </summary>
    public class LocalizationManager : Singleton<LocalizationManager>
    {
        /// <summary>
        /// Fournit la langue actuelle utilisée pour la localisation.
        /// </summary>
        [SerializeField] private Language CURRENT_LANGUAGE = Language.En;
        
        private LocalizationData Translations;
        private const string RESOURCE_PATH = "Localization";

        protected override void Awake()
        {
            base.Awake();
            Translations = Load();
        }

        /// <summary>
        /// Change la langue actuelle pour la localisation et publie un événement de changement de langue.
        /// </summary>
        /// <param name="language">La langue à utiliser pour la localisation.</param>
        public void SwitchLanguage(Language language)
        {
            CURRENT_LANGUAGE = language;
            EventBus.Publish(new OnLanguageChangeEvent());
        }

        /// <summary>
        /// Essaie de récupérer la traduction pour une clé donnée dans la langue spécifiée.
        /// Si la langue n'est pas spécifiée, utilise la langue actuelle.
        /// </summary>
        /// <param name="key">La clé de la traduction.</param>
        /// <param name="value">La valeur traduite correspondante si trouvée.</param>
        /// <param name="language">La langue dans laquelle récupérer la traduction. Si null, utilise la langue actuelle.</param>
        /// <returns>True si la traduction est trouvée, sinon false.</returns>
        public bool TryGet(string key, out string value, Language? language = null)
        {
            language ??= CURRENT_LANGUAGE;
            var mapping = Translations.table.FirstOrDefault(m => m.key == key);
            var entry = mapping?.values.FirstOrDefault(v => v.key == language.ToString());
            value = entry?.value;
            return !string.IsNullOrEmpty(value);
        }

        private LocalizationData Load()
        {
            var assets = Resources.LoadAll<TextAsset>(RESOURCE_PATH);

            if (assets.Length == 0)
            {
                throw new Exception($"No localization JSON files found in Resources/{RESOURCE_PATH}.");
            }

            var data = assets
                .OrderBy(asset => asset.name, StringComparer.Ordinal)
                .Select(asset =>
                { 
                    var localization = JsonUtility.FromJson<LocalizationData>(asset.text);

                    if (localization == null || localization.table == null)
                    {
                        throw new Exception($"Invalid localization JSON in Resources/{RESOURCE_PATH}/{asset.name}.");
                    }

                    return localization;
                })
                .ToArray();

            return new LocalizationData
            {
                languages = data
                    .Where(localization => localization.languages != null)
                    .SelectMany(localization => localization.languages)
                    .Distinct()
                    .ToArray(),
                table = data
                    .SelectMany(localization => localization.table)
                    .ToArray()
            };
        }
    }
}
