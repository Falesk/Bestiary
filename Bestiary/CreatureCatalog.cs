using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Bestiary.BMenu;

namespace Bestiary
{
    public static class CreatureCatalog
    {
        public const int DebugVariantScanMaxData = 10;
        public const float GroupIconSecondsPerVariant = 1f;
        public static readonly HashSet<string> IgnoredCreatureTypes =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "StandardGroundCreature",
                "LizardTemplate",
                "GrappleSnake",
                
            };
        public static readonly List<string> Order = new List<string>
        {
            "Slugcat",
            "SlugNPC",
            "Fly",
            "Overseer",
            "CicadaB",
            "CicadaA",
            "LanternMouse",
            "Yeek",
            "TubeWorm",
            "Rat",
            "Hazer",
            "JetFish",
            "Snail",
            "GreenLizard",
            "PinkLizard",
            "BlueLizard",
            "WhiteLizard",
            "YellowLizard",
            "BlackLizard",
            "CyanLizard",
            "RedLizard",
            "SpitLizard",
            "Salamander",
            "EelLizard",
            "ZoopLizard",
            "TrainLizard",
            "PeachLizard",
            "IndigoLizard",
            "BasiliskLizard",
            "BlizzardLizard",
            "lizards_data1",
            "lizards_data2",
            "SmallNeedleWorm",
            "BigNeedleWorm",
            "EggBug",
            "FireBug",
            "DropBug",
            "StowawayBug",
            "PoleMimic",
            "GarbageWorm",
            "TentaclePlant",
            "Leech",
            "SeaLeech",
            "JungleLeech",
            "VultureGrub",
            "Vulture",
            "KingVulture",
            "MirosVulture",
            "MirosBird",
            "Spider",
            "BigSpider",
            "SpitterSpider",
            "MotherSpider",
            "SmallCentipede",
            "Centipede",
            "RedCentipede",
            "Centiwing",
            "AquaCenti",
            "Angler",
            "Barnacle",
            "Tardigrade",
            "Frog",
            "Scavenger",
            "ScavengerElite",
            "ScavengerKing",
            "ScavengerTemplar",
            "ScavengerDisciple",
            "BoxWorm",
            "FireSprite",
            "MothGrub",
            "SmallMoth",
            "BigMoth",
            "SandGrub",
            "BigSandGrub",
            "BrotherLongLegs",
            "DaddyLongLegs",
            "TerrorLongLegs",
            "HunterDaddy",
            "Inspector",
            "Rattler",
            "RippleSpider",
            "Loach",
            "RotLoach",
            "DrillCrab",
            "TowerCrab",
            "Deer",
            "SkyWhale",
            "SkyWhale_data1",
            "BigEel",
            "BigJelly",
            "TempleGuard",
            "Millipede",
            "",
            // Falesk mods
            "BabyLizard",
            // Kiwi's mods
            "GrandOldDeer",
            "TigerLizard",
            // Drought mod
            "WalkerBeast",
            "GreyLizard",
            "SeaDrake",
            // M4blelous pack
            "WaterBlob",
            "BouncingBall",
            "Hoverfly",
            "Tailfly",
            "SilverLizard",
            "NoodleEater",
            "Polliwog",
            "MoleSalamander",
            "WaterSpitter",
            "HunterSeeker",
            "AlphaOrange",
            "ChipChop",
            "SurfaceSwimmer",
            "TintedBeetle",
            "ThornBug",
            "DivingBeetle",
            "MamaBug",
            "Sporantula",
            "Glowpillar",
            "Killerpillar",
            "MiniScutigera",
            "Scutigera",
            "RedHorrorCenti",
            "MiniBlackLeech",
            "HazerMom",
            "CommonEel",
            "Blizzor",
            "SparkEye",
            "FatFireFly",
            "ScavengerSentinel",
            "Denture",
            "XyloWorm",
            "Xylo",
            "MiniLeviathan",
            "MiniFlyingBigEel",
            "FlyingBigEel",
            
        };

        public static readonly HashSet<string> Hidden = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // "Centipede_data3"
        };
        public static readonly Dictionary<string, string> CustomDisplayNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "SkyWhale_data1", "Lotus Whale" },
                { "HunterDaddy", "Hunter Long Legs" },
                { "BabyLizard", "Baby Lizard" },

                { "GrandOldDeer", "Grand Old Deer" },
                { "TigerLizard", "Tiger Lizard" },
                
                { "WalkerBeast", "Walker Beast" },
                { "GreyLizard", "Grey Lizard" },
                { "SeaDrake", "Sea Drake" },

                { "SurfaceSwimmer", "Surface Swimmer" },
                { "Scutigera", "Scutigera" },
                { "RedHorrorCenti", "Red Horror Centipede" },
                { "MiniLeviathan", "Mini Leviathan" },
                { "FlyingBigEel", "Echo Leviathan" },
                { "MiniFlyingBigEel", "Mini Echo Leviathan" },
                { "FatFirefly", "Fat Firefly" },
                { "BouncingBall", "Bouncing Ball" },
                { "WaterSpitter", "Water Spitter" },
                { "WaterBlob", "Water Blob" },
                { "HunterSeeker", "Hunter Seeker" },
                { "TintedBeetle", "Tinted Beetle" },
                { "Blizzor", "Blizzor" },
                { "MoleSalamander", "Mole Salamander" },
                { "MiniBlackLeech", "Mini Black Leech" },
                { "CommonEel", "Common Eel" },
                { "DivingBeetle", "Diving Beetle" },
                { "ChipChop", "Chip Chop" },
                { "XyloWorm", "Xylo Worm" },
                { "SparkEye", "Spark Eye" },
                { "ScavengerSentinel", "Scavenger Sentinel" },
                { "AlphaOrange", "Alpha Orange Lizard" },
                { "MamaBug", "Mama Bug" },
                { "SilverLizard", "Silver Lizard" },
                { "NoodleEater", "Noodle Eater" },
                { "ThornBug", "Thorn Bug" },
                { "HazerMom", "Hazer Mom" },
                { "Hoverfly", "Hoverfly" },
                { "Tailfly", "Tailfly" },
                { "Polliwog", "Polliwog" },
                { "Killerpillar", "Killerpillar" },
                { "Glowpillar", "Glowpillar" },
                { "Denture", "Denture" },
                { "Xylo", "Xylo" },
                { "dddddddddddddd", "ssssssssssssssss" },
            };
        public static readonly Dictionary<string, int[]> ExtraDebugVariants =
            new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase)
            {
                { "Centipede", new[] { 1,2,3 } }, //cuz centipede have 3 almost the same variants
                { "SkyWhale", new[] { 1 } },
            };
        public static readonly HashSet<string> DependsOnLengthCreatures =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Centipede",
                "RedCentipede",
                "Centiwing",
                "AquaCenti",
                "Scutigera",
                "RedHorrorCenti",
            };
        private static bool _loggedDiscoveredCreatures;

        public static void AddAllRegisteredCreatures(List<SaveInfo.Info.KilledInfo> killedInfo)
        {
            foreach (CreatureTemplate.Type type in GetAllRegisteredTypes())
                AddIfMissing(killedInfo, type, 0, 0);

            foreach (KeyValuePair<string, int[]> pair in ExtraDebugVariants)
            {
                CreatureTemplate.Type type = GetTypeByValue(pair.Key);
                if (type == null)
                    continue;

                foreach (int data in pair.Value)
                    AddIfMissing(killedInfo, type, data, 0);
            }
        }

        public static List<SaveInfo.Info.KilledInfo> PrepareForDisplay(
            List<SaveInfo.Info.KilledInfo> source,
            bool forceDebugGroups)
        {
            source = source ?? new List<SaveInfo.Info.KilledInfo>();

            Dictionary<string, SaveInfo.Info.KilledInfo> merged =
                new Dictionary<string, SaveInfo.Info.KilledInfo>(StringComparer.OrdinalIgnoreCase);

            List<string> originalOrder = new List<string>();

            foreach (SaveInfo.Info.KilledInfo info in source)
            {
                if (info == null)
                    continue;

                if (IsIgnoredCreatureType(info.iconData.critType))
                    continue;

                string key = MakeRawKey(info.iconData.critType, info.iconData.intData);

                if (merged.TryGetValue(key, out SaveInfo.Info.KilledInfo existing))
                {
                    existing.kills += info.kills;
                }
                else
                {
                    SaveInfo.Info.KilledInfo copy = new SaveInfo.Info.KilledInfo
                    {
                        iconData = info.iconData,
                        kills = info.kills
                    };

                    merged.Add(key, copy);
                    originalOrder.Add(key);
                }
            }

            HashSet<string> consumed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            List<SaveInfo.Info.KilledInfo> result = new List<SaveInfo.Info.KilledInfo>();

            TryAddLizardGroup(result, merged, consumed, 1, "lizards_data1", "\"Slight\" and \"Opossum\"\nRot Lizards", forceDebugGroups);
            TryAddLizardGroup(result, merged, consumed, 2, "lizards_data2", "\"Full\" Rot Lizards", forceDebugGroups);
            TryAddCentipedeGroup(result, merged, consumed, forceDebugGroups);

            foreach (string key in originalOrder)
            {
                if (consumed.Contains(key))
                    continue;

                SaveInfo.Info.KilledInfo info = merged[key];
                DecorateSingleEntry(info);

                if (!Hidden.Contains(info.catalogId))
                    result.Add(info);
            }

            Dictionary<string, int> explicitOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < Order.Count; i++)
            {
                if (!explicitOrder.ContainsKey(Order[i]))
                    explicitOrder.Add(Order[i], i);
            }

            return result
                .Select((info, index) => new
                {
                    Info = info,
                    NaturalIndex = index,
                    ExplicitIndex = explicitOrder.TryGetValue(info.catalogId ?? string.Empty, out int rank)
                        ? rank
                        : int.MaxValue
                })
                .OrderBy(x => x.ExplicitIndex)
                .ThenBy(x => x.NaturalIndex)
                .Select(x => x.Info)
                .ToList();
        }
        private static void TryAddCentipedeGroup(
            List<SaveInfo.Info.KilledInfo> result,
            Dictionary<string, SaveInfo.Info.KilledInfo> merged,
            HashSet<string> consumed,
            bool forceDebugGroup)
        {
            CreatureTemplate.Type type = CreatureTemplate.Type.Centipede;

            int kills = 0;
            int totalPoints = 0;

            bool hasAnyRealEntry = false;
            bool unknownTotalPoints = false;

            for (int data = 0; data <= 3; data++)
            {
                string rawKey = MakeRawKey(type, data);

                if (!merged.TryGetValue(
                    rawKey,
                    out SaveInfo.Info.KilledInfo existing))
                {
                    continue;
                }

                hasAnyRealEntry = true;

                kills += existing.kills;
                consumed.Add(rawKey);

                if (existing.kills > 0)
                {
                    IconSymbol.IconSymbolData scoreData =
                        new IconSymbol.IconSymbolData(
                            type,
                            AbstractPhysicalObject.AbstractObjectType.Creature,
                            data
                        );

                    int variantScore =
                        BestiaryMenu.GetKillScore(scoreData);

                    if (variantScore == -1)
                    {
                        unknownTotalPoints = true;
                    }
                    else
                    {
                        totalPoints +=
                            variantScore * existing.kills;
                    }
                }
            }
             if (!forceDebugGroup && !hasAnyRealEntry)
                return;

            const string catalogId = "Centipede";

            if (Hidden.Contains(catalogId))
                return;
             IconSymbol.IconSymbolData displayIcon =
                new IconSymbol.IconSymbolData(
                    type,
                    AbstractPhysicalObject.AbstractObjectType.Creature,
                    2
                );

            SaveInfo.Info.KilledInfo centipede =
                new SaveInfo.Info.KilledInfo
                {
                     iconData = displayIcon,
                     kills = kills,

                    catalogId = catalogId,
                    displayNameOverride = Plugin.Translate("Centipede"),

                    iconVariants =
                        new List<IconSymbol.IconSymbolData>
                        {
                            displayIcon
                        },

                    isGroup = false,

                    customIconPath =
                        "bestiary_custom/icons/Centipede",

                    customImagePath =
                        "bestiary_custom/images/Centipede",
                     healthOverride =
                        "Depends on length",

                    pointsPerKillOverride =
                        "Depends on length",
                      totalPointsOverride =
                        unknownTotalPoints
                            ? "?"
                            : totalPoints.ToString()
                };

            result.Add(centipede);
        }
        private static void TryAddLizardGroup(
            List<SaveInfo.Info.KilledInfo> result,
            Dictionary<string, SaveInfo.Info.KilledInfo> merged,
            HashSet<string> consumed,
            int data,
            string catalogId,
            string displayName,
            bool forceDebugGroup)
        {
            List<CreatureTemplate.Type> lizardTypes = GetLizardTypes();
            if (lizardTypes.Count == 0)
                return;

            List<IconSymbol.IconSymbolData> variants = new List<IconSymbol.IconSymbolData>();
            int kills = 0;
            bool hasAnyRealEntry = false;

            foreach (CreatureTemplate.Type type in lizardTypes)
            {
                IconSymbol.IconSymbolData variant = new IconSymbol.IconSymbolData(
                    type,
                    AbstractPhysicalObject.AbstractObjectType.Creature,
                    data);

                variants.Add(variant);

                string rawKey = MakeRawKey(type, data);
                if (merged.TryGetValue(rawKey, out SaveInfo.Info.KilledInfo existing))
                {
                    hasAnyRealEntry = true;
                    kills += existing.kills;
                    consumed.Add(rawKey);
                }
            }

            if (!forceDebugGroup && !hasAnyRealEntry)
                return;

            if (Hidden.Contains(catalogId))
                return;

            CreatureTemplate.Type primaryType = lizardTypes.FirstOrDefault(x => x == CreatureTemplate.Type.GreenLizard)
                                                ?? lizardTypes[0];

            SaveInfo.Info.KilledInfo group =
                new SaveInfo.Info.KilledInfo
                {
                    iconData =
                        new IconSymbol.IconSymbolData(
                            primaryType,
                            AbstractPhysicalObject.AbstractObjectType.Creature,
                            data
                        ),

                    kills = kills,
                    catalogId = catalogId,

                    displayNameOverride =
                        displayName,

                    iconVariants =
                        variants,

                    isGroup = true,

                    customIconPath =
                        $"bestiary_custom/icons/{catalogId}",

                    customImagePath =
                        $"bestiary_custom/images/{catalogId}"
                };

            result.Add(group);
        }

        private static void DecorateSingleEntry(
            SaveInfo.Info.KilledInfo info)
        {
            string id =
                MakeEntryId(
                    info.iconData.critType,
                    info.iconData.intData
                );

            info.catalogId = id;
            info.isGroup = false;

            info.iconVariants =
                new List<IconSymbol.IconSymbolData>
                {
                    info.iconData
                };

            info.customIconPath =
                $"bestiary_custom/icons/{id}";

            info.customImagePath =
                $"bestiary_custom/images/{id}";
  
            if (CustomDisplayNames.TryGetValue(
                id,
                out string customName))
            {
                info.displayNameOverride =
                    Plugin.Translate(customName);
            }
            else if (info.iconData.intData != 0)
            {
                info.displayNameOverride =
                    $"{Plugin.ResolveCreatureName(info.iconData.critType.value)} " +
                    $"[data {info.iconData.intData}]";
            }
            if (DependsOnLengthCreatures.Contains(info.catalogId))
            {
                info.healthOverride =
                    "b-DependsOnLength";

                info.pointsPerKillOverride =
                    "b-DependsOnLength";
            }
        }

        public static void LogDiscoveredCreatures()
        {
            if (_loggedDiscoveredCreatures)
                return;

            _loggedDiscoveredCreatures = true;

            Plugin.logger.LogInfo("================ BESTIARY CREATURE SCAN ================");
            Plugin.logger.LogInfo($"Scanning CreatureTemplate.Type entries and unique icon variants for intData 0..{DebugVariantScanMaxData}.");

            int typeIndex = 0;
            foreach (CreatureTemplate.Type type in GetAllRegisteredTypes())
            {
                string entryId = MakeEntryId(type, 0);
                Plugin.logger.LogInfo($"[BESTIARY TYPE {typeIndex:000}] id={entryId} type={type.value}");

                HashSet<string> seenVisuals = new HashSet<string>();

                for (int data = 0; data <= DebugVariantScanMaxData; data++)
                {
                    try
                    {
                        IconSymbol.IconSymbolData iconData = new IconSymbol.IconSymbolData(
                            type,
                            AbstractPhysicalObject.AbstractObjectType.Creature,
                            data);

                        string sprite = CreatureSymbol.SpriteNameOfCreature(iconData);
                        Color color = CreatureSymbol.ColorOfCreature(iconData);
                        string signature = $"{sprite}|{color.r:F3}|{color.g:F3}|{color.b:F3}|{color.a:F3}";
                         if (seenVisuals.Add(signature))
                        {
                            Plugin.logger.LogInfo(
                                $"    [VARIANT] data={data} id={MakeEntryId(type, data)} sprite={sprite} " +
                                $"color=({color.r:F2},{color.g:F2},{color.b:F2},{color.a:F2})");
                        }
                    }
                    catch (Exception ex)
                    {
                         Plugin.logger.LogDebug($"    [VARIANT-ERROR] {type.value} data={data}: {ex.Message}");
                    }
                }

                typeIndex++;
            }

            List<CreatureTemplate.Type> lizards = GetLizardTypes();
            Plugin.logger.LogInfo($"[BESTIARY GROUP] id=lizards_data1 members={string.Join(", ", lizards.Select(x => x.value).ToArray())}");
            Plugin.logger.LogInfo($"[BESTIARY GROUP] id=lizards_data2 members={string.Join(", ", lizards.Select(x => x.value).ToArray())}");
            Plugin.logger.LogInfo("========================================================");
        }

        public static int GetAnimatedVariantIndex(int variantCount)
        {
            if (variantCount <= 1)
                return 0;

            float seconds = Mathf.Max(0.01f, GroupIconSecondsPerVariant);
            return Mathf.FloorToInt(Time.unscaledTime / seconds) % variantCount;
        }

        public static bool IsIgnoredCreatureType(CreatureTemplate.Type type)
        {
            return type == null || IgnoredCreatureTypes.Contains(type.value);
        }

        public static string MakeEntryId(CreatureTemplate.Type type, int data)
        {
            if (type == null)
                return "UnknownCreature";

            return data == 0 ? type.value : $"{type.value}_data{data}";
        }

        private static string MakeRawKey(CreatureTemplate.Type type, int data)
        {
            return $"{type?.value ?? "<null>"}|{data}";
        }

        private static void AddIfMissing(
            List<SaveInfo.Info.KilledInfo> killedInfo,
            CreatureTemplate.Type type,
            int data,
            int kills)
        {
            if (type == null)
                return;

            if (killedInfo.Any(x =>
                x != null &&
                x.iconData.critType == type &&
                x.iconData.intData == data))
            {
                return;
            }

            killedInfo.Add(new SaveInfo.Info.KilledInfo
            {
                iconData = new IconSymbol.IconSymbolData(
                    type,
                    AbstractPhysicalObject.AbstractObjectType.Creature,
                    data),
                kills = kills
            });
        }

        private static List<CreatureTemplate.Type> GetAllRegisteredTypes()
        {
            List<CreatureTemplate.Type> result = new List<CreatureTemplate.Type>();

            foreach (string text in ExtEnum<CreatureTemplate.Type>.values.entries)
            {
                try
                {
                    CreatureTemplate.Type type =
                        (CreatureTemplate.Type)ExtEnumBase.Parse(
                            typeof(CreatureTemplate.Type),
                            text,
                            false);

                    if (type != null && !IsIgnoredCreatureType(type))
                        result.Add(type);
                }
                catch (Exception ex)
                {
                    Plugin.logger.LogWarning($"[Bestiary] Failed to parse creature type '{text}': {ex.Message}");
                }
            }

            return result;
        }

        private static List<CreatureTemplate.Type> GetLizardTypes()
        {
            return GetAllRegisteredTypes()
                .Where(type =>
                    type != null &&
                    (type.value.IndexOf("Lizard", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     type == CreatureTemplate.Type.Salamander))
                .ToList();
        }

        private static CreatureTemplate.Type GetTypeByValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            foreach (CreatureTemplate.Type type in GetAllRegisteredTypes())
            {
                if (string.Equals(type.value, value, StringComparison.OrdinalIgnoreCase))
                    return type;
            }

            return null;
        }
    }
}
