using System.Collections.Generic;

namespace Bestiary.BMenu
{
    public struct SlugcatInfo
    {
        public SlugcatStats.Name name;
        public List<KilledInfo> kills;

        public SlugcatInfo(SlugcatStats.Name _name, List<KilledInfo> _kills)
        {
            name = _name;
            kills = _kills;
        }

        public SlugcatInfo(SlugcatStats.Name _name)
        {
            name = _name;
            kills = null;
        }

        public struct KilledInfo
        {
            public IconSymbol.IconSymbolData iconData;
            public int kills;

            public static KilledInfo Transform(KeyValuePair<IconSymbol.IconSymbolData, int> pair)
            {
                return new KilledInfo { iconData = pair.Key, kills = pair.Value };
            }
        }
    }
}
