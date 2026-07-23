using System.Collections.Generic;
using System.Linq;

namespace Bestiary.BMenu
{
    public class SlugcatManager
    {
        public BestiaryMenu bMenu;
        public SaveInfo[] Saves { get; private set; }
        public int SelectedSlugcat { get; private set; }
        private int _slugcatSlideNum;
        public const int slugsInColumn = 11;

        public SlugcatManager(BestiaryMenu owner)
        {
            bMenu = owner;
            SelectedSlugcat = -1;
        }

        public void InitSlugcats()
        {
            bool debugOpenAll = false;
            List<SaveInfo> listSlugcats = new List<SaveInfo>();

            for (int i = 0; i < SlugcatStats.Name.values.Count; i++)
            {
                SlugcatStats.Name name = new SlugcatStats.Name(SlugcatStats.Name.values.GetEntry(i));
                if (SlugcatStats.HiddenOrUnplayableSlugcat(name)) continue;

                bool hasSave = bMenu.manager.rainWorld.progression.IsThereASavedGame(name);

                SaveInfo saveInfo;
                if (hasSave)
                {
                    var kills = bMenu.manager.rainWorld.progression.GetOrInitiateSaveState(name, null, bMenu.manager.menuSetup, false).kills;
                    List<SaveInfo.Info.KilledInfo> killedInfo = new List<SaveInfo.Info.KilledInfo>();
                    for (int j = 0; j < kills.Count; j++)
                        killedInfo.Add(SaveInfo.Info.KilledInfo.Transform(kills[j]));
                    if (debugOpenAll)
                    {
                        for (int j = 0; j < CreatureTemplate.Type.values.Count; j++)
                        {
                            CreatureTemplate.Type type = new CreatureTemplate.Type(CreatureTemplate.Type.values.GetEntry(j));
                            if (!killedInfo.Contains(killedInfo.FirstOrDefault(x => x.iconData.critType == type)))
                                killedInfo.Add(new SaveInfo.Info.KilledInfo { iconData = new IconSymbol.IconSymbolData(type, AbstractPhysicalObject.AbstractObjectType.Creature, 0), kills = 0 });
                        }
                    }
                    saveInfo = new SaveInfo(name, killedInfo);
                }
                else saveInfo = new SaveInfo(name);
                listSlugcats.Add(saveInfo);
            }
            Inv(listSlugcats);

            //for (int i = 0; i < 5; i++)
            //    listSlugcats.Add(new SaveInfo(SlugcatStats.Name.White, new List<SaveInfo.Info.KilledInfo>()));

            Saves = listSlugcats.ToArray();
        }

        private void Inv(List<SaveInfo> listSlugcats)
        {
            SlugcatStats.Name name = MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel;
            if (bMenu.manager.rainWorld.progression.IsThereASavedGame(name))
            {
                var kills = bMenu.manager.rainWorld.progression.GetOrInitiateSaveState(name, null, bMenu.manager.menuSetup, false).kills;
                List<SaveInfo.Info.KilledInfo> killedInfo = new List<SaveInfo.Info.KilledInfo>();
                for (int j = 0; j < kills.Count; j++)
                    killedInfo.Add(SaveInfo.Info.KilledInfo.Transform(kills[j]));
                if (killedInfo.Count > 0)
                    listSlugcats.Add(new SaveInfo(name, killedInfo));
            }
        }

        public void ButtonClicked(int index)
        {
            SelectedSlugcat = index;
            bMenu.buttonManager.SlugcatButtonToggles(SelectedSlugcat);
        }

        public void SliderClicked(bool down)
        {
            if (down && _slugcatSlideNum + slugsInColumn <= Saves.Length)
                _slugcatSlideNum += _slugcatSlideNum + slugsInColumn >= Saves.Length ? 0 : 1;
            else _slugcatSlideNum -= (_slugcatSlideNum == 0) ? 0 : 1;
            bool flag = _slugcatSlideNum + slugsInColumn >= Saves.Length;

            bMenu.buttonManager.downButton.button.buttonBehav.greyedOut = flag;
            bMenu.buttonManager.downButton.icon.color = flag ? Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey) : Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White);
            bMenu.buttonManager.upButton.button.buttonBehav.greyedOut = _slugcatSlideNum == 0;
            bMenu.buttonManager.upButton.icon.color = _slugcatSlideNum == 0 ? Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey) : Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White);

            bMenu.buttonManager.RefreshSlugcats(_slugcatSlideNum);
        }
    }
}