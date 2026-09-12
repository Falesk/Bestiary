using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bestiary.BMenu
{
    public static class BestiarySaveReader
    {
        public class Data
        {
            public int cycleNumber;
            public int deaths;

            public List<KeyValuePair<IconSymbol.IconSymbolData, int>> kills =
                new List<KeyValuePair<IconSymbol.IconSymbolData, int>>();
        }

        public static bool TryRead(
            PlayerProgression progression,
            SlugcatStats.Name slugcat,
            out Data data)
        {
            data = null;

            if (progression == null ||
                slugcat == null ||
                !progression.HasSaveData)
            {
                return false;
            }

            string[] lines = progression.GetProgLinesFromMemory();

            if (lines == null || lines.Length == 0)
                return false;

            const string progDivider = "<progDivB>";

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                int dividerIndex =
                    line.IndexOf(
                        progDivider,
                        StringComparison.Ordinal
                    );

                if (dividerIndex < 0)
                    continue;

                string header =
                    line.Substring(0, dividerIndex);

                if (header != "SAVE STATE")
                    continue;

                string saveString =
                    line.Substring(
                        dividerIndex +
                        progDivider.Length
                    );

                SlugcatStats.Name saveSlugcat;

                try
                {
                    saveSlugcat =
                        BackwardsCompatibilityRemix
                            .ParseSaveNumber(saveString);
                }
                catch
                {
                    continue;
                }

                if (saveSlugcat != slugcat)
                    continue;

                data = ParseSave(saveString);

#if DEBUG
                Plugin.logger.LogInfo(
                    $"[Bestiary] Read save directly: " +
                    $"{slugcat.value}, " +
                    $"cycles={data.cycleNumber}, " +
                    $"deaths={data.deaths}, " +
                    $"kills={data.kills.Count}"
                );
#endif

                return true;
            }

            return false;
        }

        private static Data ParseSave(string saveString)
        {
            Data data = new Data();

            List<SaveStateMiner.Target> targets =
                new List<SaveStateMiner.Target>
                {
                    new SaveStateMiner.Target(
                        ">CYCLENUM",
                        "<svB>",
                        "<svA>",
                        50
                    ),

                    new SaveStateMiner.Target(
                        ">DEATHS",
                        "<dpB>",
                        "<dpA>",
                        50
                    )
                };

            List<SaveStateMiner.Result> results =
                SaveStateMiner.Mine(
                    RWCustom.Custom.rainWorld,
                    saveString,
                    targets
                );

            foreach (SaveStateMiner.Result result in results)
            {
                if (result.name == ">CYCLENUM")
                {
                    int.TryParse(
                        result.data,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out data.cycleNumber
                    );
                }
                else if (result.name == ">DEATHS")
                {
                    int.TryParse(
                        result.data,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out data.deaths
                    );
                }
            }

            string deathPersistent =
                GetField(
                    saveString,
                    "DEATHPERSISTENTSAVEDATA",
                    "<svB>",
                    "<svA>"
                );

            if (!string.IsNullOrEmpty(deathPersistent))
            {
                string deathsText =
                    GetField(
                        deathPersistent,
                        "DEATHS",
                        "<dpB>",
                        "<dpA>"
                    );

                if (!string.IsNullOrEmpty(deathsText))
                {
                    int.TryParse(
                        deathsText,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out data.deaths
                    );
                }
            }

            string killsText =
                GetField(
                    saveString,
                    "KILLS",
                    "<svB>",
                    "<svA>"
                );

            if (!string.IsNullOrEmpty(killsText))
            {
                string[] entries =
                    killsText.Split(
                        new string[] { "<svC>" },
                        StringSplitOptions.RemoveEmptyEntries
                    );

                foreach (string entry in entries)
                {
                    int divider =
                        entry.IndexOf(
                            "<svD>",
                            StringComparison.Ordinal
                        );

                    if (divider < 0)
                        continue;

                    string iconString =
                        entry.Substring(0, divider);

                    string countString =
                        entry.Substring(
                            divider + "<svD>".Length
                        );

                    try
                    {
                        IconSymbol.IconSymbolData iconData =
                            IconSymbol.IconSymbolData
                                .IconSymbolDataFromString(
                                    iconString
                                );

                        if (iconData.critType != null &&
                            iconData.critType.Index == -1)
                        {
                            continue;
                        }

                        if (iconData.itemType != null &&
                            iconData.itemType.Index == -1)
                        {
                            continue;
                        }

                        if (!int.TryParse(
                            countString,
                            NumberStyles.Any,
                            CultureInfo.InvariantCulture,
                            out int count))
                        {
                            continue;
                        }

                        data.kills.Add(
                            new KeyValuePair<
                                IconSymbol.IconSymbolData,
                                int>(
                                iconData,
                                count
                            )
                        );
                    }
                    catch (Exception ex)
                    {
                        Plugin.logger.LogWarning(
                            $"[Bestiary] Failed to read kill " +
                            $"entry '{entry}': {ex.Message}"
                        );
                    }
                }
            }

            return data;
        }

        private static string GetField(
            string source,
            string name,
            string divider,
            string cap)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            string startToken =
                name + divider;

            int start =
                source.IndexOf(
                    startToken,
                    StringComparison.Ordinal
                );

            if (start < 0)
                return null;

            start += startToken.Length;

            int end =
                source.IndexOf(
                    cap,
                    start,
                    StringComparison.Ordinal
                );

            if (end < 0)
                return null;

            return source.Substring(
                start,
                end - start
            );
        }
    }
}