using Menu;
using UnityEngine;

namespace Bestiary.BMenu
{
    public class BM : Menu.Menu
    {
        private bool lastPauseButton, exiting;
        private readonly FSprite backgroundDark;
        public static Vector2 Resolution => RWCustom.Custom.rainWorld.options.ScreenSize;
        public static Vector2 ResolutionOffset => 0.5f * Vector2.right * (1366f - RWCustom.Custom.rainWorld.options.ScreenSize.x);
        public BoxManager boxManager;
        public ButtonManager buttonManager;
        public SlugcatManager slugcatManager;
        public CreatureDescriptionPage currentDescription;

        public BM(ProcessManager manager) : base(manager, BestiaryEnums.Bestiary)
        {
            pages.Add(new Page(this, null, "main", 0));
            scene = new InteractiveMenuScene(this, pages[0], manager.rainWorld.options.subBackground);
            pages[0].subObjects.Add(scene);
            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;

            backgroundDark = new FSprite("pixel")
            {
                anchorX = 0,
                anchorY = 0,
                x = -1,
                y = -1,
                scaleX = Resolution.x + 2,
                scaleY = Resolution.y + 2,
                color = Color.black,
                alpha = 0.85f
            };
            pages[0].Container.AddChild(backgroundDark);

            slugcatManager = new SlugcatManager(this);

            boxManager = new BoxManager(this);
            InitBoxes();

            buttonManager = new ButtonManager(this);
            slugcatManager.InitSlugcats();
            InitButtons();
        }

        private void InitBoxes()
        {
            if (boxManager == null)
                return;
            Vector2 pos = new Vector2(1f / 3f, 0.1f);
            Vector2 size = 0.95f * (Vector2.one - pos);
            boxManager.CreateBox("descriptionBox", pos, size, Color.black, 0.65f);

            Vector2 pos2 = new Vector2(0.1f, 0.1f);
            Vector2 size2 = new Vector2(pos.x - 0.02f, pos.y + size.y) - pos2;
            boxManager.CreateBox("selectorBox", pos2, size2, Color.black, 0.65f);
        }

        private void InitButtons()
        {
            if (buttonManager == null)
                return;
            Vector2 size = new Vector2(0.07f, 0.04f);
            Vector2 pos = new Vector2(boxManager.boxes["selectorBox"].normilizedPos.x + 0.02f, (boxManager.boxes["selectorBox"].normilizedPos.y - size.y) / 2);
            buttonManager.CreateBackButton(pos, size);

            Vector2 offset = new Vector2(0f, -1.25f * SlugcatButton.ButtonSize.y);
            Vector2 pos2 = new Vector2(boxManager.boxes["selectorBox"].normilizedPos.x, boxManager.boxes["selectorBox"].normilizedPos.y + boxManager.boxes["selectorBox"].normilizedSize.y);
            pos2 = 0.5f * (pos2 + offset * SlugcatManager.slugsInColumn) + Vector2.left * 0.02f;
            buttonManager.CreateSlugcatButtons(pos2, offset);
        }

        public override void Update()
        {
            bool flag = RWInput.CheckPauseButton(0);
            if (flag && !lastPauseButton)
                Exit();
            lastPauseButton = flag;

            base.Update();
        }

        public void Exit()
        {
            if (exiting)
                return;
            exiting = true;
            manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
            PlaySound(SoundID.MENU_Switch_Page_Out);
        }

        public override string UpdateInfoText()
        {
            return base.UpdateInfoText();
        }

        public override void Singal(MenuObject sender, string message)
        {
            base.Singal(sender, message);
            if (message == "BACK")
                buttonManager.backButton.Action();
            if (message.Contains("SLUGCAT"))
            {
                int index = int.Parse(message.Substring(message.LastIndexOf('_') + 1));
                buttonManager.slugcatButtons[index].Action();
            }
        }

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();
            backgroundDark.RemoveFromContainer();
            boxManager.Clear();
            buttonManager.Clear();
            if (manager.rainWorld.options.musicVolume == 0f && manager.musicPlayer != null)
                manager.StopSideProcess(manager.musicPlayer);
        }
    }
}
