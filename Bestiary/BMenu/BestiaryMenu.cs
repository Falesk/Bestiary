using Menu;
using UnityEngine;
using Bestiary.Buttons;

namespace Bestiary.BMenu
{
    public class BestiaryMenu : Menu.Menu
    {
        private bool _lastPauseButton, _exiting;
        private readonly FSprite _backgroundDark;
        public static int[] killScores;
        public static Vector2 Resolution => RWCustom.Custom.rainWorld.options.ScreenSize;
        public static Vector2 ResolutionOffset => 0.5f * Vector2.right * (1366f - RWCustom.Custom.rainWorld.options.ScreenSize.x);
        public SlugcatManager slugcatManager;
        public EntityManager entityManager;
        public BoxManager boxManager;
        public ButtonManager buttonManager;
        public DescriptionPage currentDescription;

        public BestiaryMenu(ProcessManager manager) : base(manager, BestiaryEnums.Bestiary)
        {
            pages.Add(new Page(this, null, "main", 0));
            scene = new InteractiveMenuScene(this, pages[0], manager.rainWorld.options.subBackground);
            pages[0].subObjects.Add(scene);
            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;

            _backgroundDark = new FSprite("pixel")
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
            pages[0].Container.AddChild(_backgroundDark);

            InitKillScores();

            slugcatManager = new SlugcatManager(this);
            slugcatManager.InitSlugcats();

            boxManager = new BoxManager(this);
            InitBoxes();
            buttonManager = new ButtonManager(this);
            InitButtons();

            entityManager = new EntityManager(this);
        }

        private void InitBoxes()
        {
            if (boxManager == null)
                return;
            Vector2 pos = new Vector2(1f / 3f, 0.1f);
            Vector2 size = 0.95f * (Vector2.one - pos);
            boxManager.CreateBox("descriptionBox", pos, size, Color.black, 0.65f);

            Vector2 pos2 = new Vector2(0.07f, 0.1f);
            Vector2 size2 = new Vector2(pos.x - 0.02f, pos.y + size.y) - pos2;
            boxManager.CreateBox("selectorBox", pos2, size2, Color.black, 0.65f);

            Vector2 p1 = pos2 + new Vector2(0.015f, 0.06f);
            Vector2 p2 = pos2 + size2 - new Vector2(0.015f, 0.04f);
            boxManager.CreateBox("entitiesBox", p1, p2 - p1, Color.black, 0f);
            boxManager.boxes["entitiesBox"].ChangeVisibility(false);
        }

        private void InitButtons()
        {
            if (buttonManager == null)
                return;
            Vector2 pos = new Vector2(boxManager.boxes["selectorBox"].normilizedPos.x + 0.02f, (boxManager.boxes["selectorBox"].normilizedPos.y - BackButton.ButtonSize.y) / 2);
            buttonManager.CreateBackButton(pos);

            Vector2 size = new Vector2(40f, 40f) / Resolution;
            float gap = 15f / Resolution.y;
            Vector2 v = boxManager.boxes["selectorBox"].normilizedPos + Vector2.up * boxManager.boxes["selectorBox"].normilizedSize.y;
            Vector2 vd = Vector2.up * (SlugcatManager.slugsInColumn * size.y + (SlugcatManager.slugsInColumn - 1) * gap);
            Vector2 firstButtonPos = v - new Vector2(+size.x + gap, 0.5f * (boxManager.boxes["selectorBox"].normilizedSize.y - vd.y) + size.y);
            Vector2 offset = new Vector2(0f, size.y + gap);

            buttonManager.CreateSlugcatButtons(firstButtonPos, offset);

            if (buttonManager.slugcatButtons.Length >= SlugcatManager.slugsInColumn)
                buttonManager.CreateSliderButtons();

            buttonManager.CreatePagerButtons();
        }

        public override void Update()
        {
            bool flag = RWInput.CheckPauseButton(0);
            if (flag && !_lastPauseButton)
                Exit();
            _lastPauseButton = flag;

            currentDescription?.UpdateImage();

            base.Update();
        }

        public void Exit()
        {
            if (_exiting)
                return;
            _exiting = true;
            manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
            PlaySound(SoundID.MENU_Switch_Page_Out);
        }

        public override string UpdateInfoText()
        {
            if (selectedObject is SimpleButton button)
            {
                if (button.signalText.Contains("SLUGCAT"))
                {
                    int index = int.Parse(button.signalText.Substring(button.signalText.LastIndexOf('_') + 1));
                    return Plugin.Translate(SlugcatStats.getSlugcatName(slugcatManager.Saves[index].name));
                }
            }
            return base.UpdateInfoText();
        }

        private static void InitKillScores()
        {
            killScores = new int[ExtEnum<MultiplayerUnlocks.SandboxUnlockID>.values.Count];
            for (int i = 0; i < killScores.Length; i++)
                killScores[i] = 0;
            SandboxSettingsInterface.DefaultKillScores(ref killScores);
        }

        public static int GetKillScore(IconSymbol.IconSymbolData symbolData)
        {
            int killID = (int)MultiplayerUnlocks.SandboxUnlockForSymbolData(symbolData);
            if (killID > -1 && killID < killScores.Length)
                return killScores[killID];
            CreatureTemplate template = StaticWorld.GetCreatureTemplate(symbolData.critType);
            CreatureTemplate ancestor = template.ancestor;
            if (ancestor != null && ancestor.type != template.TopAncestor().type)
            {
                symbolData.critType = ancestor.type;
                return GetKillScore(symbolData);
            }
            killID = (int)MultiplayerUnlocks.SandboxUnlockForSymbolData(symbolData);
            if (killID > -1 && killID < killScores.Length)
                return killScores[killID];
            return -1;
        }

        public override void Singal(MenuObject sender, string message)
        {
            base.Singal(sender, message);
            if (message == "BACK")
                buttonManager.backButton.Action();
            else if (message.Contains("SLUGCAT"))
            {
                entityManager.SetEntityPageNum(0);
                entityManager.SetSelectedEntity(-1);
                int index = int.Parse(message.Substring(message.LastIndexOf('_') + 1));
                buttonManager.slugcatButtons[index].Action();
                entityManager.UpdatePagerButtons();
                entityManager.UpdatePageLabel(true);

                currentDescription?.Clear();

                SlugcatStats.Name name = slugcatManager.Saves[slugcatManager.SelectedSlugcat].name;
                SaveState save = manager.rainWorld.progression.GetOrInitiateSaveState(name, null, manager.menuSetup, false);
                int cycles = save.cycleNumber;
                EntityManager.EntityType entityType = entityManager.EntitiesTotal > 0 || cycles > 0 ? EntityManager.EntityType.Slugcat : EntityManager.EntityType.None;
                IconSymbol.IconSymbolData icon = new IconSymbol.IconSymbolData(CreatureTemplate.Type.Slugcat, AbstractPhysicalObject.AbstractObjectType.Creature, 0);
                currentDescription = new DescriptionPage(this, icon, entityType);
            }
            else if (message.Contains("SLIDER"))
            {
                if (message.Substring(message.LastIndexOf('_') + 1) == "DOWN")
                    buttonManager.downButton.Action();
                else buttonManager.upButton.Action();
            }
            else if (message.Contains("PAGER"))
            {
                if (message.Substring(message.LastIndexOf('_') + 1) == "NEXT")
                    buttonManager.nextButton.Action();
                else buttonManager.prevButton.Action();
            }
            else if (message.Contains("ENTITY"))
            {
                int index = int.Parse(message.Substring(message.LastIndexOf('_') + 1));
                buttonManager.entityButtons[index].Action();

                currentDescription?.Clear();
                EntityManager.EntityType entityType = DefineEntityType(entityManager.GetEntityByIndex(index));
                currentDescription = new DescriptionPage(this, entityManager.GetEntityByIndex(index).iconData, entityType);
            }
        }

        public EntityManager.EntityType DefineEntityType(SaveInfo.Info info)
        {
            if (info is SaveInfo.Info.KilledInfo)
                return EntityManager.EntityType.Creature;
            if (info is SaveInfo.Info.ItemInfo)
                return EntityManager.EntityType.Item;
            return EntityManager.EntityType.None;
        }

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();
            _backgroundDark.RemoveFromContainer();
            currentDescription?.Clear();
            boxManager.Clear();
            buttonManager.Clear();
            entityManager.Clear();
            if (manager.rainWorld.options.musicVolume == 0f && manager.musicPlayer != null)
                manager.StopSideProcess(manager.musicPlayer);
        }
    }
}
