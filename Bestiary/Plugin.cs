using BepInEx;
using BepInEx.Logging;
using System;
using System.IO;
using System.Linq;

namespace Bestiary
{
    [BepInPlugin(ID, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string ID = "falesk.bestiary";
        public const string Name = "Bestiary";
        public const string Version = "1.0";
        public static ManualLogSource logger;

        public void Awake()
        {
            try
            {
                On.RainWorld.OnModsInit += RainWorld_OnModsInit;
                On.RainWorld.LoadModResources += RainWorld_LoadModResources;
                On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
                On.RainWorld.UnloadResources += RainWorld_UnloadResources;
            }
            catch (Exception e) { Logger.LogError(e); }
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            logger = Logger;
            BestiaryEnums.UnregisterValues();
            BestiaryEnums.RegisterValues();
            HooksMainMenu.Init();
            HooksPauseMenu.Init();
        }

        private void RainWorld_LoadModResources(On.RainWorld.orig_LoadModResources orig, RainWorld self)
        {
            orig(self);
            string illustrationsDir = AssetManager.ResolveDirectory("crit_illustrations");
            foreach (string file in Directory.GetFiles(illustrationsDir))
            {
                string imagePath = $"crit_illustrations/{Path.GetFileNameWithoutExtension(file)}";
                string name = $"description_{Path.GetFileNameWithoutExtension(file)}";
                Futile.atlasManager.LoadImage(imagePath);
                FAtlasElement element = Futile.atlasManager.GetElementWithName(imagePath);
                element.name = name;
                Futile.atlasManager._allElementsByName.Remove(name);
                Futile.atlasManager.AddElement(element);
            }
        }

        private void RainWorld_UnloadResources(On.RainWorld.orig_UnloadResources orig, RainWorld self)
        {
            orig(self);
        }

        private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
        {
            orig(self, newlyDisabledMods);
            if (newlyDisabledMods.Any(mod => mod.id == ID))
                BestiaryEnums.UnregisterValues();
        }

        public static string Translate(string text)
        {
            string translation = RWCustom.Custom.rainWorld.inGameTranslator.Translate(text);
            if (string.IsNullOrEmpty(translation) || translation == "!NO TRANSLATION!")
            {
                string currLang = RWCustom.Custom.rainWorld.options.language.value;
                RWCustom.Custom.rainWorld.options.language = InGameTranslator.LanguageID.English;
                translation = RWCustom.Custom.rainWorld.inGameTranslator.Translate(text);
                RWCustom.Custom.rainWorld.options.language = new InGameTranslator.LanguageID(currLang);
            }
            return translation;
        }
    }
}
