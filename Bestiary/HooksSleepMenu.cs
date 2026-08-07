using Bestiary.BMenu;
using Menu;
using UnityEngine;

namespace Bestiary
{
    public static class HooksSleepMenu
    {
        public static void Init()
        {
            On.Menu.SleepAndDeathScreen.AddSubObjects += SleepAndDeathScreen_AddSubObjects;
            On.Menu.SleepAndDeathScreen.Update += SleepAndDeathScreen_Update;
            On.Menu.SleepAndDeathScreen.UpdateInfoText += SleepAndDeathScreen_UpdateInfoText;
            On.Menu.SleepAndDeathScreen.Singal += SleepAndDeathScreen_Singal;
        }

        private static void SleepAndDeathScreen_Singal(On.Menu.SleepAndDeathScreen.orig_Singal orig, SleepAndDeathScreen self, MenuObject sender, string message)
        {
            orig(self, sender, message);
            if (message != null && message == "BESTIARY")
            {
                self.manager.RequestMainProcessSwitch(BestiaryEnums.BestiarySleepMenu);
                self.PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);
            }
        }

        private static string SleepAndDeathScreen_UpdateInfoText(On.Menu.SleepAndDeathScreen.orig_UpdateInfoText orig, SleepAndDeathScreen self)
        {
            if (!self.RevealMap && !self.UsesWarpMap && self.selectedObject is SymbolButton button && button.signalText == "BESTIARY")
                return Plugin.Translate("Open bestiary for this campaign");
            return orig(self);
        }

        private static void SleepAndDeathScreen_Update(On.Menu.SleepAndDeathScreen.orig_Update orig, SleepAndDeathScreen self)
        {
            orig(self);
            CustomData.SleepAndDeathScreenData sData = self.GetData();
            sData.bestiaryButton.buttonBehav.greyedOut = self.ButtonsGreyedOut;
        }

        private static void SleepAndDeathScreen_AddSubObjects(On.Menu.SleepAndDeathScreen.orig_AddSubObjects orig, SleepAndDeathScreen self)
        {
            CustomData.SleepAndDeathScreenData sData = self.GetData();
            sData.bestiaryButton = new SymbolButton(self, self.pages[0], "ScholarB", "BESTIARY", new Vector2(30f, BestiaryMenu.Resolution.y * 0.9f)) { size = Vector2.one * 30f };
            sData.bestiaryButton.roundedRect.size = Vector2.one * 30f;
            self.pages[0].subObjects.Add(sData.bestiaryButton);
            orig(self);
        }
    }
}
