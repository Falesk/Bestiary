using BepInEx;
using BepInEx.Logging;
using System;
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
                On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
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

        private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
        {
            orig(self, newlyDisabledMods);
            if (newlyDisabledMods.Any(mod => mod.id == ID))
                BestiaryEnums.UnregisterValues();
        }
    }
}
