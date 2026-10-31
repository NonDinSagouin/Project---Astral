## Architecture du projet Unity
`Statut` : conventions de référence
`Périmètre` : architecture générale, nommage, assemblies, ScriptableObjects, services persistants, singletons et événements
`Langue du code` : anglais

## 1. Objectifs

Ce document définit les règles communes à appliquer lors du
développement du projet afin de conserver une architecture cohérente,
lisible, testable et facile à faire évoluer.

## 2. Organisation du projet
Les scripts du projet sont regroupés par domaine fonctionnel plutôt que
par type technique uniquement.

``` text
Assets/
└── _Project/
    ├── Scenes/
    │   ├── Bootstrap.unity
    │   ├── Persistent.unity
    │   └── ...
    ├── Scripts/
    │   ├── Core/
    │   │   ├── Singleton.cs
    │   │   ├── GameBootstrapper.cs
    │   │   └── SceneLoader.cs
    │   ├── Services/
    │   │   ├── SettingsManager.cs
    │   │   ├── AudioManager.cs
    │   │   └── SaveManager.cs
    │   ├── Combat/
    │   ├── Characters/
    │   ├── UI/
    │   └── ...
    ├── Prefabs/
    ├── ScriptableObjects/
    └── Art/
```

#### 2.1 Fichiers et classes

-   Un fichier contient normalement une classe publique principale.
-   Le nom du fichier correspond exactement au nom de cette classe.
-   Les classes et composants ne sont pas placés dans `Core` simplement
    parce qu'ils sont utilisés à plusieurs endroits : ce dossier est
    réservé aux mécanismes fondamentaux de l'architecture.
-   Les namespaces suivent les domaines fonctionnels du projet.

Exemple :

``` csharp
namespace MyGame.Combat
{
    public class CombatSystem
    {
    }
}
```

#### 2.2 Assemblies

Les fichiers `.asmdef` découpent `Assets/_Project/Scripts` en assemblies Unity. Un script appartient à l'assembly défini dans son dossier ou dans le dossier parent le plus proche qui contient un `.asmdef`.

Assemblies actuellement déclarés :

| Assembly | Dossier | Références de projet déclarées |
|---|---|---|
| `Core` | `Scripts/Core` | `Helper` |
| `Helper` | `Scripts/Helper` | Aucune |
| `Data` | `Scripts/Data` | Aucune |
| `Gameplay` | `Scripts/Gameplay` | `Helper` |
| `Presentation` | `Scripts/Presentation` | `Helper` |

Règles pour les dépendances entre assemblies :

- Ajouter une référence explicite dans le `.asmdef` lorsqu'un assembly utilise des types d'un autre assembly.
- Garder les dépendances orientées et sans cycle ; ne pas créer de référence circulaire pour contourner une séparation de responsabilités.
- Placer chaque script dans l'assembly de son domaine fonctionnel, plutôt que de regrouper tous les scripts dans `Core`.

## 3. Conventions de nommage

Le code technique est écrit en anglais : classes, méthodes, propriétés,
variables, événements, namespaces et commentaires techniques.

Tableau

Règles complémentaires :

-   Ne pas utiliser de préfixes hongrois ou de préfixes de champ tels
    que `m_`.
-   Préférer les noms explicites aux abréviations non évidentes.
-   Les méthodes expriment généralement une action : `StartCombat()`,
    `TakeDamage()`.
-   Les méthodes retournant un booléen expriment une condition :
    `CanAttack()`, `IsAlive()`.
-   Les booléens utilisent des formulations positives : `isPaused`,
    `hasTarget`, `canMove`.
-   Conserver exactement la casse et les noms des méthodes de cycle de
    vie Unity : `Awake`, `Start`, `Update`, `OnEnable`, `OnDisable`,
    `OnDestroy`, etc.
-   Les champs destinés à l'Inspector restent privés et utilisent
    `[SerializeField]` sauf raison documentée de les exposer
    publiquement.
-   Lorsqu'un champ sérialisé est renommé, utiliser
    `[FormerlySerializedAs]` si nécessaire pour préserver les données
    des scènes et prefabs.

## 4. Responsabilités des composants

Les suffixes de classes sont des indications de responsabilité, pas des
obligations automatiques.

  -----------------------------------------------------------------------
  Suffixe                             Responsabilité typique
  ----------------------------------- -----------------------------------

  `Controller`                        Contrôle d'un acteur ou d'un élément interactif
  `View`                              Présentation et affichage
  `Manager`                           Coordination d'un ensemble de ressources ou de fonctionnalités
  `Service`                           Fonctionnalité partagée par plusieurs systèmes
  `System`                            Logique d'un domaine de jeu
  `Factory`                           Création d'objets
  `Spawner`                           Apparition d'entités

  -----------------------------------------------------------------------

## 5. Conventions d'utilisation des `ScriptableObject`

Les `ScriptableObject` servent à définir des données réutilisables et
éditables dans l'Inspector, par exemple les définitions de personnages,
d'objets ou les configurations de systèmes. Ils ne remplacent ni les
composants de scène (`MonoBehaviour`), ni les services responsables du
comportement à l'exécution.

-   Placer les types de données partagées dans le domaine approprié ;
    les types génériques de données peuvent aller dans `Scripts/Data`,
    associé à l'assembly `Data`.
-   Donner à chaque type un nom explicite correspondant à sa
    responsabilité, et ajouter `[CreateAssetMenu]` avec un chemin de
    menu regroupé par domaine.
-   Garder les champs sérialisés privés et les exposer en lecture seule
    aux systèmes consommateurs lorsque nécessaire.
-   Utiliser les références sérialisées pour relier les assets aux
    composants ou à d'autres assets. Éviter les recherches globales
    implicites qui masquent les dépendances.
-   Traiter les assets partagés comme des définitions de données :
    éviter d'y stocker l'état temporaire d'une partie ou de modifier
    leurs champs à l'exécution, car toutes les références au même asset
    observent la même instance.
-   Limiter `OnValidate` à la validation ou à la normalisation des
    données dans l'éditeur ; ne pas y déclencher de logique de jeu.

Exemple :

``` csharp
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "CharacterDefinition", menuName = "Data/Character Definition")]
    public class CharacterDefinition : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private int maxHealth = 1;

        public string DisplayName => displayName;
        public int MaxHealth => maxHealth;
    }
}
```

## 6. Conventions d'utilisation des prefabs

Les prefabs sont des modèles réutilisables de `GameObject` et de leurs
composants. Ils conviennent aux entités et éléments de présentation qui
doivent être instanciés ou configurés de manière cohérente ; ils ne
remplacent pas les `ScriptableObject` pour les données partagées.

-   Ranger les prefabs du projet sous `Assets/_Project/Prefabs`, puis
    les classer par domaine fonctionnel (par exemple `Characters`,
    `Combat` ou `UI`).
-   Donner à chaque prefab un nom explicite lié à son rôle. Éviter les
    noms génériques comme `New Prefab`.
-   Garder à la racine du prefab le composant ou l'objet qui représente
    son rôle principal ; organiser les éléments visuels et auxiliaires
    comme enfants lorsque cela clarifie la hiérarchie.
-   Préférer des composants cohésifs et réutilisables. Éviter de faire
    dépendre un prefab de références de scène qui ne sont pas garanties
    lors de son instanciation.
-   Assigner les dépendances connues via l'Inspector ou les fournir
    explicitement à l'initialisation ; éviter les recherches globales
    implicites.
-   Instancier et détruire les prefabs depuis le système responsable de
    leur cycle de vie (par exemple un spawner ou une factory), et non
    depuis des composants sans responsabilité claire.
-   Utiliser les variantes ou les prefabs imbriqués lorsqu'ils évitent
    une duplication réelle ; garder leur hiérarchie simple et vérifier
    les overrides avant de les appliquer au prefab source.
-   Placer les réglages réutilisables et indépendants de la hiérarchie
    des `GameObject` dans des `ScriptableObject`, plutôt que de
    dupliquer ces valeurs dans plusieurs prefabs.

## 7. Règles d'utilisation des singletons
#### 7.1 Principe

Le singleton est une exception contrôlée, utilisée lorsqu'une instance
unique et un accès partagé sont réellement nécessaires.

Avant de créer un singleton, préférer dans cet ordre lorsque c'est
adapté :

1.  une référence sérialisée entre composants connus ;
2.  une dépendance fournie explicitement par l'architecture ;
3.  une référence via un contexte ou un système de scène ;
4.  un singleton pour un service véritablement global.

Les entités pouvant exister en plusieurs exemplaires
(`PlayerController`, `EnemyController`, `HealthBarView`, etc.) ne sont
pas des singletons.

#### 7.2 Classe de base commune

Tout singleton du projet doit hériter de `Core.Singleton<T>` et ne doit
pas réimplémenter son propre mécanisme d'instance.

La classe de base centralise :

-   la propriété `Instance` ;
-   la détection des doublons ;
-   l'enregistrement et la libération de l'instance ;
-   la persistance entre les changements de scène ;
-   un point d'extension pour l'initialisation propre à la classe
    dérivée.

Exemple de classe dérivée :

``` csharp
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

## 8. Règles d'utilisation des événements
à faire