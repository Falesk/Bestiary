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
            Creatures = new Dictionary<string, string>();
            Slugcats = new Dictionary<string, string>();

            string path = AssetManager.ResolveFilePath($"text/text_{locale}/descriptions.json");
            if (!File.Exists(path))
                path = AssetManager.ResolveFilePath($"text/text_eng/descriptions.json");
            Dictionary<string, object> dict = File.ReadAllText(path).dictionaryFromJson();

            try
            {
                if (dict != null)
                {
                    if (dict.TryGetValue("creatures", out object _))
                    {
                        Dictionary<string, object> d = (Dictionary<string, object>)dict["creatures"];
                        foreach (var pair in d)
                            Creatures.Add(pair.Key, pair.Value.ToString());
                    }
                    if (dict.TryGetValue("slugcats", out object _))
                    {
                        Dictionary<string, object> d = (Dictionary<string, object>)dict["slugcats"];
                        foreach (var pair in d)
                            Slugcats.Add(pair.Key, pair.Value.ToString());
                    }
                }
            }
            catch(Exception ex) { Debug.LogException(ex); }
        }
    }
}
