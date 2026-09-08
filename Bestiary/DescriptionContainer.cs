using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Bestiary
{
    public class DescriptionContainer
    {
        public Dictionary<string, string> Creatures;
        public Dictionary<string, string> Slugcats;

        public DescriptionContainer(string locale)
        {
            Creatures =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase
                );

            Slugcats =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase
                );
            LoadFile("eng");
            if (!string.Equals(
                locale,
                "eng",
                StringComparison.OrdinalIgnoreCase))
            {
                LoadFile(locale);
            }
        }

        private void LoadFile(string locale)
        {
            string path =
                AssetManager.ResolveFilePath(
                    $"text/text_{locale}/descriptions.json"
                );

            if (!File.Exists(path))
                return;

            try
            {
                Dictionary<string, object> dict =
                    File.ReadAllText(path)
                        .dictionaryFromJson();

                if (dict == null)
                    return;

                if (dict.TryGetValue(
                    "creatures",
                    out object creaturesObj))
                {
                    Dictionary<string, object> creatures =
                        creaturesObj as Dictionary<string, object>;

                    if (creatures != null)
                    {
                        foreach (var pair in creatures)
                        {
                            Creatures[pair.Key] =
                                pair.Value?.ToString()
                                ?? string.Empty;
                        }
                    }
                }

                if (dict.TryGetValue(
                    "slugcats",
                    out object slugcatsObj))
                {
                    Dictionary<string, object> slugcats =
                        slugcatsObj as Dictionary<string, object>;

                    if (slugcats != null)
                    {
                        foreach (var pair in slugcats)
                        {
                            Slugcats[pair.Key] =
                                pair.Value?.ToString()
                                ?? string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}