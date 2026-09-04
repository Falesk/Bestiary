using System.Collections.Generic;

namespace Bestiary
{
    public static class HooksGeneral
    {
        public static void Init()
        {
            On.PlayerProgression.MiscProgressionData.ctor += MiscProgressionData_ctor;
            //On.PlayerProgression.MiscProgressionData.ToString += MiscProgressionData_ToString;
            //On.PlayerProgression.MiscProgressionData.FromString += MiscProgressionData_FromString;
            //On.WinState.CycleCompleted += WinState_CycleCompleted;
            On.InGameTranslator.LoadFonts += InGameTranslator_LoadFonts;
        }

        private static void InGameTranslator_LoadFonts(On.InGameTranslator.orig_LoadFonts orig, InGameTranslator.LanguageID lang, Menu.Menu menu)
        {
            orig(lang, menu);
            string locale = LocalizationTranslator.LangShort(lang);
            Plugin.descriptionContainer = new DescriptionContainer(locale);
        }

        //private static void WinState_CycleCompleted(On.WinState.orig_CycleCompleted orig, WinState self, RainWorldGame game)
        //{
        //    orig(self, game);
        //    CustomData.MiscProgressionCustomData cData = game.world.regionState.saveState.progression.miscProgressionData.GetData();
        //    cData.savedObjects.Clear();
        //    for (int i = 0; i < game.world.shelters.Length; i++)
        //    {
        //        AbstractRoom room = game.world.GetAbstractRoom(game.world.shelters[i]);
        //        for (int j = 0; j < room.entities.Count; j++)
        //        {
        //            if (room.entities[j] is AbstractPhysicalObject obj && !(room.entities[j] is AbstractCreature))
        //            {
        //                IconSymbol.IconSymbolData? iconData = ItemSymbol.SymbolDataFromItem(obj);
        //                if (iconData != null && !cData.savedObjects.Contains(iconData.Value))
        //                    cData.savedObjects.Add(iconData.Value);
        //            }
        //        }
        //    }
        //}

        //private static void MiscProgressionData_FromString(On.PlayerProgression.MiscProgressionData.orig_FromString orig, PlayerProgression.MiscProgressionData self, string s)
        //{
        //    try
        //    {
        //        string[] data = s.Split(new string[] { "<mpdA>" }, System.StringSplitOptions.RemoveEmptyEntries);
        //        string soLine = data.FirstOrDefault(x => x.Split(new string[] { "<mpdB>" }, System.StringSplitOptions.RemoveEmptyEntries)[0] == "BSAVEDOBJECTS");

        //        if (soLine != default && self.GetData() is CustomData.MiscProgressionCustomData miscData)
        //        {
        //            string[] savedObjects = soLine.Split(new string[] { "<mpdB>" }, System.StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { ',' });
        //            for (int i = 0; i < savedObjects.Length; i++)
        //            {
        //                miscData.savedObjects.Add(IconSymbol.IconSymbolData.IconSymbolDataFromString(savedObjects[i]));
        //            }
        //        }
        //    }
        //    catch { }

        //    orig(self, s);
        //}

        //private static string MiscProgressionData_ToString(On.PlayerProgression.MiscProgressionData.orig_ToString orig, PlayerProgression.MiscProgressionData self)
        //{
        //    string text = orig(self);
        //    if (self.GetData() is CustomData.MiscProgressionCustomData data && data.savedObjects != null && data.savedObjects.Count > 0)
        //    {
        //        text += "BSAVEDOBJECTS<mpdB>";
        //        for (int i = 0; i < data.savedObjects.Count; i++)
        //            text += data.savedObjects[i].ToString() + (i == data.savedObjects.Count ? "" : ",");
        //        text += "<mpdA>";
        //    }
        //    return text;
        //}

        private static void MiscProgressionData_ctor(On.PlayerProgression.MiscProgressionData.orig_ctor orig, PlayerProgression.MiscProgressionData self, PlayerProgression owner)
        {
            orig(self, owner);
            if (self.GetData() is CustomData.MiscProgressionCustomData data)
                data.savedObjects = new List<IconSymbol.IconSymbolData>();
        }
    }
}
