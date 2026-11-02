# 🌐 Localisation

[Retour au sommaire de l'architecture](../../Architecture.md)

Le système de localisation charge les fichiers JSON placés dans
`Assets/_Project/Resources/Localization`. `StaticLocalization` les
combine au démarrage et permet de rechercher une traduction par clé et
par langue.

## Fichiers de traduction

- Regrouper les traductions dans des fichiers JSON du dossier
  `Resources/Localization`.
- Respecter la structure `languages` et `table` attendue par
  `LocalizationData`.
- Utiliser des clés descriptives et stables, organisées par domaine avec
  des segments séparés par des points, par exemple
  `menu.button.start`.
- Déclarer les identifiants de langue de façon cohérente dans
  `languages` et dans les entrées `values`. Les identifiants sont
  comparés tels quels : `En` et `en` sont différents.
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

`StaticLocalization.TryGet(key, out value)` utilise la langue courante
par défaut. Utiliser son paramètre `language` pour rechercher
explicitement une autre langue.

## Changement de langue

`OnLanguageChangeEvent` signale un changement ; l'événement ne modifie
pas lui-même la langue. Après avoir mis à jour `CURRENT_LANGUAGE`,
publier l'événement pour que les textes abonnés se rafraîchissent :

```csharp
using Core;
using Core.Event;
using Core.Localization;

StaticLocalization.CURRENT_LANGUAGE = "Fr";
EventBus.Publish(new OnLanguageChangeEvent());
```

Utiliser exactement un identifiant présent dans les fichiers de
traduction. Si une clé ou sa traduction est introuvable, `TryGet`
retourne `false` ; `TextTranslator` signale cette erreur au lieu
d'afficher silencieusement une valeur de repli.
