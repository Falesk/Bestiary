using System.Collections.Generic;
using UnityEngine;
namespace Bestiary
{
    public static class HooksGeneral
    {
        public static void Init()
        {
            On.PlayerProgression.MiscProgressionData.ctor += MiscProgressionData_ctor;
            On.InGameTranslator.LoadFonts += InGameTranslator_LoadFonts;
        }

        private static void InGameTranslator_LoadFonts(On.InGameTranslator.orig_LoadFonts orig, InGameTranslator.LanguageID lang, Menu.Menu menu)
        {
            orig(lang, menu);
            string locale = LocalizationTranslator.LangShort(lang);
            Plugin.descriptionContainer = new DescriptionContainer(locale);
        }
                                                 
        private static void MiscProgressionData_ctor(On.PlayerProgression.MiscProgressionData.orig_ctor orig, PlayerProgression.MiscProgressionData self, PlayerProgression owner)
        {
            orig(self, owner);
            if (self.GetData() is CustomData.MiscProgressionCustomData data)
                data.savedObjects = new List<IconSymbol.IconSymbolData>();
        }
    }
}
