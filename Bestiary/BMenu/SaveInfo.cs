using System.Collections.Generic;

namespace Bestiary.BMenu
{
    public class SaveInfo
    {
        public SlugcatStats.Name name;
        public List<Info.KilledInfo> kills;
        public List<Info.ItemInfo> items;

        public SaveInfo(SlugcatStats.Name _name, List<Info.KilledInfo> _kills, List<Info.ItemInfo> _items)
        {
            name = _name;
            kills = _kills;
            items = _items;
        }

        public SaveInfo(SlugcatStats.Name _name)
        {
            name = _name;
            kills = null;
        }

        public abstract class Info
        {
            public IconSymbol.IconSymbolData iconData;

            public class KilledInfo : Info
            {
                public int kills;

                public static KilledInfo Transform(KeyValuePair<IconSymbol.IconSymbolData, int> pair) => new KilledInfo { iconData = pair.Key, kills = pair.Value };
            }

            public class ItemInfo : Info
            {
                public AbstractPhysicalObject.AbstractObjectType objectType;

                public static ItemInfo Transform(IconSymbol.IconSymbolData data) => new ItemInfo { iconData = data, objectType = data.itemType };
            }
        }
    }
}
