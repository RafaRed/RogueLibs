## Contents ##

1. [Main page](https://github.com/Abbysssal/RogueLibs)
2. [RogueLibs](./RogueLibs.md)
3. **CustomNames**
4. [CustomMutators](./CustomMutators.md)
5. [CustomItems](./CustomItems.md)
6. [CustomAbilities](./CustomAbilities.md)
7. [Extras](./Extras.md)
8. [RogueLibs Changelog](./Changelog.md)
9. [Mod Examples](./Examples.md)

## Creating CustomNames ##
```cs
public static CustomName SetCustomName(string id, string type, CustomNameInfo info);
```
```cs
CustomName name = RogueLibs.SetCustomName("myId", "Agent", new CustomNameInfo(
        "english",
        null, // Chinese
        null, // German
        null, // Spanish
        null, // Brazilian
        null, // Russian
        null, // French
        null)); // Korean
// null strings will default to english.

// If you don't know which type you need, use null.
```
You can also get CustomName by its id and type:
```cs
public static CustomName GetCustomName(string id, string type);
```
```cs
CustomName found = RogueLibs.GetCustomName("MyNameId", "MyType");
```
## Deleting CustomNames ##
Just in case you don't need your CustomNames anymore:
```cs
public static bool DeleteCustomName(CustomName customName);
public static bool DeleteCustomName(string id, string type);
```
## Using CustomNames ##
You can get your CustomName's Id and Type:
```cs
public string Id { get; }
public string Type { get; }
```
```cs
string myId = myCustomName.Id;
string myType = myCustomName.Type;
```
You can access different localization strings:
```cs
public string English { get; set; }
public string SChinese { get; set; }
public string German { get; set; }
public string Spanish { get; set; }
public string Brazilian { get; set; }
public string Russian { get; set; }
public string French { get; set; }
public string KoreanA { get; set; }
```
```cs
string englishString = myCustomName.English;

myCustomName.English = "newName";
```
Or you can access the translations array itself:
```cs
public string[] Translations { get; set; }
```
```cs
string randomString = myCustomName.Translations[Random.Next(0, 8)];
myCustomName.Translations[3] = randomString;
```















