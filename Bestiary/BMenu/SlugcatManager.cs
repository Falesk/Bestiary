using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bestiary.BMenu
{
    public class SlugcatManager
    {
        public BM bMenu;
        public SlugcatInfo[] Slugcats { get; private set; }
        private int choosedSlugcat, slugcatSlideNum;
        public const int slugsInColumn = 11;

        public SlugcatManager(BM owner)
        {
            bMenu = owner;
            choosedSlugcat = -1;
        }

        public void InitSlugcats()
        {
            bool debugOpenAll = true;
            List<SlugcatInfo> listSlugcats = new List<SlugcatInfo>();

            for (int i = 0; i < SlugcatStats.Name.values.Count; i++)
            {
                SlugcatStats.Name name = new SlugcatStats.Name(SlugcatStats.Name.values.GetEntry(i));
                if (SlugcatStats.HiddenOrUnplayableSlugcat(name)) continue;

                bool hasSave = bMenu.manager.rainWorld.progression.IsThereASavedGame(name);

                SlugcatInfo slugInfo;
                if (hasSave)
                {
                    var kills = bMenu.manager.rainWorld.progression.GetOrInitiateSaveState(name, null, bMenu.manager.menuSetup, false).kills;
                    List<SlugcatInfo.KilledInfo> killedInfo = new List<SlugcatInfo.KilledInfo>();
                    for (int j = 0; j < kills.Count; j++)
                        killedInfo.Add(SlugcatInfo.KilledInfo.Transform(kills[j]));
                    if (debugOpenAll)
                    {
                        for (int j = 0; j < CreatureTemplate.Type.values.Count; j++)
                        {
                            CreatureTemplate.Type type = new CreatureTemplate.Type(CreatureTemplate.Type.values.GetEntry(j));
                            if (!killedInfo.Contains(killedInfo.FirstOrDefault(x => x.iconData.critType == type))/* && CreatureIsKillable(type)*/)
                                killedInfo.Add(new SlugcatInfo.KilledInfo { iconData = new IconSymbol.IconSymbolData(type, AbstractPhysicalObject.AbstractObjectType.Creature, 0), kills = 0 });
                        }
                    }
                    slugInfo = new SlugcatInfo(name, killedInfo);
                }
                else slugInfo = new SlugcatInfo(name);
                listSlugcats.Add(slugInfo);
            }

            Slugcats = listSlugcats.ToArray();
        }

        public void Action(int index)
        {
            choosedSlugcat = index;
            bMenu.buttonManager.UpdateButtonToggles(choosedSlugcat);
        }
    }
}
