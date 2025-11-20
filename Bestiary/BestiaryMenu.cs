using Menu;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Bestiary
{
    public class BestiaryMenu : Menu.Menu
    {
        private readonly bool debug = false;
        private readonly FSprite darkSprite;
        private FSprite descriprionBoxBack, selectorBoxBack, slugcatSliderUp, slugcatSliderDown, entityPagerNext, entityPagerPrev, filterSprite, sortingSprite;
        private bool exiting, lastPauseButton;
        private FSprite[] slugcatSprites, entitySprites;
        private SimpleButton[] slugcatButtons, entityButtons;
        public SimpleButton backButton, slugcatSliderUpButton, slugcatSliderDownButton, entityPagerNextButton, entityPagerPrevButton, filterButton, sortingButton;
        public RoundedRect descriptionBoxBorder, selectorBoxBorder;
        public CreatureDescriptionPage currentDescription;
        public MenuLabel emptinessLabel, pageLabel;
        public SlugcatInfo[] slugcats;
        public Dictionary<string, CreatureDescriptionPage.Characteristic> characteristics;
        public const float buttonSize = 40f;
        public const int buttonsInColumn = 8, slugsInColumn = 11;
        public int choosedSlugcat, choosedEntity, slugcatSlideNum, entityPageNum;
        private int[] killScores;

        public BestiaryMenu(ProcessManager manager) : base(manager, BestiaryEnums.Bestiary)
        {
            pages.Add(new Page(this, null, "main", 0));
            scene = new InteractiveMenuScene(this, pages[0], manager.rainWorld.options.subBackground);
            pages[0].subObjects.Add(scene);
            darkSprite = new FSprite("pixel")
            {
                scaleX = manager.rainWorld.screenSize.x + 2,
                scaleY = manager.rainWorld.screenSize.y + 2,
                anchorX = 0,
                anchorY = 0,
                color = new Color(0f, 0f, 0f),
                alpha = 0.85f,
                x = -1f,
                y = -1f,
            };
            pages[0].Container.AddChild(darkSprite);
            backButton = new SimpleButton(this, pages[0], Plugin.Translate("BACK"), "BACK", new Vector2(150f, 25f), new Vector2(110f, 30f));
            pages[0].subObjects.Add(backButton);
            backObject = backButton;
            backButton.nextSelectable[0] = backButton;
            backButton.nextSelectable[2] = backButton;
            backButton.nextSelectable[3] = backButton;

            choosedEntity = -1;
            choosedSlugcat = -1;

            InitKillScores();
            InitBoxes();
            InitSlugcats();
            InitSPButtons();
            InitUpperPanel();

            emptinessLabel = new MenuLabel(this, pages[0], Plugin.Translate("[ Choose Slugcat ]"), descriprionBoxBack.GetPosition() + new Vector2(descriprionBoxBack.scaleX, descriprionBoxBack.scaleY) / 2f, Vector2.one, false);
            pages[0].subObjects.Add(emptinessLabel);
            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
        }

        public void InitSPButtons()
        {
            float btnSize = 25;
            if (slugcatButtons.Length > slugsInColumn)
            {
                slugcatSlideNum = 0;

                slugcatSliderUpButton = new SimpleButton(this, pages[0], string.Empty, "SLIDER_UP", new Vector2(118f - buttonSize / 4f, 720f) - btnSize * Vector2.one, btnSize * Vector2.one);
                slugcatSliderUp = new FSprite("Menu_Symbol_Arrow")
                {
                    color = MenuRGB(MenuColors.DarkGrey),
                    x = slugcatSliderUpButton.pos.x + btnSize / 2f,
                    y = slugcatSliderUpButton.pos.y + btnSize / 2f
                };
                slugcatSliderUpButton.buttonBehav.greyedOut = true;
                pages[0].subObjects.Add(slugcatSliderUpButton);
                pages[0].Container.AddChild(slugcatSliderUp);

                slugcatSliderDownButton = new SimpleButton(this, pages[0], string.Empty, "SLIDER_DOWN", new Vector2(118f - buttonSize / 4f, 105f) - btnSize * Vector2.one, btnSize * Vector2.one);
                slugcatSliderDown = new FSprite("Menu_Symbol_Arrow")
                {
                    color = MenuRGB(MenuColors.White),
                    x = slugcatSliderDownButton.pos.x + btnSize / 2f,
                    y = slugcatSliderDownButton.pos.y + btnSize / 2f,
                    rotation = 180f
                };
                pages[0].subObjects.Add(slugcatSliderDownButton);
                pages[0].Container.AddChild(slugcatSliderDown);
            }
        }

        public void InitPagerButtons()
        {
            float btnSize = 25;
            if (entityButtons.Length > 4 * buttonsInColumn)
            {
                if (entityPagerNextButton != null)
                {
                    pages[0].RemoveSubObject(entityPagerNextButton);
                    entityPagerNextButton.RemoveSprites();
                    pages[0].RemoveSubObject(entityPagerPrevButton);
                    entityPagerPrevButton.RemoveSprites();
                    entityPagerNext.RemoveFromContainer();
                    entityPagerPrev.RemoveFromContainer();
                }

                entityPageNum = 0;

                entityPagerNextButton = new SimpleButton(this, pages[0], string.Empty, "PAGER_NEXT", new Vector2(400f, 100f) - btnSize * Vector2.one / 2f, btnSize * Vector2.one);
                entityPagerNext = new FSprite("Menu_Symbol_Arrow")
                {
                    color = MenuRGB(MenuColors.DarkGrey),
                    x = entityPagerNextButton.pos.x + btnSize / 2f,
                    y = entityPagerNextButton.pos.y + btnSize / 2f,
                    rotation = 90f
                };
                entityPagerNextButton.buttonBehav.greyedOut = true;
                pages[0].subObjects.Add(entityPagerNextButton);
                pages[0].Container.AddChild(entityPagerNext);

                entityPagerPrevButton = new SimpleButton(this, pages[0], string.Empty, "PAGER_PREV", new Vector2(360f, 100f) - btnSize * Vector2.one / 2f, btnSize * Vector2.one);
                entityPagerPrev = new FSprite("Menu_Symbol_Arrow")
                {
                    color = MenuRGB(MenuColors.DarkGrey),
                    x = entityPagerPrevButton.pos.x + btnSize / 2f,
                    y = entityPagerPrevButton.pos.y + btnSize / 2f,
                    rotation = -90f
                };
                entityPagerPrevButton.buttonBehav.greyedOut = true;
                pages[0].subObjects.Add(entityPagerPrevButton);
                pages[0].Container.AddChild(entityPagerPrev);
            }
        }

        public void InitCreatures(SlugcatInfo slugcat)
        {
            if (entityButtons != null)
            {
                for (int i = 0; i < entityButtons.Length; i++)
                {
                    pages[0].RemoveSubObject(entityButtons[i]);
                    entityButtons[i].RemoveSprites();
                    entitySprites[i].RemoveFromContainer();
                }
            }

            entityButtons = new SimpleButton[slugcat.kills.Count];
            entitySprites = new FSprite[slugcat.kills.Count];
            characteristics = new Dictionary<string, CreatureDescriptionPage.Characteristic>();

            for (int i = 0; i < entityButtons.Length; i++)
            {
                float critButtonSize = buttonSize + 12f;
                entityButtons[i] = new SimpleButton(this, pages[0], string.Empty, $"ENTITY_{i}", Vector2.zero, critButtonSize * Vector2.one);

                FSprite entitySprite = new FSprite(Futile.atlasManager.GetElementWithName(CreatureSymbol.SpriteNameOfCreature(slugcat.kills[i].iconData)))
                { color = CreatureSymbol.ColorOfCreature(slugcat.kills[i].iconData) };
                entitySprites[i] = entitySprite;

                CreatureTemplate cTemplate = StaticWorld.GetCreatureTemplate(slugcat.kills[i].iconData.critType);
                CreatureDescriptionPage.Characteristic characteristic = new CreatureDescriptionPage.Characteristic()
                {
                    hp = cTemplate.baseDamageResistance,
                    foodPoints = cTemplate.meatPoints,
                    score = GetKillScore(slugcat.kills[i].iconData),
                    kills = slugcat.kills[i].kills,
                    behaviour = cTemplate.relationships[CreatureTemplate.Type.Slugcat.Index].type
                };
                if (cTemplate.breedParameters is LizardBreedParams breedParams)
                {
                    characteristic.damage = breedParams.biteDamage;
                    characteristic.biteChance = breedParams.biteDamageChance;
                }
                if (!characteristics.ContainsKey(slugcat.kills[i].iconData.critType.value + slugcat.kills[i].iconData.intData.ToString()))
                    characteristics.Add(slugcat.kills[i].iconData.critType.value + slugcat.kills[i].iconData.intData.ToString(), characteristic);

                pages[0].subObjects.Add(entityButtons[i]);
                pages[0].Container.AddChild(entitySprites[i]);
            }

            InitPagerButtons();
            RefreshEntities();
        }

        private void InitKillScores()
        {
            killScores = new int[ExtEnum<MultiplayerUnlocks.SandboxUnlockID>.values.Count];
            for (int i = 0; i < killScores.Length; i++)
                killScores[i] = 0;
            SandboxSettingsInterface.DefaultKillScores(ref killScores);
        }

        public void InitUpperPanel()
        {
            Vector2 pos = new Vector2(selectorBoxBorder.pos.x + 140f, selectorBoxBorder.pos.y + 25f);
            pageLabel = new MenuLabel(this, pages[0], string.Empty, pos, Vector2.one, false);
            pageLabel.label.alignment = FLabelAlignment.Right;
            pages[0].subObjects.Add(pageLabel);

            pos = new Vector2(pos.x - 100f, pos.y + selectorBoxBorder.size.y - 65f);
            filterButton = new SimpleButton(this, pages[0], string.Empty, "FILTER", pos, 25f * Vector2.one);
            sortingButton = new SimpleButton(this, pages[0], string.Empty, "SORTING", pos + 40f * Vector2.right, 25f * Vector2.one);
            pages[0].subObjects.Add(filterButton);
            pages[0].subObjects.Add(sortingButton);

            filterSprite = new FSprite("Menu_Symbol_Show_List")
            {
                color = MenuRGB(MenuColors.White),
                x = filterButton.pos.x + 12.5f,
                y = filterButton.pos.y + 12.5f
            };
            pages[0].Container.AddChild(filterSprite);

            sortingSprite = new FSprite("Menu_Symbol_Shuffle")
            {
                color = MenuRGB(MenuColors.White),
                x = sortingButton.pos.x + 12.5f,
                y = sortingButton.pos.y + 12.5f
            };
            pages[0].Container.AddChild(sortingSprite);
            UpdateUpperPanel(true);
        }

        public void UpdateUpperPanel(bool hide = false)
        {
            pageLabel.text = hide || entityButtons.Length / (4 * buttonsInColumn) == 0 ? string.Empty : 
                Plugin.Translate("Page $ of %")
                .Replace("$", (entityPageNum + 1).ToString())
                .Replace("%", (entityButtons.Length / (4 * buttonsInColumn) + 1)
                .ToString());
            filterSprite.isVisible = !hide;
            sortingSprite.isVisible = !hide;
            ToggleButtonVisibility(filterButton, !hide);
            ToggleButtonVisibility(sortingButton, !hide);
        }

        private void ToggleButtonVisibility(SimpleButton button, bool isVisible)
        {
            for (int i = 0; i < button.roundedRect.sprites.Length; i++)
                button.roundedRect.sprites[i].isVisible = isVisible;
            for (int i = 0; i < button.selectRect.sprites.Length; i++)
                button.selectRect.sprites[i].isVisible = isVisible;
        }

        public bool CreatureIsKillable(CreatureTemplate.Type type)
        {
            CreatureTemplate creatureTemplate = StaticWorld.GetCreatureTemplate(type);
            bool creatureTemplateFlag = false;
            if (creatureTemplate != null)
                creatureTemplateFlag = !(creatureTemplate.baseDamageResistance == 0f || creatureTemplate.baseDamageResistance > 50f || creatureTemplate.TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs);
            return CreatureSymbol.DoesCreatureEarnATrophy(type) &&
                creatureTemplateFlag &&
                CreatureSymbol.SpriteNameOfCreature(new IconSymbol.IconSymbolData(type, AbstractPhysicalObject.AbstractObjectType.Creature, 0)) != "Futile_White" &&
                type != CreatureTemplate.Type.Slugcat && type != MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC;
        }

        public void InitSlugcats()
        {
            List<SimpleButton> listSlugcatButtons = new List<SimpleButton>();
            List<FSprite> listSlugcatSprites = new List<FSprite>();
            List<SlugcatInfo> listSlugcats = new List<SlugcatInfo>();
            bool debugOpenAll = debug;

            for (int i = 0; i < SlugcatStats.Name.values.Count; i++)
            {
                SlugcatStats.Name name = new SlugcatStats.Name(SlugcatStats.Name.values.GetEntry(i));
                if (SlugcatStats.HiddenOrUnplayableSlugcat(name)) continue;

                bool hasSave = manager.rainWorld.progression.IsThereASavedGame(name);
                SimpleButton slugButton = new SimpleButton(this, pages[0], string.Empty, $"SLUGCAT_{listSlugcatButtons.Count}", new Vector2(115f, 670f - 50f * listSlugcatButtons.Count) - buttonSize * Vector2.one, buttonSize * Vector2.one);
                slugButton.buttonBehav.greyedOut = !hasSave;
                listSlugcatButtons.Add(slugButton);

                FSprite slugSpite = new FSprite(hasSave ? "Kill_Slugcat" : "Sandbox_SmallQuestionmark")
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
                    List<SlugcatInfo.KilledInfo> killedInfo = new List<SlugcatInfo.KilledInfo>();
                    for (int j = 0; j < kills.Count; j++)
                        killedInfo.Add(SlugcatInfo.KilledInfo.Transform(kills[j]));
                    if (debugOpenAll)
                    {
                        for (int j = 0; j < CreatureTemplate.Type.values.Count; j++)
                        {
                            CreatureTemplate.Type type = new CreatureTemplate.Type(CreatureTemplate.Type.values.GetEntry(j));
                            if (!killedInfo.Contains(killedInfo.FirstOrDefault(x => x.iconData.critType == type))/* && CreatureIsKillable(type)*/ && type.value != "BabyLizard")
                                killedInfo.Add(new SlugcatInfo.KilledInfo { iconData = new IconSymbol.IconSymbolData(type, AbstractPhysicalObject.AbstractObjectType.Creature, 0), kills = 0 });
                        }
                    }
                    slugInfo = new SlugcatInfo(name, killedInfo);
                }
                else slugInfo = new SlugcatInfo(name);
                listSlugcats.Add(slugInfo);

                pages[0].subObjects.Add(slugButton);
                pages[0].Container.AddChild(slugSpite);
            }

            slugcatButtons = listSlugcatButtons.ToArray();
            slugcatSprites = listSlugcatSprites.ToArray();
            slugcats = listSlugcats.ToArray();
            RefreshSlugcats();
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
                x = descriptionBoxBorder.pos.x + 6f,
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
                x = selectorBoxBorder.pos.x + 6f,
                y = selectorBoxBorder.pos.y + 6f,
                alpha = 0.65f
            };
            selectorBoxBack.SetAnchor(0f, 0f);
        }

        public void RefreshSlugcats()
        {
            bool flag = slugcatButtons.Length <= slugsInColumn;
            for (int i = 0; i < slugcatButtons.Length; i++)
            {
                slugcatButtons[i].buttonBehav.greyedOut = i < slugcatSlideNum || i - slugcatSlideNum >= slugsInColumn || slugcats[i].kills == null;
                ToggleButtonVisibility(slugcatButtons[i], !slugcatButtons[i].buttonBehav.greyedOut);
                slugcatButtons[i].pos = new Vector2(115f, (flag ? 710f : 670f) + 50f * (slugcatSlideNum - i)) - buttonSize * Vector2.one;
                slugcatSprites[i].SetPosition(slugcatButtons[i].pos + (buttonSize / 2f) * Vector2.one);
                slugcatSprites[i].alpha = slugcatButtons[i].buttonBehav.greyedOut ? 0f : 1f;
            }
        }

        public void RefreshEntities()
        {
            for (int i = 0; i < entityButtons.Length; i++)
            {
                entityButtons[i].buttonBehav.greyedOut = !(i >= buttonsInColumn * 4 * entityPageNum && i < buttonsInColumn * 4 * (entityPageNum + 1));
                for (int j = 0; j < entityButtons[i].roundedRect.sprites.Length; j++)
                    entityButtons[i].roundedRect.sprites[j].isVisible = !entityButtons[i].buttonBehav.greyedOut;
                float critButtonSize = buttonSize + 12f;
                float val = (selectorBoxBorder.size.x - critButtonSize - 67f) / 4f;
                Vector2 pos = new Vector2(selectorBoxBorder.pos.x + critButtonSize + 67f * (i % 4) + val / 2f, selectorBoxBorder.pos.y + selectorBoxBorder.size.y - 67f * ((i / 4) % buttonsInColumn + 0.5f) - val / 2.5f) - critButtonSize * Vector2.one;
                entityButtons[i].pos = pos;

                entitySprites[i].SetPosition(entityButtons[i].pos + (critButtonSize / 2f) * Vector2.one);
                entitySprites[i].alpha = entityButtons[i].buttonBehav.greyedOut ? 0f : 1f;

                entityButtons[i].buttonBehav.greyedOut = entityButtons[i].buttonBehav.greyedOut || choosedSlugcat == -1;
            }
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
                    entityButtons[i].buttonBehav.greyedOut = entityButtons[i].buttonBehav.greyedOut || choosedSlugcat == -1;
                    entitySprites[i].element = Futile.atlasManager.GetElementWithName(CreatureSymbol.SpriteNameOfCreature(info.kills[i].iconData));
                    entitySprites[i].color = CreatureSymbol.ColorOfCreature(info.kills[i].iconData);
                }
            }
        }

        public override void Update()
        {
            bool flag = RWInput.CheckPauseButton(0);
            if (flag && !lastPauseButton && manager.dialog == null)
                OnExit();
            lastPauseButton = flag;
            UpdateSelectables();

            base.Update();
        }

        public void UpdateSelectables()
        {
            if (!(entityButtons != null && entityButtons.Length > 4))
                return;
            // 0 == left, 1 == up, 2 == right, 3 == down
            for (int i = 0; i < entityButtons.Length; i++)
            {
                int slugInd = (int)Mathf.Lerp(0f, slugsInColumn, Mathf.InverseLerp(0f, buttonsInColumn, i / 4 % buttonsInColumn)) + slugcatSlideNum;
                entityButtons[i].nextSelectable[0] = (i % 4 == 0) ? slugcatButtons[System.Math.Min(slugInd, slugcatButtons.Length - 1)] : entityButtons[i - 1];
                entityButtons[i].nextSelectable[1] = (i % (4 * buttonsInColumn) < 4) ? entityButtons[i] : entityButtons[i - 4];
                entityButtons[i].nextSelectable[2] = (i % (4 * buttonsInColumn) == 4 * buttonsInColumn - 1 || i == entityButtons.Length - 1) ? (entityPagerNextButton ?? backButton) : entityButtons[i + 1];
                entityButtons[i].nextSelectable[3] = (i % (4 * buttonsInColumn) > 4 * buttonsInColumn - 5 || i > entityButtons.Length - 5) ? (entityPagerNextButton ?? backButton) : entityButtons[i + 4];
            }
            if (entityPagerNextButton != null)
            {
                entityPagerNextButton.nextSelectable[0] = entityPagerPrevButton;
                entityPagerNextButton.nextSelectable[1] = entityButtons[System.Math.Min(4 * buttonsInColumn * (entityPageNum + 1), entityButtons.Length) - 1];
                entityPagerNextButton.nextSelectable[2] = entityPagerPrevButton;
                entityPagerNextButton.nextSelectable[3] = backButton;
                entityPagerPrevButton.nextSelectable[0] = entityPagerNextButton;
                entityPagerPrevButton.nextSelectable[1] = entityPagerNextButton.nextSelectable[1];
                entityPagerPrevButton.nextSelectable[2] = entityPagerNextButton;
                entityPagerPrevButton.nextSelectable[3] = backButton;
                backButton.nextSelectable[1] = entityPagerNextButton.nextSelectable[1];
            }
            for (int i = 0; i < slugcatButtons.Length; i++)
            {
                slugcatButtons[i].nextSelectable[0] = slugcatButtons[i];
                int entInd = 4 * (int)Mathf.Lerp(0f, buttonsInColumn, Mathf.InverseLerp(0f, slugsInColumn, i - slugcatSlideNum)) + 4 * buttonsInColumn * entityPageNum;
                slugcatButtons[i].nextSelectable[2] = entityButtons[System.Math.Min(entInd, entityButtons.Length - 1)];
            }
        }

        public override string UpdateInfoText()
        {
            if (selectedObject is SimpleButton button)
            {
                if (button.signalText.Contains("SLUGCAT"))
                {
                    int index = int.Parse(button.signalText.Substring(button.signalText.LastIndexOf('_') + 1));
                    return Plugin.Translate(SlugcatStats.getSlugcatName(slugcats[index].name));
                }
                if (button.signalText.Contains("FILTER"))
                    return Plugin.Translate("Creature filter");
                if (button.signalText.Contains("SORTING"))
                    return Plugin.Translate("Creature sorting");
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

        public void RefreshEmptinessLabel(bool show)
        {
            if (show)
            {
                if (slugcats[choosedSlugcat].kills?.Count > 0)
                    emptinessLabel.text = Plugin.Translate("[ Choose Entity ]");
                else emptinessLabel.text = "[ Nothing to load ]";
            }
            else emptinessLabel.text = string.Empty;
        }

        public override void Singal(MenuObject sender, string message)
        {
            base.Singal(sender, message);
            if (message == "BACK")
                OnExit();
            bool setPageAtZero = false;
            if (message.Contains("SLUGCAT"))
            {
                PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);
                choosedSlugcat = int.Parse(message.Substring(message.LastIndexOf('_') + 1));
                if (slugcats[choosedSlugcat].kills != null)
                {
                    InitCreatures(slugcats[choosedSlugcat]);
                    UpdateEntitiesWithInfo(slugcats[choosedSlugcat]);
                }
                UpdateUpperPanel(slugcats[choosedSlugcat].kills == null);
                currentDescription?.Clear();
                RefreshEmptinessLabel(true);
                for (int i = 0; i < slugcatButtons.Length; i++)
                    slugcatButtons[i].toggled = false;
                for (int i = 0; i < entityButtons.Length; i++)
                    entityButtons[i].toggled = false;
                slugcatButtons[choosedSlugcat].toggled = true;
                setPageAtZero = true;
                entityPageNum = 0;
                choosedEntity = -1;
            }
            if (message.Contains("ENTITY"))
            {
                PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);
                choosedEntity = int.Parse(message.Substring(message.LastIndexOf('_') + 1));
                for (int i = 0; i < entityButtons.Length; i++)
                    entityButtons[i].toggled = false;
                entityButtons[choosedEntity].toggled = true;
                if (choosedSlugcat != -1)
                {
                    SlugcatInfo.KilledInfo critInfo = slugcats[choosedSlugcat].kills[choosedEntity];
                    currentDescription?.Clear();

                    currentDescription = new CreatureDescriptionPage(this, critInfo.iconData)
                    { characteristic = characteristics[critInfo.iconData.critType.value + critInfo.iconData.intData.ToString()] };
                    currentDescription.GenerateCharacteristicLabels();

                    RefreshEmptinessLabel(false);
                }
            }
            if (message.Contains("SLIDER"))
            {
                if (message.Substring(message.LastIndexOf('_') + 1) == "DOWN" && slugcatSlideNum + slugsInColumn <= slugcatButtons.Length)
                    slugcatSlideNum += slugcatSlideNum + slugsInColumn >= slugcatButtons.Length ? 0 : 1;
                else slugcatSlideNum -= (slugcatSlideNum == 0) ? 0 : 1;
                bool flag = slugcatSlideNum + slugsInColumn >= slugcatButtons.Length;
                slugcatSliderDownButton.buttonBehav.greyedOut = flag;
                slugcatSliderDown.color = flag ? MenuRGB(MenuColors.DarkGrey) : MenuRGB(MenuColors.White);
                slugcatSliderUpButton.buttonBehav.greyedOut = slugcatSlideNum == 0;
                slugcatSliderUp.color = slugcatSlideNum == 0 ? MenuRGB(MenuColors.DarkGrey) : MenuRGB(MenuColors.White);
                RefreshSlugcats();
            }
            if (message.Contains("PAGER") || (setPageAtZero && entityPagerNextButton != null))
            {
                if (message.Substring(message.LastIndexOf('_') + 1) == "NEXT" && (entityPageNum + 1) * 4 * buttonsInColumn < entityButtons.Length)
                    entityPageNum++;
                else entityPageNum -= (entityPageNum == 0) ? 0 : 1;
                bool flag = (entityPageNum + 1) * 4 * buttonsInColumn > entityButtons.Length;
                entityPagerNextButton.buttonBehav.greyedOut = flag;
                entityPagerNext.color = flag ? MenuRGB(MenuColors.DarkGrey) : MenuRGB(MenuColors.White);
                entityPagerPrevButton.buttonBehav.greyedOut = entityPageNum == 0;
                entityPagerPrev.color = entityPageNum == 0 ? MenuRGB(MenuColors.DarkGrey) : MenuRGB(MenuColors.White);
                UpdateUpperPanel();
                if (choosedSlugcat != -1)
                    RefreshEntities();
                if (choosedEntity != -1)
                    entityButtons[choosedEntity].toggled = choosedEntity > entityPageNum * 4 * buttonsInColumn && entityPageNum <= (entityPageNum + 1) * 4 * buttonsInColumn;
            }
        }

        private int GetKillScore(IconSymbol.IconSymbolData symbolData)
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

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();
            darkSprite.RemoveFromContainer();
            descriprionBoxBack.RemoveFromContainer();
            selectorBoxBack.RemoveFromContainer();
            slugcatSliderUp?.RemoveFromContainer();
            slugcatSliderDown?.RemoveFromContainer();
            entityPagerNext?.RemoveFromContainer();
            entityPagerPrev?.RemoveFromContainer();
            filterSprite?.RemoveFromContainer();
            sortingSprite?.RemoveFromContainer();
            currentDescription?.Clear();
            for (int i = 0; i < slugcatSprites.Length; i++)
                slugcatSprites[i].RemoveFromContainer();
            for (int i = 0; i < entitySprites?.Length; i++)
                entitySprites[i].RemoveFromContainer();
            if (manager.rainWorld.options.musicVolume == 0f && manager.musicPlayer != null)
                manager.StopSideProcess(manager.musicPlayer);
        }

        public class CreatureDescriptionPage
        {
            private readonly string name;
            private readonly BestiaryMenu menu;
            private readonly RoundedRect imageBox;
            private readonly FSprite icon, image;
            private readonly MenuLabel entityName, entityDescriptionLabel;
            private MenuLabel[] entityDescription, entityCharacteristicLabels;
            public Characteristic characteristic;

            public CreatureDescriptionPage(BestiaryMenu owner, IconSymbol.IconSymbolData iconData)
            {
                menu = owner;
                name = iconData.critType.ToString();
                icon = new FSprite(CreatureSymbol.SpriteNameOfCreature(iconData))
                {
                    color = CreatureSymbol.ColorOfCreature(iconData),
                    scale = 2f
                };
                icon.SetPosition(menu.descriprionBoxBack.GetPosition() + new Vector2(50f, menu.descriprionBoxBack.scaleY - 50f));
                menu.pages[0].Container.AddChild(icon);

                entityName = new MenuLabel(menu, menu.pages[0], ResolveName(name), menu.descriprionBoxBack.GetPosition() + new Vector2(100f, menu.descriprionBoxBack.scaleY - 50f), Vector2.one, true);
                entityName.label.alignment = FLabelAlignment.Left;
                menu.pages[0].subObjects.Add(entityName);

                Vector2 descrPos = menu.descriprionBoxBack.GetPosition() + menu.descriprionBoxBack.scaleX / 2f * Vector2.right + menu.descriprionBoxBack.scaleY / 2.2f * Vector2.up;
                entityDescriptionLabel = new MenuLabel(menu, menu.pages[0], Plugin.Translate("b-Description"), descrPos, Vector2.one, true);
                menu.pages[0].subObjects.Add(entityDescriptionLabel);

                Vector2 boxSize = new Vector2(480f, 270f);
                Vector2 boxPos = menu.descriptionBoxBorder.pos + menu.descriptionBoxBorder.size - 30f * Vector2.one - boxSize;
                imageBox = new RoundedRect(menu, menu.pages[0], boxPos, boxSize, true);
                for (int i = 0; i < imageBox.SideSprite(0); i++)
                    imageBox.sprites[i].color = new Color(0.6f, 0.6f, 0.6f);
                imageBox.fillAlpha = 0.65f;
                menu.pages[0].subObjects.Add(imageBox);

                string imageName = $"description_{name.ToLower()}";
                if (Futile.atlasManager._allElementsByName.TryGetValue(imageName, out FAtlasElement element))
                {
                    image = new FSprite(element);
                    image.scale = Mathf.Min(0.8f * boxSize.x / image.element.sourceSize.x, 0.8f * boxSize.y / image.element.sourceSize.y);
                }
                else image = new FSprite("Sandbox_QuestionMark") { color = MenuRGB(MenuColors.Black) };
                image.SetPosition(boxPos + boxSize / 2f);
                menu.pages[0].Container.AddChild(image);
                
                GetDescription();
            }

            private void GetDescription()
            {
                string[] lines;
                string path = AssetManager.ResolveFilePath($"{RWCustom.Custom.rainWorld.inGameTranslator.SpecificTextFolderDirectory()}{Path.DirectorySeparatorChar}{name.ToLower()}.txt");
                if (File.Exists(path))
                    lines = File.ReadAllLines(path);
                else lines = new string[] { "CAN'T FIND A CREATURE DESCRIPTION" };

                entityDescription = new MenuLabel[lines.Length];
                for (int i = 0; i < lines.Length; i++)
                {
                    Vector2 pos = new Vector2(menu.descriprionBoxBack.GetPosition().x + 40f, entityDescriptionLabel.pos.y - 30f * (i + 1.5f));
                    entityDescription[i] = new MenuLabel(menu, menu.pages[0], lines[i], pos, Vector2.one, false);
                    entityDescription[i].label.alignment = FLabelAlignment.Left;
                    menu.pages[0].subObjects.Add(entityDescription[i]);
                }
            }

            public void GenerateCharacteristicLabels()
            {
                string[] lines = characteristic.GenerateLines();
                entityCharacteristicLabels = new MenuLabel[lines.Length];
                for (int i = 0; i < lines.Length; i++)
                {
                    Vector2 pos = menu.descriprionBoxBack.GetPosition() + new Vector2(30f, menu.descriprionBoxBack.scaleY - 100f - 20f * i);
                    entityCharacteristicLabels[i] = new MenuLabel(menu, menu.pages[0], lines[i], pos, Vector2.one, false);
                    entityCharacteristicLabels[i].label.alignment = FLabelAlignment.Left;
                    menu.pages[0].subObjects.Add(entityCharacteristicLabels[i]);
                }
            }

            private string ResolveName(string baseString)
            {
                string name = "creaturetype-" + baseString;
                if (RWCustom.Custom.rainWorld.inGameTranslator.HasShortstringTranslation(name))
                    return Plugin.Translate(name);
                else
                {
                    CreatureTemplate template = StaticWorld.GetCreatureTemplate(new CreatureTemplate.Type(baseString));
                    CreatureTemplate ancestor = template.ancestor;
                    if (ancestor != null && ancestor.type.value != template.TopAncestor().type.ToString())
                        return ResolveName(ancestor.type.value);
                    if (ancestor != null && RWCustom.Custom.rainWorld.inGameTranslator.HasShortstringTranslation("creaturetype-" + ancestor.type.value))
                        return Plugin.Translate("creaturetype-" + ancestor.type.value) + $"\n({baseString})";
                    return baseString;
                }
            }

            public void Clear()
            {
                icon.RemoveFromContainer();
                image.RemoveFromContainer();
                menu.pages[0].RemoveSubObject(entityName);
                entityName.RemoveSprites();
                menu.pages[0].RemoveSubObject(entityDescriptionLabel);
                entityDescriptionLabel.RemoveSprites();
                menu.pages[0].RemoveSubObject(imageBox);
                imageBox.RemoveSprites();
                for (int i = 0; i < entityCharacteristicLabels.Length; i++)
                {
                    menu.pages[0].RemoveSubObject(entityCharacteristicLabels[i]);
                    entityCharacteristicLabels[i].RemoveSprites();
                }
                for (int i = 0; i < entityDescription.Length; i++)
                {
                    menu.pages[0].RemoveSubObject(entityDescription[i]);
                    entityDescription[i].RemoveSprites();
                }
            }

            public struct Characteristic
            {
                public float hp, damage, biteChance;
                public int foodPoints, score, kills;
                public CreatureTemplate.Relationship.Type behaviour;
                public bool IsLizard => biteChance != default;

                public string[] GenerateLines()
                {
                    List<string> lines = new List<string>();

                    if (damage != default)
                        lines.Add(Plugin.Translate("Damage: %").Replace("%", damage.ToString()));
                    if (biteChance != default)
                        lines.Add(Plugin.Translate("Deadly Bite Chance: %").Replace("%", $"{biteChance * 100f:F1}%"));
                    lines.Add(Plugin.Translate("Kill count: %").Replace("%", kills.ToString()));
                    if (foodPoints != 0)
                        lines.Add(Plugin.Translate("Restores % food pips to carnivorous slugcats").Replace("%", foodPoints.ToString()));
                    else lines.Add(Plugin.Translate("Doesn't restore food pips"));
                    lines.Add(Plugin.Translate("Health: %").Replace("%", hp.ToString()));
                    lines.Add(Plugin.Translate("Behaviour") + ": " + Plugin.Translate($"behav-{behaviour.value}"));
                    lines.Add(Plugin.Translate("Points per kill: %").Replace("%", score == -1 ? "?" : score.ToString()));
                    lines.Add(Plugin.Translate("Total points: %").Replace("%", score == -1 ? "?" : (score * kills).ToString()));
                    return lines.ToArray();
                }
            }
        }

        public struct SlugcatInfo
        {
            public SlugcatStats.Name name;
            public List<KilledInfo> kills;

            public SlugcatInfo(SlugcatStats.Name _name, List<KilledInfo> _kills)
            {
                name = _name;
                kills = _kills;
            }

            public SlugcatInfo(SlugcatStats.Name _name)
            {
                name = _name;
                kills = null;
            }

            public struct KilledInfo
            {
                public IconSymbol.IconSymbolData iconData;
                public int kills;

                public static KilledInfo Transform(KeyValuePair<IconSymbol.IconSymbolData, int> pair)
                {
                    return new KilledInfo { iconData = pair.Key, kills = pair.Value };
                }
            }
        }
    }
}
