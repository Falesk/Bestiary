using Menu;
using UnityEngine;

namespace Bestiary
{
    public class BestiaryMenu : Menu.Menu
    {
        private readonly FSprite darkSprite;
        private bool exiting;
        public SimpleButton backButton;

        public BestiaryMenu(ProcessManager manager) : base(manager, BestiaryEnums.Bestiary)
        {
            pages.Add(new Page(this, null, "main", 0));
            scene = new InteractiveMenuScene(this, pages[0], manager.rainWorld.options.subBackground);
            pages[0].subObjects.Add(scene);
            darkSprite = new FSprite("pixel")
            {
                scaleX = 1366,
                scaleY = 770,
                anchorX = 0,
                anchorY = 0,
                color = new Color(0f, 0f, 0f),
                alpha = 0.85f,
                x = -1f,
                y = -1f,
            };
            pages[0].Container.AddChild(darkSprite);
            backButton = new SimpleButton(this, pages[0], Translate("BACK"), "BACK", new Vector2(195f, 50f), new Vector2(110f, 30f));
            pages[0].subObjects.Add(backButton);
            backObject = backButton;
            backButton.nextSelectable[0] = backButton;
            backButton.nextSelectable[2] = backButton;
            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
        }

        public override void Update()
        {
            base.Update();
            if (RWInput.CheckPauseButton(0) && manager.dialog == null)
                OnExit();
        }

        public void OnExit()
        {
            if (exiting)
                return;
            exiting = true;
            manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
            PlaySound(SoundID.MENU_Switch_Page_Out);
        }

        public override void Singal(MenuObject sender, string message)
        {
            base.Singal(sender, message);
            if (message == "BACK")
                OnExit();
        }

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();
            darkSprite.RemoveFromContainer();
            if (manager.rainWorld.options.musicVolume == 0f && manager.musicPlayer != null)
                manager.StopSideProcess(manager.musicPlayer);
        }
    }
}
