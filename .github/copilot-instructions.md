# GitHub Copilot Instructions for CaveBiome (Continued)

## Mod Overview and Purpose
CaveBiome (Continued) is a mod for RimWorld that adds a new underground biome called "cave". This mod allows players to explore and settle deep tunnels, providing a unique experience with new flora, fauna, and settlement opportunities. The mod is an update to the original CaveBiome mod by Rikiki, with contributions from the community, including translations and bug fixes.

## Key Features and Systems
- **New Biome**: Adds a new cave biome to RimWorld, allowing for deep underground exploration and settlement.
- **Flora and Fauna**: Introduces unique cave flora and fauna that thrive in the underground environment.
- **Building Features**: Includes new buildings and features specific to the cave biome, such as corpse generators and specialized structures.
- **Patches**: Applies patches to vanilla RimWorld features to improve compatibility and functionality, such as handling rain and fishing mechanics.

## Coding Patterns and Conventions
- **Mod Structure**: The mod follows a standard RimWorld mod structure, with C# source files for logic and XML files for definitions.
- **Namespace**: All C# files are under the `CaveBiome` namespace.
- **Class Naming**: C# class names are descriptive and follow PascalCase conventions (e.g., `Building_AnimalCorpsesGenerator`).
- **Method Naming**: Methods are named descriptively and follow camelCase conventions (e.g., `generateAnimalCorpses`).
- **XML Defs**: XML definitions follow RimWorld's standard naming conventions and are organized under relevant categories like `BiomeDefs`, `ThingDefs`, and `MapConditionDefs`.

## XML Integration
The mod extensively uses XML definitions to define various aspects of the cave biome, including:
- **BiomeDefs**: Defines the cave biome and its properties.
- **ThingDefs**: Defines new things specific to the cave biome, such as flora and specialized buildings.
- **MapConditionDefs**: Defines map conditions unique to the cave biome.
- **GenStepDefs**: Defines generation steps for cave-specific features during map generation.

Example XML snippet from `Biomes_Cave.xml`:
xml
<Defs>
  <BiomeDef>
    <defName>Cave</defName>
    <label>Cave</label>
    <description>A dark and mysterious underground biome.</description>
    <temperatureOffset>0</temperatureOffset>
    <rainfall>0</rainfall>
    <wildAnimals>...</wildAnimals>
    <wildPlants>...</wildPlants>
  </BiomeDef>
</Defs>


## Harmony Patching
The mod uses Harmony patching to modify vanilla RimWorld behavior, ensuring compatibility and adding new features. Key patches include:
- **Rain Handling**: Prevents short-circuiting of electrical devices in the rain unless they are built in a cave-well.
- **Fishing Mechanics**: Patches Vanilla Fishing Expanded to ensure compatibility with the cave biome.

Example Harmony patch from `CaveBiome.cs`:
csharp
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaveBiome
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.ExposeData))]
    public static class Thing_ExposeData_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Thing __instance)
        {
            if (__instance is Building building && building.def == ThingDefOf.PowerConduit)
            {
                building.ShortCircuitInRain = false;
            }
        }
    }
}


## Suggestions for Copilot
To ensure that Copilot provides the most relevant and accurate suggestions for CaveBiome (Continued), consider the following guidelines:

1. **Contextual Suggestions**: When working with XML definitions, Copilot should suggest relevant XML tags and attributes based on the RimWorld modding schema.
2. **Code Completion**: For C# code, Copilot should suggest methods and classes that are commonly used in RimWorld modding, such as building generators, map generation steps, and harmony patches.
3. **Documentation and Examples**: Copilot should provide documentation and examples for XML definitions and C# code, including descriptions of fields, methods, and their purposes.
4. **Error Handling**: When generating code or XML, Copilot should suggest error handling strategies, such as checking for null values or ensuring compatibility with other mods.

By following these guidelines, Copilot can be a valuable tool for developers working on CaveBiome (Continued) and other RimWorld mods.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

