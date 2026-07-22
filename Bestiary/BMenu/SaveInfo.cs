using System.Collections.Generic;

namespace Bestiary.BMenu
{
    public class SaveInfo
    {
        public SlugcatStats.Name name;
        public List<Info.KilledInfo> kills;

        public SaveInfo(SlugcatStats.Name _name, List<Info.KilledInfo> _kills)
        {
            name = _name;
            kills = _kills;
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
        }
    }
}
