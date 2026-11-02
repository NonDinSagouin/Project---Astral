# 🌐 Localisation

[Retour au sommaire de l'architecture](../../Architecture.md)

Le système de localisation charge les fichiers JSON placés dans
`Assets/_Project/Resources/Localization`. `LocalizationManager`, un
singleton (`Core.Manager`), les combine à son initialisation et permet
de rechercher une traduction par clé et par langue. Il doit donc être
présent dans une scène chargée avant l'utilisation des traductions.

## Fichiers de traduction

- Regrouper les traductions dans des fichiers JSON du dossier
  `Resources/Localization`.
- Respecter la structure `languages` et `table` attendue par
  `LocalizationData`.
- Utiliser des clés descriptives et stables, organisées par domaine avec
  des segments séparés par des points, par exemple
  `menu.button.start`.
- Utiliser dans `languages` et dans les entrées `values` les noms des
  valeurs de l'enum `Language` (`En`, `Fr`). Les identifiants sont
  comparés tels quels : `En` et `en` sont différents. Ajouter une langue
  implique d'ajouter une valeur à l'enum `Language`.
- Fournir une valeur pour chaque langue prise en charge et chaque clé.
  Garder les clés de traduction uniques entre tous les fichiers chargés.
  Une clé manquante ou une traduction vide n'a pas de valeur de repli.

Exemple :

```json
{
  "languages": ["En", "Fr"],
  "table": [
    {
      "key": "menu.button.start",
      "values": [
        { "key": "En", "value": "Start" },
        { "key": "Fr", "value": "Commencer" }
      ]
    }
  ]
}
```

## Traduire un texte d'interface

Ajouter `TextTranslator` au même GameObject qu'un composant `Text` ou
`TextMeshProUGUI`, puis renseigner la clé de traduction dans l'Inspector.
Lorsque `startRefresh` est activé, le composant récupère la clé depuis le
texte initial de l'élément à son démarrage et affiche sa traduction.
Pour une configuration explicite, désactiver cette option et renseigner
la clé et le composant texte dans l'Inspector.

`LocalizationManager.Instance.TryGet(key, out value)` utilise la langue
courante par défaut. Utiliser son paramètre `language` pour rechercher
explicitement une autre langue.

## Changement de langue

Changer de langue uniquement via `LocalizationManager.SwitchLanguage`.
La méthode met à jour la langue courante puis publie
`OnLanguageChangeEvent` pour que les textes abonnés se rafraîchissent.
Ne pas publier cet événement manuellement pour changer de langue.

```csharp
using Core.Manager;

LocalizationManager.Instance.SwitchLanguage(Language.Fr);
```

Si une clé ou sa traduction est introuvable, `TryGet` retourne `false` ;
`TextTranslator` signale cette erreur au lieu d'afficher silencieusement
une valeur de repli.
