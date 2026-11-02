# Conventions de code

[Retour au sommaire de l'architecture](../../Architecture.md)

## Langue et nommage

Le code technique est écrit en anglais : classes, méthodes, propriétés,
variables, événements, namespaces et commentaires techniques.

- Ne pas utiliser de préfixes hongrois ou de préfixes de champ tels que
  `m_`.
- Préférer les noms explicites aux abréviations non évidentes.
- Les méthodes expriment généralement une action : `StartCombat()`,
  `TakeDamage()`.
- Les méthodes retournant un booléen expriment une condition :
  `CanAttack()`, `IsAlive()`.
- Les booléens utilisent des formulations positives : `isPaused`,
  `hasTarget`, `canMove`.
- Conserver exactement la casse et les noms des méthodes de cycle de vie
  Unity : `Awake`, `Start`, `Update`, `OnEnable`, `OnDisable`,
  `OnDestroy`, etc.
- Les champs destinés à l'Inspector restent privés et utilisent
  `[SerializeField]`, sauf raison documentée de les exposer publiquement.
- Lorsqu'un champ sérialisé est renommé, utiliser
  `[FormerlySerializedAs]` si nécessaire pour préserver les données des
  scènes et prefabs.

## Responsabilités des composants

Les suffixes de classes sont des indications de responsabilité, pas
des obligations automatiques.

| Suffixe | Responsabilité typique |
|---|---|
| `Controller` | Contrôle d'un acteur ou d'un élément interactif |
| `View` | Présentation et affichage |
| `Manager` | Coordination d'un ensemble de ressources ou de fonctionnalités |
| `Service` | Fonctionnalité partagée par plusieurs systèmes |
| `System` | Logique d'un domaine de jeu |
| `Factory` | Création d'objets |
| `Spawner` | Apparition d'entités |
