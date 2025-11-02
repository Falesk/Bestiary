using Menu;
using System.Collections.Generic;
using UnityEngine;

namespace Bestiary
{
    public class BestiaryMenu : Menu.Menu
    {
        private readonly FSprite darkSprite;
        private FSprite descriprionBoxBack, selectorBoxBack;
        private bool exiting, lastPauseButton;
        private FSprite[] slugcatSprites, entitySprites;
        private SimpleButton[] slugcatButtons, entityButtons;
        public SimpleButton backButton;
        public RoundedRect descriptionBoxBorder, selectorBoxBorder;
        public MenuLabel entityStatictic;
        public SlugcatInfo[] slugcats;
        public const float buttonSize = 40f;
        public const int buttonsInColumn = 11;
        public int choosedSlugcat, choosedEntity;

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
            backButton = new SimpleButton(this, pages[0], Translate("BACK"), "BACK", new Vector2(150f, 25f), new Vector2(110f, 30f));
            pages[0].subObjects.Add(backButton);
            backObject = backButton;
            backButton.nextSelectable[0] = backButton;
            backButton.nextSelectable[2] = backButton;
            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;

            InitBoxes();
            InitSlugcats();
            InitCreatures();

            entityStatictic = new MenuLabel(this, pages[0], "[ Choose Slugcat ]", descriprionBoxBack.GetPosition() + new Vector2(descriprionBoxBack.scaleX, descriprionBoxBack.scaleY) / 2f, Vector2.one, false);
            pages[0].subObjects.Add(entityStatictic);
        }

        public void InitCreatures()
        {
            List<SimpleButton> listEntityButtons = new List<SimpleButton>();
            List<FSprite> listEntitySprites = new List<FSprite>();

            for (int i = 0; i < CreatureTemplate.Type.values.Count; i++)
            {
                CreatureTemplate.Type type = new CreatureTemplate.Type(CreatureTemplate.Type.values.GetEntry(i));
                if (!CreatureIsKillable(type)) continue;

                float val = (selectorBoxBorder.size.x - buttonSize - 55f) / 5f;
                Vector2 pos = new Vector2(selectorBoxBorder.pos.x + buttonSize + 55f * (listEntityButtons.Count % 5) + val / 2f, selectorBoxBorder.pos.y + selectorBoxBorder.size.y - 55f * (listEntityButtons.Count / 5) - val / 2.5f) - buttonSize * Vector2.one;
                SimpleButton entityButton = new SimpleButton(this, pages[0], "", $"ENTITY_{listEntityButtons.Count}", pos, buttonSize * Vector2.one);
                entityButton.buttonBehav.greyedOut = true;
                listEntityButtons.Add(entityButton);

                FSprite entitySprite = new FSprite("Symbol_Unknown")
                {
                    color = MenuRGB(MenuColors.DarkGrey),
                    x = entityButton.pos.x + buttonSize / 2f,
                    y = entityButton.pos.y + buttonSize / 2f
                };
                listEntitySprites.Add(entitySprite);

                pages[0].subObjects.Add(entityButton);
                pages[0].Container.AddChild(entitySprite);
            }

            entityButtons = listEntityButtons.ToArray();
            entitySprites = listEntitySprites.ToArray();
        }

        public bool CreatureIsKillable(CreatureTemplate.Type type)
        {
            CreatureTemplate creatureTemplate = StaticWorld.GetCreatureTemplate(type);
            if (creatureTemplate != null)
                return !(creatureTemplate.baseDamageResistance == 0f || creatureTemplate.baseDamageResistance > 50f) || creatureTemplate.TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs;
            return false;
        }

        public void InitSlugcats()
        {
            List<SimpleButton> listSlugcatButtons = new List<SimpleButton>();
            List<FSprite> listSlugcatSprites = new List<FSprite>();
            List<SlugcatInfo> listSlugcats = new List<SlugcatInfo>();

            for (int i = 0; i < SlugcatStats.Name.values.Count; i++)
            {
                SlugcatStats.Name name = new SlugcatStats.Name(SlugcatStats.Name.values.GetEntry(i));
                if (SlugcatStats.HiddenOrUnplayableSlugcat(name)) continue;

                bool hasSave = manager.rainWorld.progression.IsThereASavedGame(name);
                SimpleButton slugButton = new SimpleButton(this, pages[0], "", $"SLUGCAT_{listSlugcatButtons.Count}", new Vector2(115f, 710f - 50f * listSlugcatButtons.Count) - buttonSize * Vector2.one, buttonSize * Vector2.one);
                slugButton.buttonBehav.greyedOut = !hasSave;
                listSlugcatButtons.Add(slugButton);

                FSprite slugSpite = new FSprite(hasSave ? "Kill_Slugcat" : "Symbol_Unknown")
                {
                    color = hasSave ? PlayerGraphics.DefaultSlugcatColor(name) : MenuRGB(MenuColors.DarkGrey),
                    x = slugButton.pos.x + buttonSize / 2f,
                    y = slugButton.pos.y + buttonSize / 2f
                };
                listSlugcatSprites.Add(slugSpite);

                SlugcatInfo slugInfo;
                if (hasSave)
                {
                    var kills = manager.rainWorld.progression.GetOrInitiateSaveState(name, null, manager.menuSetup, false).kills;
                    slugInfo = new SlugcatInfo(name, kills);
                }
                else slugInfo = new SlugcatInfo(name);
                listSlugcats.Add(slugInfo);

                pages[0].subObjects.Add(slugButton);
                pages[0].Container.AddChild(slugSpite);
            }

            slugcatButtons = listSlugcatButtons.ToArray();
            slugcatSprites = listSlugcatSprites.ToArray();
            slugcats = listSlugcats.ToArray();
        }

        public void InitBoxes()
        {
            descriptionBoxBorder = new RoundedRect(this, pages[0], new Vector2(455f, 75f), new Vector2(870f, 650f), false);
            selectorBoxBorder = new RoundedRect(this, pages[0], new Vector2(130f, 75f), new Vector2(300f, 650f), false);
            pages[0].subObjects.Add(descriptionBoxBorder);
            pages[0].subObjects.Add(selectorBoxBorder);

            descriprionBoxBack = new FSprite("pixel")
            {
                color = new Color(0f, 0f, 0f),
                scaleX = descriptionBoxBorder.size.x - 12f,
                scaleY = descriptionBoxBorder.size.y - 12f,
                x = descriptionBoxBorder.pos.x + 6f - (1366f - manager.rainWorld.options.ScreenSize.x) / 2f,
                y = descriptionBoxBorder.pos.y + 6f,
                alpha = 0.65f
            };
            descriprionBoxBack.SetAnchor(0f, 0f);
            infoLabel.x = Mathf.Ceil(descriprionBoxBack.x + descriprionBoxBack.scaleX / 2f);

            selectorBoxBack = new FSprite("pixel")
            {
                color = new Color(0f, 0f, 0f),
                scaleX = selectorBoxBorder.size.x - 12f,
                scaleY = selectorBoxBorder.size.y - 12f,
                x = selectorBoxBorder.pos.x + 6f - (1366f - manager.rainWorld.options.ScreenSize.x) / 2f,
                y = selectorBoxBorder.pos.y + 6f,
                alpha = 0.65f
            };
            selectorBoxBack.SetAnchor(0f, 0f);
        }

        public void UpdateEntitiesWithInfo(SlugcatInfo info)
        {
            if (info.kills == null) return;
            for (int i = 0; i < entityButtons.Length; i++)
            {
                if (i >= info.kills.Count)
                {
                    entityButtons[i].buttonBehav.greyedOut = true;
                    entitySprites[i].element = Futile.atlasManager.GetElementWithName("Symbol_Unknown");
                    entitySprites[i].color = MenuRGB(MenuColors.DarkGrey);
                }
                else
                {
                    entityButtons[i].buttonBehav.greyedOut = false;
                    entitySprites[i].element = Futile.atlasManager.GetElementWithName(CreatureSymbol.SpriteNameOfCreature(info.kills[i].Key));
                    entitySprites[i].color = CreatureSymbol.ColorOfCreature(info.kills[i].Key);
                }
            }
        }

        public override void Update()
        {
            bool flag = RWInput.CheckPauseButton(0);
            if (flag && !lastPauseButton && manager.dialog == null)
                OnExit();
            lastPauseButton = flag;
            base.Update();
        }

        public override string UpdateInfoText()
        {
            if (selectedObject is SimpleButton button)
            {
                if (button.signalText.Contains("SLUGCAT"))
                {
                    int index = int.Parse(button.signalText.Substring(button.signalText.LastIndexOf('_') + 1));
                    return Translate(SlugcatStats.getSlugcatName(slugcats[index].name));
                }
            }
            return base.UpdateInfoText();
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
            if (message.Contains("SLUGCAT"))
            {
                choosedSlugcat = int.Parse(message.Substring(message.LastIndexOf('_') + 1));
                if (slugcats[choosedSlugcat].kills != null)
                {
                    entityStatictic.text = "[ Choose Entity ]";
                    UpdateEntitiesWithInfo(slugcats[choosedSlugcat]);
                }
                else entityStatictic.text = "Nothing to load";
                for (int i = 0; i < slugcatButtons.Length; i++)
                    slugcatButtons[i].toggled = false;
                for (int i = 0; i < entityButtons.Length; i++)
                    entityButtons[i].toggled = false;
                slugcatButtons[choosedSlugcat].toggled = true;
            }
            if (message.Contains("ENTITY"))
            {
                choosedEntity = int.Parse(message.Substring(message.LastIndexOf('_') + 1));
                var critInfo = slugcats[choosedSlugcat].kills[choosedEntity];
                entityStatictic.text = $"{critInfo.Key.critType} - {critInfo.Value} kills";
                for (int i = 0; i < entityButtons.Length; i++)
                    entityButtons[i].toggled = false;
                entityButtons[choosedEntity].toggled = true;
            }
        }

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();
            darkSprite.RemoveFromContainer();
            descriprionBoxBack.RemoveFromContainer();
            selectorBoxBack.RemoveFromContainer();
            for (int i = 0; i < slugcatSprites.Length; i++)
                slugcatSprites[i].RemoveFromContainer();
            for (int i = 0; i < entitySprites.Length; i++)
                entitySprites[i].RemoveFromContainer();
            if (manager.rainWorld.options.musicVolume == 0f && manager.musicPlayer != null)
                manager.StopSideProcess(manager.musicPlayer);
        }

        public struct SlugcatInfo
        {
            public SlugcatStats.Name name;
            public List<KeyValuePair<IconSymbol.IconSymbolData, int>> kills;

            public SlugcatInfo(SlugcatStats.Name _name, List<KeyValuePair<IconSymbol.IconSymbolData, int>> _kills)
            {
                name = _name;
                kills = _kills;
            }

            public SlugcatInfo(SlugcatStats.Name _name)
            {
                name = _name;
                kills = null;
            }
        }
    }
}
