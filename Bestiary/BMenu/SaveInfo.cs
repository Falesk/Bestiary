using System.Collections.Generic;

namespace Bestiary.BMenu
{
    public class SaveInfo
    {
        public SlugcatStats.Name name;
        public List<Info.KilledInfo> kills;
        public List<Info.ItemInfo> items;

        public int cycles;
        public int deaths;
        public bool hasSave;

        public SaveInfo(SlugcatStats.Name _name, List<Info.KilledInfo> _kills, List<Info.ItemInfo> _items, int _cycles = 0, int _deaths = 0)
        {
            name = _name;
            kills = _kills;
            items = _items;

            cycles = _cycles;
            deaths = _deaths;
            hasSave = true;
        }

        public SaveInfo(SlugcatStats.Name _name)
        {
            name = _name;
            kills = null;
            items = null;

            cycles = 0;
            deaths = 0;
            hasSave = false;
        }

        public abstract class Info
        {
            public IconSymbol.IconSymbolData iconData;

            public class KilledInfo : Info
            {
                public int kills;
                public string catalogId;
                public string displayNameOverride;
                public string descriptionOverride;
                public string customIconPath;
                public string customImagePath;
                public bool isGroup;
                public string healthOverride;
                public string pointsPerKillOverride;
                public string totalPointsOverride;
                public List<IconSymbol.IconSymbolData> iconVariants;

                public static KilledInfo Transform(KeyValuePair<IconSymbol.IconSymbolData, int> pair) =>
                    new KilledInfo
                    {
                        iconData = pair.Key,
                        kills = pair.Value
                    };
            }

            public class ItemInfo : Info
            {
                public AbstractPhysicalObject.AbstractObjectType objectType;

                public static ItemInfo Transform(IconSymbol.IconSymbolData data) =>
                    new ItemInfo
                    {
                        iconData = data,
                        objectType = data.itemType
                    };
            }
        }
    }
}
