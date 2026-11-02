# Conventions des assets Unity

[Retour au sommaire de l'architecture](../../Architecture.md)

## ScriptableObject

Les `ScriptableObject` servent à définir des données réutilisables et
éditables dans l'Inspector, par exemple les définitions de personnages,
d'objets ou les configurations de systèmes. Ils ne remplacent ni les
composants de scène (`MonoBehaviour`), ni les services responsables du
comportement à l'exécution.

- Placer les types de données partagées dans le domaine approprié ; les
  types génériques de données peuvent aller dans `Scripts/Data`, associé
  à l'assembly `Data`.
- Donner à chaque type un nom explicite correspondant à sa
  responsabilité, et ajouter `[CreateAssetMenu]` avec un chemin de menu
  regroupé par domaine.
- Garder les champs sérialisés privés et les exposer en lecture seule
  aux systèmes consommateurs lorsque nécessaire.
- Utiliser les références sérialisées pour relier les assets aux
  composants ou à d'autres assets. Éviter les recherches globales
  implicites qui masquent les dépendances.
- Traiter les assets partagés comme des définitions de données :
  éviter d'y stocker l'état temporaire d'une partie ou de modifier
  leurs champs à l'exécution, car toutes les références au même asset
  observent la même instance.
- Limiter `OnValidate` à la validation ou à la normalisation des données
  dans l'éditeur ; ne pas y déclencher de logique de jeu.

Exemple :

```csharp
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

## Prefabs

Les prefabs sont des modèles réutilisables de `GameObject` et de leurs
composants. Ils conviennent aux entités et éléments de présentation qui
doivent être instanciés ou configurés de manière cohérente ; ils ne
remplacent pas les `ScriptableObject` pour les données partagées.

- Ranger les prefabs du projet sous `Assets/_Project/Prefabs`, puis les
  classer par domaine fonctionnel (par exemple `Characters`, `Combat`
  ou `UI`).
- Donner à chaque prefab un nom explicite lié à son rôle. Éviter les
  noms génériques comme `New Prefab`.
- Garder à la racine du prefab le composant ou l'objet qui représente
  son rôle principal ; organiser les éléments visuels et auxiliaires
  comme enfants lorsque cela clarifie la hiérarchie.
- Préférer des composants cohésifs et réutilisables. Éviter de faire
  dépendre un prefab de références de scène qui ne sont pas garanties
  lors de son instanciation.
- Assigner les dépendances connues via l'Inspector ou les fournir
  explicitement à l'initialisation ; éviter les recherches globales
  implicites.
- Instancier et détruire les prefabs depuis le système responsable de
  leur cycle de vie (par exemple un spawner ou une factory), et non
  depuis des composants sans responsabilité claire.
- Utiliser les variantes ou les prefabs imbriqués lorsqu'ils évitent une
  duplication réelle ; garder leur hiérarchie simple et vérifier les
  overrides avant de les appliquer au prefab source.
- Placer les réglages réutilisables et indépendants de la hiérarchie des
  `GameObject` dans des `ScriptableObject`, plutôt que de dupliquer ces
  valeurs dans plusieurs prefabs.
