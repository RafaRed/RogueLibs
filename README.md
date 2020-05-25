## [Contents](https://github.com/Abbysssal/RogueLibs) ##

1. **Main page**
2. [RogueLibs](https://github.com/Abbysssal/RogueLibs/blob/master/RogueLibs.md)
3. [Mutators](https://github.com/Abbysssal/RogueLibs/blob/master/Mutators.md)
4. [CustomNames](https://github.com/Abbysssal/RogueLibs/blob/master/CustomNames.md)
5. [Extras](https://github.com/Abbysssal/RogueLibs/blob/master/Extras.md)
6. [RogueLibs Changelog](https://github.com/Abbysssal/RogueLibs/blob/master/Changelog.md)

## Links ##
*  [RogueLibs on GitHub](https://github.com/Abbysssal/RogueLibs)
*  [Steam guide on modding with BepInEx](https://steamcommunity.com/sharedfiles/filedetails/?id=2106187116)

## Mods using RogueLibs ##
*  Custom Loader - you can find this mod in #modding-gallery at [official Streets of Rogue Discord server](https://discord.com/invite/streetsofrogue)

# RogueLibs v1.1 #
This modding library allows you to easily add custom mutators and localization lines, plus it has some extra functions that you might need.

## How to use RogueLibs in your mods ##
You can find instructions on how to do that here (9. Modding Libraries, RogueLibs):
https://steamcommunity.com/sharedfiles/filedetails/?id=2106187116

## Plugin Example ##
```cs
using System;
using BepInEx;
using RogueLibsCore;

namespace RocketBulletsMutator
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInDependency(RogueLibs.pluginGuid, "1.1")]
    public class RocketBulletsMutator : BaseUnityPlugin
    {
        public const string pluginGuid = "abbysssal.streetsofrogue.rocketbulletsmutator";
        public const string pluginName = "Rocket Bullets Mutators";
        public const string pluginVersion = "1.0";

        public static CustomMutator RocketBulletsCommon { get; set; }

        protected void Awake()
        {
            RocketBulletsCommon = RogueLibs.SetMutator("RocketBulletsCommon", true,
                new CustomNameInfo("Rocket Bullets (Common weapons)",
                                    null, null, null, null,
                                    "Ракетные пули (Простое оружие)",
                                    null, null),
                new CustomNameInfo("Replaces common bullets (Pistol, Shotgun, Machinegun, etc.) with rockets. Rate of fire is unchanged.",
                                    null, null, null, null,
                                    "Заменяет простые пули (Пистолет, Дробовик, Автомат и т.п.) на ракеты. Скорость стрельбы не изменена.",
                                    null, null));
            RocketBulletsCommon.AddConflicting("RocketLaunchers", "NoGuns");

            this.PatchPrefix(typeof(Gun), "spawnBullet", GetType(), "Gun_spawnBullet", new Type[] { typeof(bulletStatus), typeof(InvItem), typeof(int), typeof(bool), typeof(string) });
        }
        protected static void Gun_spawnBullet(ref bulletStatus bulletType)
        {
            if (RocketBulletsCommon.IsActive && (bulletType == bulletStatus.Normal || bulletType == bulletStatus.Shotgun || bulletType == bulletStatus.Revolver))
                bulletType = bulletStatus.Rocket;
        }
    }
}
```
