using System;
using System.Linq;
using UnityEngine;

namespace Core.Localization
{ 
    /// <summary>
    /// Fournit un accès statique aux traductions localisées.
    /// </summary>
    public static class StaticLocalization
    {
        /// <summary>
        /// Fournit la langue actuelle utilisée pour la localisation.
        /// </summary>
        public static string CURRENT_LANGUAGE = "En";
        
        private static readonly LocalizationData Translations = Load();
        private const string RESOURCE_PATH = "Localization";

        private static LocalizationData Load()
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

        /// <summary>
        /// Essaie de récupérer la traduction pour une clé donnée dans la langue spécifiée.
        /// </summary>
        /// <param name="key">La clé de la traduction.</param>
        /// <param name="value">La valeur traduite correspondante si trouvée.</param>
        /// <param name="language">La langue dans laquelle récupérer la traduction. Si null, utilise la langue actuelle.</param>
        /// <returns>True si la traduction est trouvée, sinon false.</returns>
        public static bool TryGet(string key, out string value, string language = null)
        {
            language ??= CURRENT_LANGUAGE;
            var mapping = Translations.table.FirstOrDefault(m => m.key == key);
            var entry = mapping?.values.FirstOrDefault(v => v.key == language);
            value = entry?.value;
            return !string.IsNullOrEmpty(value);
        }
    }
}
