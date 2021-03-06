## Contents ##

1. [Main page](https://github.com/Abbysssal/RogueLibs)
2. **RogueLibs**
3. [CustomNames](./CustomNames.md)
4. [CustomMutators](./CustomMutators.md)
5. [CustomItems](./CustomItems.md)
6. [CustomAbilities](./CustomAbilities.md)
7. [Extras](./Extras.md)
8. [RogueLibs Changelog](./Changelog.md)
9. [Mod Examples](./Examples.md)

## Faster Patches ##
You can replace this part of your code:
```cs
Harmony harmony = new Harmony(pluginGuid);
MethodInfo original = AccessTools.Method(typeof(OriginalClass), "OriginalMethod");
MethodInfo patch = AccessTools.Method(typeof(MyPatches), "PatchMethod");
harmony.Patch(original, patch);
```
With this:
```cs
this.PatchPrefix(typeof(OriginalClass), "OriginalMethod", typeof(MyPatches), "PatchMethod");
```
In case of postfixes, use:
```cs
this.PatchPostfix(typeof(OriginalClass), "OriginalMethod", typeof(MyPatches), "PatchMethod");
```
These methods take much less space, and most importantly they catch and log all exceptions that can happen during patching. If the patch was successful, methods return true, otherwise - false.
## RogueLibs ##
You can access RogueLibs' GUID, Name and Version:
```cs
public const string pluginGuid = "abbysssal.streetsofrogue.roguelibs";
public const string pluginName = "RogueLibs";
public const string pluginVersion = "1.3.1";
```
You can use pluginGuid in your BepInDependency attribute. But DO NOT use pluginVersion in it! Otherwise, BepInEx will think that your mod requires the version of RogueLibs that the user has installed.

You can get the RogueLibs instance that is being used right now and its Mutators, Items and Names:
```cs
public static RogueLibs Instance { get; internal set; }
public List<CustomMutator> Mutators { get; set; }
public List<CustomItem> Items { get; set; }
public List<CustomName> Names { get; set; }
```
```cs
RogueLibs myLibs = RogueLibs.Instance;
List<CustomMutator> myMutators = myLibs.Mutators;
List<CustomName> myNames = myLibs.Names;
```
## Some Features ##
You can get mouse cursor's position in-game:
```cs
public static Vector2 MouseIngamePosition();
```
And you can spawn Agents (NPCs) and Items:
```cs
public static Agent SpawnAgent(Vector2 position, string agentId);
public static Item SpawnItem(Vector2 position, string itemId, int amount);
```
```cs
Vector2 cursorPos = RogueLibs.MouseIngamePosition();
Agent spawnedAgent = RogueLibs.SpawnAgent(cursorPos, "Assassin");
Item spawnedItem = RogueLibs.SpawnItem(cursorPos, "Axe", 5000);
```














