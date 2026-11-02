# Organisation du projet

[Retour au sommaire de l'architecture](../../Architecture.md)

Les scripts du projet sont regroupés par domaine fonctionnel plutôt que
par type technique uniquement.

```text
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

## Fichiers et classes

- Un fichier contient normalement une classe publique principale.
- Le nom du fichier correspond exactement au nom de cette classe.
- Les classes et composants ne sont pas placés dans `Core` simplement
  parce qu'ils sont utilisés à plusieurs endroits : ce dossier est
  réservé aux mécanismes fondamentaux de l'architecture.
- Les namespaces suivent les domaines fonctionnels du projet.

Exemple :

```csharp
namespace MyGame.Combat
{
    public class CombatSystem
    {
    }
}
```

## Assemblies

Les fichiers `.asmdef` découpent `Assets/_Project/Scripts` en assemblies
Unity. Un script appartient à l'assembly défini dans son dossier ou dans
le dossier parent le plus proche qui contient un `.asmdef`.

Assemblies actuellement déclarés :

| Assembly | Dossier | Références de projet déclarées |
|---|---|---|
| `Core` | `Scripts/Core` | `Helper` |
| `Helper` | `Scripts/Helper` | Aucune |
| `Data` | `Scripts/Data` | Aucune |
| `Gameplay` | `Scripts/Gameplay` | `Helper` |
| `Presentation` | `Scripts/Presentation` | `Helper` |

Règles pour les dépendances entre assemblies :

- Ajouter une référence explicite dans le `.asmdef` lorsqu'un assembly
  utilise des types d'un autre assembly.
- Garder les dépendances orientées et sans cycle ; ne pas créer de
  référence circulaire pour contourner une séparation de responsabilités.
- Placer chaque script dans l'assembly de son domaine fonctionnel,
  plutôt que de regrouper tous les scripts dans `Core`.
