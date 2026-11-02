# ⚙️ Services et événements

[Retour au sommaire de l'architecture](../../Architecture.md)

## Singletons

### Principe

Le singleton est une exception contrôlée, utilisée lorsqu'une instance
unique et un accès partagé sont réellement nécessaires.

Avant de créer un singleton, préférer dans cet ordre lorsque c'est
adapté :

1. une référence sérialisée entre composants connus ;
2. une dépendance fournie explicitement par l'architecture ;
3. une référence via un contexte ou un système de scène ;
4. un singleton pour un service véritablement global.

Les entités pouvant exister en plusieurs exemplaires
(`PlayerController`, `EnemyController`, `HealthBarView`, etc.) ne sont
pas des singletons.

### Classe de base commune

Tout singleton du projet doit hériter de `Core.Singleton<T>` et ne doit
pas réimplémenter son propre mécanisme d'instance.

La classe de base centralise :

- la propriété `Instance` ;
- la détection des doublons ;
- l'enregistrement et la libération de l'instance ;
- la persistance entre les changements de scène ;
- un point d'extension pour l'initialisation propre à la classe dérivée.

Exemple de classe dérivée :

```csharp
using Core;
using UnityEngine;

public class SettingsManager : Singleton<SettingsManager>
{
    protected override void OnSingletonAwake()
    {
        // Initialize settings.
    }
}
```

## Événements

Les événements servent à signaler un fait déjà survenu et à découpler
l'émetteur des systèmes qui souhaitent réagir. Utiliser `EventBus` pour
diffuser ces notifications typées, plutôt que de faire dépendre
l'émetteur directement de ses abonnés.

- Définir un type dédié par événement, sous forme de `readonly struct`,
  dans `Core.Event`. Son nom décrit le fait signalé et se termine par
  `Event` (par exemple `OnLanguageChangeEvent`).
- Ajouter à l'événement uniquement les données nécessaires aux abonnés.
  Garder ses données immuables et éviter d'y placer des références à des
  composants ou à des services.
- Publier l'événement après avoir mis à jour l'état concerné. La
  publication est synchrone : les abonnés sont appelés immédiatement
  pendant l'appel à `EventBus.Publish`.
- S'abonner lorsque le composant devient actif, puis se désabonner
  lorsqu'il est désactivé ou détruit. Utiliser la même méthode de rappel
  pour les deux opérations afin d'éviter les abonnements en double et
  les callbacks vers des composants inactifs.
- Réserver les événements aux notifications sans réponse attendue. Pour
  une opération qui doit retourner un résultat, appeler une méthode ou
  utiliser un service adapté.

Exemple d'abonnement :

```csharp
using Core;
using Core.Event;
using UnityEngine;

public class LanguageAwareComponent : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus.Subscribe<OnLanguageChangeEvent>(OnLanguageChange);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<OnLanguageChangeEvent>(OnLanguageChange);
    }

    private void OnLanguageChange(OnLanguageChangeEvent evt)
    {
        // Refresh language-dependent state.
    }
}
```

Exemple de publication, une fois la nouvelle langue appliquée :

```csharp
EventBus.Publish(new OnLanguageChangeEvent());
```

## Gestion des informations de la souris

`Core.Manager.MouseManager` est un singleton qui lit la souris via le
nouveau Input System (`Mouse.current`) à chaque `Update`. S'il n'y a pas
de souris active, la mise à jour est ignorée.

Le gestionnaire récupère la position de la souris en coordonnées écran,
puis la convertit avec `Camera.main.ScreenToWorldPoint`. La coordonnée
`z` du résultat est forcée à `0` pour obtenir une position sur le plan
XY du monde. Les deux valeurs sont actuellement stockées dans des champs
sérialisés privés : elles sont consultables dans l'inspecteur, mais ne
sont pas exposées comme propriétés publiques.

La visibilité du curseur est réglée avec le champ `showCursor` de
l'inspecteur (`true` pour l'afficher). Lorsque `debugMode` est activé,
`MouseManager` journalise les positions écran et monde.
