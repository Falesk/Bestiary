using Menu;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace Bestiary
{
    public static class HooksMainMenu
    {
        public static void Init()
        {
            On.Menu.MainMenu.ctor += MainMenu_ctor;
            IL.Menu.MainMenu.AddMainMenuButton += MainMenu_AddMainMenuButton;
            IL.ProcessManager.PostSwitchMainProcess += ProcessManager_PostSwitchMainProcess;
        }

        private static void ProcessManager_PostSwitchMainProcess(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    MoveType.After,
                    x => x.MatchCallOrCallvirt(typeof(MainLoopProcess).GetMethod(nameof(MainLoopProcess.ResumeProcess))),
                    x => x.MatchBr(out ILLabel _)
                    );
                c.MoveAfterLabels();
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate<Action<ProcessManager, ProcessManager.ProcessID>>((self, ID) =>
                {
                    if (ID == BestiaryEnums.Bestiary)
                        self.currentMainLoop = new BestiaryMenu(self);
                });
            }
            catch (Exception e) { Plugin.logger.LogError(e); }
        }

        private static void MainMenu_AddMainMenuButton(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    MoveType.After,
                    x => x.MatchLdfld(typeof(MenuObject).GetField(nameof(MenuObject.subObjects))),
                    x => x.MatchLdarg(1),
                    x => x.MatchCallOrCallvirt(typeof(System.Collections.Generic.List<MenuObject>).GetMethod(nameof(System.Collections.Generic.List<MenuObject>.Add))),
                    x => x.MatchLdcI4(8)
                    );
                c.MoveAfterLabels();
                c.Emit(OpCodes.Ldc_I4_1);
                c.Emit(OpCodes.Add);
            }
            catch (Exception e) { Plugin.logger.LogError(e); }
        }

        private static void MainMenu_ctor(On.Menu.MainMenu.orig_ctor orig, MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
        {
            orig(self, manager, showRegionSpecificBkg);
            float buttonWidth = MainMenu.GetButtonWidth(self.CurrLang);
            Vector2 pos = new Vector2(683f - buttonWidth / 2f, 0f);
            Vector2 size = new Vector2(buttonWidth, 30f);
            self.AddMainMenuButton(new SimpleButton(self, self.pages[0], self.Translate("BESTIARY"), "BESTIARY", pos, size), new Action(self.BestiaryButtonPressed), 2);
        }

        private static void BestiaryButtonPressed(this MainMenu self)
        {
            self.manager.RequestMainProcessSwitch(BestiaryEnums.Bestiary);
            self.PlaySound(SoundID.MENU_Switch_Page_In);
        }
    }
}
