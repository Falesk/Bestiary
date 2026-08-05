using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
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
        private static bool loaded = false;

        public Dictionary<string, string> entityDescriptions;

        public void Awake()
        {
            logger = Logger;
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.RainWorld.LoadModResources += RainWorld_LoadModResources;
            On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
            On.RainWorld.UnloadResources += RainWorld_UnloadResources;
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (!loaded)
                {
                    BestiaryEnums.UnregisterValues();
                    BestiaryEnums.RegisterValues();
                    HooksMainMenu.Init();
                    HooksKillingNotify.Init();
                    loaded = true;
                }
            }
            catch (Exception e) { Logger.LogError(e); }
        }

        private void RainWorld_LoadModResources(On.RainWorld.orig_LoadModResources orig, RainWorld self)
        {
            orig(self);
            string name = "bestiaryAtlas";
            string path = "bestiary_illustrations/bestiaryAtlas";
            if (!Futile.atlasManager.DoesContainAtlas(name))
                Futile.atlasManager.ActuallyLoadAtlasOrImage(name, path, path);
        }

        private void RainWorld_UnloadResources(On.RainWorld.orig_UnloadResources orig, RainWorld self)
        {
            orig(self);
            string name = "bestiaryAtlas";
            if (Futile.atlasManager.DoesContainAtlas(name))
                Futile.atlasManager.ActuallyUnloadAtlasOrImage(name);
        }

        private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
        {
            orig(self, newlyDisabledMods);
            if (newlyDisabledMods.Any(mod => mod.id == ID))
            {
                BestiaryEnums.UnregisterValues();
                string name = "bestiaryAtlas";
                if (Futile.atlasManager.DoesContainAtlas(name))
                    Futile.atlasManager.ActuallyUnloadAtlasOrImage(name);
            }
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

        public static string ResolveCreatureName(string critType)
        {
            string name = "creaturetype-" + critType;
            if (RWCustom.Custom.rainWorld.inGameTranslator.HasShortstringTranslation(name))
                return Translate(name);
            else
            {
                CreatureTemplate template = StaticWorld.GetCreatureTemplate(new CreatureTemplate.Type(critType));
                CreatureTemplate ancestor = template.ancestor;
                if (ancestor != null && ancestor.type.value != template.TopAncestor().type.ToString())
                    return ResolveCreatureName(ancestor.type.value);
                if (ancestor != null && RWCustom.Custom.rainWorld.inGameTranslator.HasShortstringTranslation("creaturetype-" + ancestor.type.value))
                    return Translate("creaturetype-" + ancestor.type.value) + $"\n({critType})";
                return critType;
            }
        }
    }
}
