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
#if DEBUG
             CreatureCatalog.LogDiscoveredCreatures();
#endif

            List<SaveInfo> listSlugcats = new List<SaveInfo>();

            for (int i = 0; i < SlugcatStats.Name.values.Count; i++)
            {
                SlugcatStats.Name name = new SlugcatStats.Name(SlugcatStats.Name.values.GetEntry(i));
                if (!ModManager.MSC && (SlugcatStats.IsSlugcatFromMSC(name) || name.value == "Slugpup"))
                    continue;

                if (!ModManager.Watcher && name.value == "Watcher")
                    continue;

                if (SlugcatStats.HiddenOrUnplayableSlugcat(name))
                    continue;

                BestiarySaveReader.Data saveData;
                bool hasSave = BestiarySaveReader.TryRead(bMenu.manager.rainWorld.progression, name, out saveData);
                SaveInfo saveInfo;
                if (hasSave)
                {
                    var kills = saveData.kills;
                    List<SaveInfo.Info.KilledInfo> killedInfo = new List<SaveInfo.Info.KilledInfo>();

                    for (int j = 0; j < kills.Count; j++)
                    {
                         if (!killedInfo.Any(x =>
                            x.iconData.critType == kills[j].Key.critType &&
                            x.iconData.intData == kills[j].Key.intData))
                        {
                            killedInfo.Add(SaveInfo.Info.KilledInfo.Transform(kills[j]));
                        }
                    }

                    bool forceDebugGroups = false;
#if DEBUG
                    CreatureCatalog.AddAllRegisteredCreatures(killedInfo);
                    forceDebugGroups = true;
#endif

                    killedInfo = CreatureCatalog.PrepareForDisplay(killedInfo, forceDebugGroups);
                    List<SaveInfo.Info.ItemInfo> itemInfo = new List<SaveInfo.Info.ItemInfo>();
                    saveInfo = new SaveInfo(name, killedInfo, itemInfo, saveData.cycleNumber, saveData.deaths);
                }
                else
                {
#if DEBUG
                     List<SaveInfo.Info.KilledInfo> killedInfo = new List<SaveInfo.Info.KilledInfo>();
                    CreatureCatalog.AddAllRegisteredCreatures(killedInfo);
                    killedInfo = CreatureCatalog.PrepareForDisplay(killedInfo, true);
                    saveInfo = new SaveInfo(name, killedInfo, new List<SaveInfo.Info.ItemInfo>());
#else
                    saveInfo = new SaveInfo(name);
#endif
                }

                listSlugcats.Add(saveInfo);
            }
            if (ModManager.MSC)
                Inv(listSlugcats);
            Saves = listSlugcats.ToArray();
        }

        private void Inv(List<SaveInfo> listSlugcats)
        {
            SlugcatStats.Name name = MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel;
            BestiarySaveReader.Data saveData;

            if (BestiarySaveReader.TryRead(bMenu.manager.rainWorld.progression, name, out saveData))
            {
                var kills = saveData.kills;
                List<SaveInfo.Info.KilledInfo> killedInfo = new List<SaveInfo.Info.KilledInfo>();

                for (int j = 0; j < kills.Count; j++)
                {
                    if (!killedInfo.Any(x =>
                        x.iconData.critType == kills[j].Key.critType &&
                        x.iconData.intData == kills[j].Key.intData))
                    {
                        killedInfo.Add(SaveInfo.Info.KilledInfo.Transform(kills[j]));
                    }
                }

                bool forceDebugGroups = false;
#if DEBUG
                CreatureCatalog.AddAllRegisteredCreatures(killedInfo);
                forceDebugGroups = true;
#endif

                killedInfo = CreatureCatalog.PrepareForDisplay(killedInfo, forceDebugGroups);

                List<SaveInfo.Info.ItemInfo> itemInfo = new List<SaveInfo.Info.ItemInfo>();

                if (killedInfo.Count > 0)
                    listSlugcats.Add(new SaveInfo(name, killedInfo, itemInfo, saveData.cycleNumber, saveData.deaths));
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
            else
                _slugcatSlideNum -= (_slugcatSlideNum == 0) ? 0 : 1;

            bool flag = _slugcatSlideNum + slugsInColumn >= Saves.Length;

            bMenu.buttonManager.downButton.button.buttonBehav.greyedOut = flag;
            bMenu.buttonManager.downButton.icon.color = flag
                ? Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey)
                : Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White);

            bMenu.buttonManager.upButton.button.buttonBehav.greyedOut = _slugcatSlideNum == 0;
            bMenu.buttonManager.upButton.icon.color = _slugcatSlideNum == 0
                ? Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey)
                : Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White);

            bMenu.buttonManager.RefreshSlugcats(_slugcatSlideNum);
        }
    }
}
