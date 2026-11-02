/// <summary>
/// Contient les classes de données pour la localisation.
/// Définit les structures de données utilisées.
/// </summary>
namespace Core.Localization
{
    /// <summary>
    /// Représente une valeur de traduction pour une langue spécifique.
    /// </summary>
    [System.Serializable]
    public class LocalizationValue
    {
        public string key;
        public string value;
    }

    /// <summary>
    /// Représente une correspondance de traduction pour une clé spécifique.
    /// Contient les valeurs de traduction pour différentes langues.
    /// </summary>
    [System.Serializable]
    public class LocalizationMapping
    {
        public string key;
        public LocalizationValue[] values;
    }

    /// <summary>
    /// Contient les données de localisation complètes.
    /// Inclut la liste des langues disponibles et la table de correspondances de traduction.
    /// </summary>
    [System.Serializable]
    public class LocalizationData
    {
        public string[] languages;
        public LocalizationMapping[] table;
    }
}