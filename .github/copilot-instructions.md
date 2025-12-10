# CaveBiome (Continued) GitHub Copilot Instructions

## Mod Overview and Purpose
The CaveBiome mod introduces an exciting new biome to RimWorld called the "Cave". This mod allows players to delve into deep tunnel systems, providing a unique environment for exploration and settlement on the Rimworld. It continues the legacy of Rikiki's original mod by adding new functionalities and maintaining compatibility with the game's updates. Notably, the mod requires the Caveworld Flora mod to be enabled first for full functionality.

## Key Features and Systems
- **Cave Biome Generation**: Adds the cave biome to the world generation, which is integrated with DeepRim and Z-Levels mods.
- **Environmental Dynamics**: Implements adjustments to ensure electric objects do not short-circuit in rain unless placed improperly in caves.
- **French Translation**: Includes full French language support, enhancing accessibility.
- **Integration with Vanilla Fishing Expanded**: Seamlessly integrates with the Vanilla Fishing Expanded mod through specific patches.
- **Map Components and Conditions**: Utilizes map components like `MapComponent_CaveWellLight` to manage cave-specific phenomena.

## Coding Patterns and Conventions
The code primarily follows standard C# conventions with customization suited for RimWorld modding:
- Use of meaningful and descriptive class and method names, focusing on the functionality such as `BiomeWorker_Cave` or `IncidentWorker_MeteoriteImpactCave`.
- Encapsulation through private methods where appropriate, such as `TryFindDropPodSpot`.
- Static utility classes like `Util_CaveBiome` for shared functionalities.
- Consistent use of namespaces and file organization reflecting the mod's structure.

## XML Integration
The mod uses XML for data-driven development, typical in RimWorld modding:
- XML files control biome characteristics, plants, and atmospheric elements to tailor the cave environment.
- Mod metadata is defined in XML, ensuring compatibility and proper loading order in RimWorld's mod system.

## Harmony Patching
Harmony is leveraged for runtime patching, enabling modifications to core game functionality without altering original code:
- `DeepRimPatch` class showcases the use of Harmony to integrate features with other mods.
- Use Harmony annotations like `[HarmonyPatch]` to target specific game methods needing alteration.

## Suggestions for Copilot
To enhance productivity and maintain code quality in this mod project, consider the following suggestions for using GitHub Copilot:
- **Refactor Common Logic**: Use Copilot to suggest and refactor repeated code snippets into shared methods, especially when working with similar incident or game condition classes.
- **Autocomplete XML Elements**: Leverage Copilot's suggestions for quickly completing XML configurations, ensuring consistency across data files.
- **Inline Documentation**: Utilize Copilot to generate and maintain inline documentation, explaining non-trivial code logic for future maintainability.
- **Test and Validate Harmony Patches**: Prompt Copilot to create test cases or example scenarios to validate the Harmony patches applied, ensuring compatibility with updates and other mods.

By adhering to these guidelines, Copilot can significantly streamline the development process and support the integration of new features into the CaveBiome mod.
