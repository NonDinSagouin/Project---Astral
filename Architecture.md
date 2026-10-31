## Architecture du projet Unity
`Statut` : conventions de référence
`Périmètre` : architecture générale, nommage, services persistants, singletons et événements
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

## 5. Règles d'utilisation des singletons
#### 5.1 Principe

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

#### 5.2 Classe de base commune

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

## 6. Règles d'utilisation des événements
à faire