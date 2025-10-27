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
                On.World.ctor += World_ctor;
            }
            catch (Exception e) { Logger.LogError(e); }
        }

        private void World_ctor(On.World.orig_ctor orig, World self, RainWorldGame game, Region region, string name, bool singleRoomWorld)
        {
            orig(self, game, region, name, singleRoomWorld);
            UnityEngine.Debug.Log("==== saveState Kills ====");
            for (int i = 0; i < game.GetStorySession.saveState.kills.Count; i++)
                UnityEngine.Debug.Log($"{game.GetStorySession.saveState.kills[i].Key} - {game.GetStorySession.saveState.kills[i].Value}");
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
