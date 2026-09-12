using Menu;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Bestiary.BMenu
{
    public class DescriptionPage
    {
        public BestiaryMenu bMenu;
        public string name;
        public FSprite icon, image;
        public FSprite[] foodMeter;
        private MenuLabel _entityName, _entityDescriptionLabel, _emptinessLabel;
        public readonly EntityManager.EntityType entityType;
        public MenuLabel[] entityDescription, entityCharacteristicLabels;
        public Rect DescBox => bMenu.boxManager.boxes["descriptionBox"].Rectangle;
        public ICharacteristic characteristic;
        private SaveInfo.Info.KilledInfo _creatureInfo;
        private string _customCreatureIconElement;
        private SlugcatStats.Name _slugcatId;
        private int _lastAnimatedCreatureIconVariant = -1;
  
        private const int DescriptionVisibleLines = 10;
        private const float DescriptionLineSpacing = 27f;
        private static readonly Color DescriptionCommentColor = new Color(0f, 217f / 255f, 224f / 255f);

        private VerticalSlider _descriptionSlider;

        private int _descriptionScrollOffset;
        private int _descriptionMaxScroll;
        private float _descriptionScrollValue = 1f;

        private float _descriptionFirstLineY;

        public float DescriptionScrollValue => _descriptionScrollValue;
        private MenuIllustration inv;
        private int invCounter, invFrame;

        public DescriptionPage(BestiaryMenu owner, IconSymbol.IconSymbolData iconData, EntityManager.EntityType type)
        {
            bMenu = owner;
            entityType = type;
            switch (entityType)
            {
                case EntityManager.EntityType.Creature:
                    SaveInfo.Info.KilledInfo info = bMenu.entityManager.GetEntityByRealIndex(bMenu.entityManager.SelectedEntity) as SaveInfo.Info.KilledInfo;
                    _creatureInfo = info;
                    characteristic = info != null
                        ? new CreatureCharacteristic(info)
                        : null;
                    InitCreaturePage(iconData);
                    break;
                // case EntityManager.EntityType.Slugcat:
                //     SlugcatStats.Name name = bMenu.slugcatManager.Saves[bMenu.slugcatManager.SelectedSlugcat].name;
                //     SaveState save = bMenu.manager.rainWorld.progression.GetOrInitiateSaveState(name, null, bMenu.manager.menuSetup, false);
                //     _slugcatId = name;
                //     int deaths = save.deathPersistentSaveData.deaths;
                //     int cycles = save.cycleNumber;
                //     characteristic = new SlugcatCharacteristic(name, deaths, cycles - 1);
                //     InitSlugcatPage();
                //     break;
                case EntityManager.EntityType.Slugcat:
                {
                    SaveInfo saveInfo = bMenu.slugcatManager.Saves[bMenu.slugcatManager.SelectedSlugcat];
                    SlugcatStats.Name name = saveInfo.name;
                    _slugcatId = name;
                    characteristic = new SlugcatCharacteristic(name, saveInfo.deaths, saveInfo.cycles - 1);
                    InitSlugcatPage();
                    break;
                }
                case EntityManager.EntityType.Item:
                    characteristic = null;
                    InitItemPage();
                    break;
                case EntityManager.EntityType.Iterator:
                    break;
                default:
                    InitEmptiness();
                    break;
            }
        }

        private void InitItemPage()
        {
            if (!(bMenu.entityManager.GetEntityByRealIndex(bMenu.entityManager.SelectedEntity) is SaveInfo.Info.ItemInfo info)) return;

            name = info.iconData.itemType.ToString();
            icon = new FSprite(ItemSymbol.SpriteNameForItem(info.iconData.itemType, info.iconData.intData))
            {
                color = ItemSymbol.ColorForItem(info.iconData.itemType, info.iconData.intData),
                scale = 2
            };
            icon.SetPosition(DescBox.position + new Vector2(50f, DescBox.size.y - 50f) - BestiaryMenu.ResolutionOffset);
            bMenu.pages[0].Container.AddChild(icon);

            Vector2 pos = DescBox.position + new Vector2(100f, DescBox.size.y - 50f);
            _entityName = new MenuLabel(bMenu, bMenu.pages[0], Plugin.ResolveItemName(name), pos, Vector2.zero, true);
            _entityName.label.alignment = FLabelAlignment.Left;
            bMenu.pages[0].subObjects.Add(_entityName);

            GetGeneralInfo();
        }

        private void InitSlugcatPage()
        {
            SlugcatCharacteristic sChar = characteristic as SlugcatCharacteristic;
            name = SlugcatStats.getSlugcatName(sChar.slugcat).ToString();
            icon = new FSprite("Kill_Slugcat")
            {
                color = PlayerGraphics.DefaultSlugcatColor(sChar.slugcat),
                scale = 2
            };
            icon.SetPosition(DescBox.position + new Vector2(50f, DescBox.size.y - 50f) - BestiaryMenu.ResolutionOffset);
            bMenu.pages[0].Container.AddChild(icon);

            Vector2 pos = DescBox.position + new Vector2(100f, DescBox.size.y - 50f);
            _entityName = new MenuLabel(bMenu, bMenu.pages[0], Plugin.Translate(SlugcatStats.getSlugcatName(sChar.slugcat)), pos, Vector2.zero, true);
            _entityName.label.alignment = FLabelAlignment.Left;
            bMenu.pages[0].subObjects.Add(_entityName);

            GetGeneralInfo();
        }

        private void InitCreaturePage(IconSymbol.IconSymbolData iconData)
        {
            name = iconData.critType.ToString();

            if (_creatureInfo != null &&
                BestiaryAssets.TryGetCustomElement(_creatureInfo.customIconPath, out _customCreatureIconElement))
            {
                icon = new FSprite(_customCreatureIconElement)
                {
                    color = Color.white,
                    scale = 2f
                };
            }
            else
            {
                IconSymbol.IconSymbolData current = GetCurrentCreatureIconData(iconData);
                string sprite = CreatureSymbol.SpriteNameOfCreature(current);
                icon = new FSprite(sprite) { color = CreatureSymbol.ColorOfCreature(current), scale = 2f };

                if (_creatureInfo?.iconVariants != null && _creatureInfo.iconVariants.Count > 1)
                    _lastAnimatedCreatureIconVariant = CreatureCatalog.GetAnimatedVariantIndex(_creatureInfo.iconVariants.Count);
            }

            icon.SetPosition(DescBox.position + new Vector2(50f, DescBox.size.y - 50f) - BestiaryMenu.ResolutionOffset);
            bMenu.pages[0].Container.AddChild(icon);

            Vector2 pos = DescBox.position + new Vector2(100f, DescBox.size.y - 50f);
            string displayName = _creatureInfo != null && !string.IsNullOrWhiteSpace(_creatureInfo.displayNameOverride)
                ? _creatureInfo.displayNameOverride
                : Plugin.ResolveCreatureName(name);
            _entityName = new MenuLabel(bMenu, bMenu.pages[0], displayName, pos, Vector2.zero, true);
            _entityName.label.alignment = FLabelAlignment.Left;
            bMenu.pages[0].subObjects.Add(_entityName);

            GetGeneralInfo();
        }

        private IconSymbol.IconSymbolData GetCurrentCreatureIconData(IconSymbol.IconSymbolData fallback)
        {
            if (_creatureInfo?.iconVariants != null && _creatureInfo.iconVariants.Count > 0)
            {
                int index = CreatureCatalog.GetAnimatedVariantIndex(_creatureInfo.iconVariants.Count);
                return _creatureInfo.iconVariants[index];
            }

            return fallback;
        }

        private void ApplyCreatureHeaderVariant(IconSymbol.IconSymbolData data)
        {
            if (icon == null)
                return;

            try
            {
                string sprite = CreatureSymbol.SpriteNameOfCreature(data);
                icon.SetElementByName(sprite);
                icon.color = CreatureSymbol.ColorOfCreature(data);
                icon.scale = 2f;
            }
            catch (System.Exception ex)
            {
                Plugin.logger.LogWarning(
                    $"[Bestiary] Failed to render description icon {data.critType?.value} data={data.intData}: {ex.Message}");
            }
        }

        private void UpdateAnimatedCreatureIcon()
        {
            if (entityType != EntityManager.EntityType.Creature ||
                _creatureInfo == null ||
                !string.IsNullOrEmpty(_customCreatureIconElement) ||
                _creatureInfo.iconVariants == null ||
                _creatureInfo.iconVariants.Count <= 1 ||
                icon == null)
            {
                return;
            }

            int variant = CreatureCatalog.GetAnimatedVariantIndex(_creatureInfo.iconVariants.Count);
            if (variant == _lastAnimatedCreatureIconVariant)
                return;

            _lastAnimatedCreatureIconVariant = variant;
            ApplyCreatureHeaderVariant(_creatureInfo.iconVariants[variant]);
            icon.SetPosition(DescBox.position + new Vector2(50f, DescBox.size.y - 50f) - BestiaryMenu.ResolutionOffset);
        }

        private void GetGeneralInfo()
        {
            Vector2 descrPos = DescBox.position + DescBox.size.x / 2f * Vector2.right + DescBox.size.y / 2.2f * Vector2.up;
            _entityDescriptionLabel = new MenuLabel(bMenu, bMenu.pages[0], Plugin.Translate("b-Description"), descrPos, Vector2.one, true);
            bMenu.pages[0].subObjects.Add(_entityDescriptionLabel);

            Vector2 imgPos = bMenu.boxManager.boxes["descriptionBox"].normilizedPos + new Vector2(0.4f, 0.5f) * bMenu.boxManager.boxes["descriptionBox"].normilizedSize;
            InitImage(imgPos);

            GetDescription();
            GenerateCharacteristicLabels();
        }

        private void InitImage(Vector2 nPos)
        {
            Vector2 nSize = 0.9f * (bMenu.boxManager.boxes["descriptionBox"].normilizedPos + bMenu.boxManager.boxes["descriptionBox"].normilizedSize - nPos);
            bMenu.boxManager.CreateBox("imageBox", nPos, nSize, new Color(0.6f, 0.6f, 0.6f), 0.65f);
             string customImagePath = null;

            if (entityType == EntityManager.EntityType.Creature &&
                _creatureInfo != null)
            {
                customImagePath = _creatureInfo.customImagePath;
            }
            else if (entityType == EntityManager.EntityType.Slugcat &&
                    _slugcatId != null)
            {
                customImagePath =
                    $"bestiary_custom/images/{_slugcatId.value}";
#if DEBUG
                Plugin.logger.LogInfo(
                    $"[Bestiary] Slugcat image ID: {_slugcatId.value}"
                );
#endif
            }

            if (!string.IsNullOrEmpty(customImagePath) &&
                BestiaryAssets.TryGetCustomElement(
                    customImagePath,
                    out string customImageElement))
            {
                image = new FSprite(customImageElement);

                image.scale = Mathf.Min(
                    0.9f *
                    bMenu.boxManager.boxes["imageBox"].Rectangle.size.x /
                    image.element.sourceSize.x,

                    0.9f *
                    bMenu.boxManager.boxes["imageBox"].Rectangle.size.y /
                    image.element.sourceSize.y
                );
            }
            else
            {
                 string imageName = $"bestiary_{name.ToLower()}";
                if (Futile.atlasManager._allElementsByName.TryGetValue(imageName, out FAtlasElement element))
                {
                    image = new FSprite(element);
                    image.scale = Mathf.Min(
                        0.9f * bMenu.boxManager.boxes["imageBox"].Rectangle.size.x / image.element.sourceSize.x,
                        0.9f * bMenu.boxManager.boxes["imageBox"].Rectangle.size.y / image.element.sourceSize.y);
                }
                else if (IsInv)
                {
                    InvImg();
                    return;
                }
                else
                {
                    image = new FSprite("Sandbox_QuestionMark")
                    {
                        color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.Black),
                        scale = 2
                    };
                }
            }

            image.SetPosition(bMenu.boxManager.boxes["imageBox"].Rectangle.center - BestiaryMenu.ResolutionOffset);
            bMenu.pages[0].Container.AddChild(image);
        }

        private void InvImg()
        {
            Vector2 pos = bMenu.boxManager.boxes["imageBox"].Rectangle.center;
            inv = new MenuIllustration(bMenu, bMenu.pages[0], "Content", "blush_001", pos, true, true);
            invFrame = 1;
            bMenu.pages[0].subObjects.Add(inv);
        }

        public void UpdateImage()
        {
             UpdateAnimatedCreatureIcon();

            if (!IsInv)
                return;

            invCounter++;
            if (invCounter >= 5)
            {
                invCounter = 0;
                invFrame++;
                string num = invFrame.ToString("000");
                if (!File.Exists(AssetManager.ResolveFilePath($"Content/blush_{num}.png")))
                {
                    invFrame = 1;
                    num = "001";
                }
                inv.fileName = $"blush_{num}";
                inv.LoadFile("Content");
                inv.sprite.SetElementByName(inv.fileName);
            }
        }

        public void GenerateCharacteristicLabels()
        {
            if (characteristic == null)
                return;
            string[] lines = characteristic.GenerateLines();
            entityCharacteristicLabels = new MenuLabel[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                Vector2 pos = DescBox.position + new Vector2(30f, DescBox.size.y - 100f - 20f * i);
                if (lines[i] != null && lines[i] == string.Empty && foodMeter == null && characteristic is SlugcatCharacteristic)
                    InitFoodPips(pos);
                entityCharacteristicLabels[i] = new MenuLabel(bMenu, bMenu.pages[0], lines[i], pos, Vector2.one, false);
                entityCharacteristicLabels[i].label.alignment = FLabelAlignment.Left;
                bMenu.pages[0].subObjects.Add(entityCharacteristicLabels[i]);
            }
        }

        private void InitFoodPips(Vector2 pos)
        {
            SlugcatCharacteristic sChar = characteristic as SlugcatCharacteristic;

            foodMeter = new FSprite[sChar.maxFood * 2 + 1];
            for (int i = 0; i < sChar.maxFood; i++)
            {
                foodMeter[2 * i] = new FSprite("FoodCircleA");
                foodMeter[2 * i + 1] = new FSprite("FoodCircleB");

                Vector2 offset = Vector2.right * 27f * i + new Vector2(1f, -0.75f) * foodMeter[2 * i].element.sourcePixelSize * 0.5f - BestiaryMenu.ResolutionOffset;
                offset += i >= sChar.minFood ? Vector2.right * 10f : Vector2.zero;
                foodMeter[2 * i].SetPosition(pos + offset);
                foodMeter[2 * i + 1].SetPosition(pos + offset);
                bMenu.pages[0].Container.AddChild(foodMeter[2 * i]);
                bMenu.pages[0].Container.AddChild(foodMeter[2 * i + 1]);
            }

            foodMeter[foodMeter.Length - 1] = new FSprite("pixel")
            {
                scaleY = 30,
                scaleX = 3
            };
            foodMeter[foodMeter.Length - 1].SetPosition(pos - BestiaryMenu.ResolutionOffset + sChar.minFood * Vector2.right * 27f + Vector2.right * 4f + Vector2.down * 0.75f * 0.5f * foodMeter[0].element.sourcePixelSize.y);
            bMenu.pages[0].Container.AddChild(foodMeter[foodMeter.Length - 1]);
        }

        private void InitEmptiness()
        {
            Vector2 pos = bMenu.boxManager.boxes["descriptionBox"].Rectangle.center;
            _emptinessLabel = new MenuLabel(bMenu, bMenu.pages[0], Plugin.Translate("[ Nothing to load ]"), pos, Vector2.zero, false);
            _emptinessLabel.label.alignment = FLabelAlignment.Center;
            bMenu.pages[0].subObjects.Add(_emptinessLabel);
        }

        private void GetDescription()
        {
            if (IsInv)
            {
                Inv();
                return;
            }

            string description = "CAN'T FIND AN ENTITY DESCRIPTION";

            switch (entityType)
            {
                case EntityManager.EntityType.Creature:
                {
                    string descriptionKey =
                        _creatureInfo != null &&
                        !string.IsNullOrWhiteSpace(
                            _creatureInfo.catalogId)
                            ? _creatureInfo.catalogId
                            : name;

                    if (!Plugin.descriptionContainer.Creatures
                            .TryGetValue(
                                descriptionKey.ToLowerInvariant(),
                                out description) ||
                        string.IsNullOrWhiteSpace(description))
                    {
                        if (_creatureInfo != null &&
                            !string.IsNullOrWhiteSpace(
                                _creatureInfo.descriptionOverride))
                        {
                            description =
                                Plugin.Translate(
                                    _creatureInfo.descriptionOverride
                                );
                        }
                        else
                        {
                            Plugin.logger.LogWarning(
                                $"[Bestiary] No description found for creature: {descriptionKey}"
                            );

                            description =
                                Plugin.Translate(
                                    "NO DESCRIPTION FOUND"
                                ) +
                                "\n\n" +
                                Plugin.Translate(
                                    "Internal name:"
                                ) +
                                " " +
                                descriptionKey;
                        }
                    }

                    break;
                }

                case EntityManager.EntityType.Slugcat:
                {
                    string descriptionKey =
                        name.ToLowerInvariant();

                    if (!Plugin.descriptionContainer.Slugcats
                            .TryGetValue(
                                descriptionKey,
                                out description) ||
                        string.IsNullOrWhiteSpace(description))
                    {
                        Plugin.logger.LogWarning(
                            $"[Bestiary] No description found for slugcat: {name}"
                        );

                        description =
                            Plugin.Translate(
                                "NO DESCRIPTION FOUND"
                            ) +
                            "\n\n" +
                            Plugin.Translate(
                                "Internal name:"
                            ) +
                            " " +
                            name;
                    }

                    break;
                }
            }

            description = description.Trim();

            FLabel measureLabel =
                new FLabel(
                    RWCustom.Custom.GetFont(),
                    string.Empty
                );

            float fieldLength =
                DescBox.width * 0.92f;

            List<MenuLabel> labels =
                new List<MenuLabel>();

            _descriptionFirstLineY =
                _entityDescriptionLabel.pos.y -
                DescriptionLineSpacing * 1.25f;

            void AddLine(string text, bool isComment)
            {
                Vector2 pos = new Vector2(
                    DescBox.position.x + 30f,
                    _descriptionFirstLineY -
                    DescriptionLineSpacing * labels.Count
                );

                MenuLabel l = new MenuLabel(
                    bMenu,
                    bMenu.pages[0],
                    text,
                    pos,
                    Vector2.zero,
                    false
                );

                l.label.alignment = FLabelAlignment.Left;

                if (isComment)
                    l.label.color = DescriptionCommentColor;

                labels.Add(l);
                bMenu.pages[0].subObjects.Add(l);
            }
            void AddWrappedLine(string manualLine, bool isComment)
            {
                if (string.IsNullOrWhiteSpace(manualLine))
                {
                    AddLine(string.Empty, isComment);
                    return;
                }

                string[] words = manualLine.Split(
                    new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries
                );

                string currentLine = string.Empty;

                foreach (string word in words)
                {
                    string candidate =
                        string.IsNullOrEmpty(currentLine)
                            ? word
                            : currentLine + " " + word;

                    measureLabel.text = candidate;

                    if (measureLabel.textRect.width > fieldLength &&
                        !string.IsNullOrEmpty(currentLine))
                    {
                        AddLine(currentLine, isComment);
                        currentLine = word;
                    }
                    else
                    {
                        currentLine = candidate;
                    }
                }

                if (!string.IsNullOrEmpty(currentLine))
                    AddLine(currentLine, isComment);
            }
            string normalizedDescription = description
                .Replace("\r\n", "\n")
                .Replace('\r', '\n');

            string[] manualLines =
                normalizedDescription.Split('\n');

            List<string> normalLines =
                new List<string>();

            List<string> commentLines =
                new List<string>();

            foreach (string manualLine in manualLines)
            {
                string trimmedStart =
                    manualLine.TrimStart();

                if (trimmedStart.StartsWith("//"))
                {
                    string commentText =
                        trimmedStart.Substring(2).TrimStart();

                    commentLines.Add(commentText);
                }
                else
                {
                    normalLines.Add(manualLine);
                }
            }

            while (normalLines.Count > 0 &&
                string.IsNullOrWhiteSpace(
                    normalLines[normalLines.Count - 1]))
            {
                normalLines.RemoveAt(
                    normalLines.Count - 1
                );
            }

            foreach (string normalLine in normalLines)
            {
                AddWrappedLine(
                    normalLine,
                    false
                );
            }

            if (commentLines.Count > 0)
            {
                if (labels.Count > 0)
                    AddLine(string.Empty, false);

                foreach (string commentLine in commentLines)
                {
                    AddWrappedLine(
                        commentLine,
                        true
                    );
                }
            }

            entityDescription = labels.ToArray();

            _descriptionScrollOffset = 0;
            _descriptionScrollValue = 1f;

            _descriptionMaxScroll = Mathf.Max(
                0,
                entityDescription.Length -
                DescriptionVisibleLines
            );

            ApplyDescriptionScroll();
            InitDescriptionSlider();
        }
        private void BindDescriptionSliderNavigation()
        {
            if (_descriptionSlider == null)
                return;
             _descriptionSlider.nextSelectable[2] = _descriptionSlider;
             if (entityType == EntityManager.EntityType.Slugcat &&
                bMenu.buttonManager?.slugcatButtons != null)
            {
                int index = bMenu.slugcatManager.SelectedSlugcat;

                if (index >= 0 &&
                    index < bMenu.buttonManager.slugcatButtons.Length)
                {
                    var slugButton =
                        bMenu.buttonManager.slugcatButtons[index]?.button;

                    if (slugButton != null)
                    {
                        slugButton.nextSelectable[2] = _descriptionSlider;
                        _descriptionSlider.nextSelectable[0] = slugButton;
                    }
                }

                return;
            }
             if (entityType == EntityManager.EntityType.Creature &&
                bMenu.buttonManager?.entityButtons != null &&
                bMenu.buttonManager.entityButtons.Count > 0)
            {
                int count = bMenu.buttonManager.entityButtons.Count;
                int buttonsPerRow = EntityManager.buttonsInRow;

                for (int rowStart = 0; rowStart < count; rowStart += buttonsPerRow)
                {
                    int rightIndex = Math.Min(rowStart + buttonsPerRow - 1, count - 1);
                    var entityButton = bMenu.buttonManager.entityButtons[rightIndex]?.button;

                    if (entityButton != null)
                        entityButton.nextSelectable[2] = _descriptionSlider;
                }

                int pageSize = EntityManager.buttonsInRow * EntityManager.buttonsInColumn;
                int pageStart = bMenu.entityManager.CurrentPage * pageSize;
                int localSelected = bMenu.entityManager.SelectedEntity - pageStart;

                if (localSelected >= 0 && localSelected < count)
                {
                    int rowStart = (localSelected / buttonsPerRow) * buttonsPerRow;
                    int rightIndex = Math.Min(rowStart + buttonsPerRow - 1, count - 1);
                    var returnButton = bMenu.buttonManager.entityButtons[rightIndex]?.button;

                    if (returnButton != null)
                        _descriptionSlider.nextSelectable[0] = returnButton;
                }
                else
                {
                    var fallback = bMenu.buttonManager.entityButtons[count - 1]?.button;
                    if (fallback != null)
                        _descriptionSlider.nextSelectable[0] = fallback;
                }
            }
        }
        private void UnbindDescriptionSliderNavigation()
        {
            if (_descriptionSlider == null)
                return;

            if (bMenu.buttonManager?.entityButtons != null)
            {
                for (int i = 0;
                    i < bMenu.buttonManager.entityButtons.Count;
                    i++)
                {
                    var button =
                        bMenu.buttonManager
                            .entityButtons[i]
                            ?.button;

                    if (button != null &&
                        button.nextSelectable[2] ==
                        _descriptionSlider)
                    {
                        button.nextSelectable[2] = null;
                    }
                }
            }

            if (bMenu.buttonManager?.slugcatButtons != null)
            {
                for (int i = 0;
                    i < bMenu.buttonManager.slugcatButtons.Length;
                    i++)
                {
                    var button =
                        bMenu.buttonManager
                            .slugcatButtons[i]
                            ?.button;

                    if (button != null &&
                        button.nextSelectable[2] ==
                        _descriptionSlider)
                    {
                        button.nextSelectable[2] = null;
                    }
                }
            }
        }
        public void RefreshDescriptionSliderNavigation()
        {
            BindDescriptionSliderNavigation();
        }
        private void InitDescriptionSlider()
        {
            if (_descriptionMaxScroll <= 0)
                return;
             float bottomY =
                _descriptionFirstLineY -
                DescriptionLineSpacing *
                (DescriptionVisibleLines - 1);
                _descriptionSlider =
                new VerticalSlider(
                    bMenu,
                    bMenu.pages[0],
                    string.Empty,

                    new Vector2(
                        DescBox.xMax - 45f,
                        bottomY
                    ),

                    new Vector2(
                        30f,
                        DescriptionLineSpacing *
                        (DescriptionVisibleLines - 1)
                    ),

                    BestiaryEnums.DescriptionScroll,
                    true
                );

            bMenu.pages[0].subObjects.Add(
                _descriptionSlider
            );

            BindDescriptionSliderNavigation();
        }
        private void ApplyDescriptionScroll()
        {
            if (entityDescription == null)
                return;

            for (int i = 0; i < entityDescription.Length; i++)
            {
                MenuLabel line = entityDescription[i];

                int visibleIndex =
                    i - _descriptionScrollOffset;

                bool visible =
                    visibleIndex >= 0 &&
                    visibleIndex < DescriptionVisibleLines;

                if (visible)
                {
                    Vector2 targetPos = new Vector2(
                        DescBox.position.x + 30f,
                        _descriptionFirstLineY -
                        DescriptionLineSpacing * visibleIndex
                    );
                      line.pos = targetPos;
                    line.lastPos = targetPos;
                }

                line.label.isVisible = visible;
            }
        }
        public void SetDescriptionScroll(float value)
        {
            if (_descriptionMaxScroll <= 0)
            {
                _descriptionScrollValue = 1f;
                return;
            }
              _descriptionScrollValue = Mathf.Clamp01(value);
             int newOffset = Mathf.RoundToInt(
                (1f - _descriptionScrollValue) * _descriptionMaxScroll
            );

            newOffset = Mathf.Clamp(newOffset, 0, _descriptionMaxScroll);

            if (newOffset == _descriptionScrollOffset)
                return;

            _descriptionScrollOffset = newOffset;
            ApplyDescriptionScroll();
        }
        public void UpdateDescriptionScrollWheel()
        {
            if (_descriptionMaxScroll <= 0 ||
                entityDescription == null)
            {
                return;
            }

            float top =
                _descriptionFirstLineY +
                DescriptionLineSpacing * 0.5f;

            float bottom =
                _descriptionFirstLineY -
                DescriptionLineSpacing *
                (DescriptionVisibleLines - 1) -
                DescriptionLineSpacing * 0.5f;

            Rect descriptionArea =
                new Rect(
                    DescBox.xMin + 20f,
                    bottom,
                    DescBox.width - 60f,
                    top - bottom
                );
              if (!descriptionArea.Contains(
                bMenu.mousePosition))
            {
                return;
            }

            int wheel =
                bMenu.mouseScrollWheelMovement;

            if (wheel == 0)
                return;

            int oldOffset =
                _descriptionScrollOffset;

            _descriptionScrollOffset =
                Mathf.Clamp(
                    _descriptionScrollOffset + wheel,
                    0,
                    _descriptionMaxScroll
                );

            if (oldOffset != _descriptionScrollOffset)
            {
                  _descriptionScrollValue =
                    1f - _descriptionScrollOffset / (float)_descriptionMaxScroll;

                ApplyDescriptionScroll();
            }
        }
        private bool IsInv => ModManager.MSC && MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel != null && name == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel.value;
        private void Inv()
        {
            string line = Plugin.Translate("Thanks Andrew.");
            entityDescription = new MenuLabel[1];

            Vector2 pos = new Vector2(DescBox.position.x + 30f, _entityDescriptionLabel.pos.y - 45f);
            entityDescription[0] = new MenuLabel(bMenu, bMenu.pages[0], line, pos, Vector2.one, false);
            entityDescription[0].label.alignment = FLabelAlignment.Left;
            bMenu.pages[0].subObjects.Add(entityDescription[0]);
        }

        public void Clear()
        {
            UnbindDescriptionSliderNavigation();

            if (_descriptionSlider != null)
            {
                bMenu.pages[0].RemoveSubObject(
                    _descriptionSlider
                );

                _descriptionSlider.RemoveSprites();
                _descriptionSlider = null;
            }

            icon?.RemoveFromContainer();
            image?.RemoveFromContainer();
            if (_entityDescriptionLabel != null)
            {
                bMenu.pages[0].RemoveSubObject(_entityDescriptionLabel);
                _entityDescriptionLabel.RemoveSprites();
            }
            if (_entityName != null)
            {
                bMenu.pages[0].RemoveSubObject(_entityName);
                _entityName.RemoveSprites();
            }
            if (_emptinessLabel != null)
            {
                bMenu.pages[0].RemoveSubObject(_emptinessLabel);
                _emptinessLabel.RemoveSprites();
            }

            if (bMenu.boxManager.boxes.TryGetValue("imageBox", out BoxManager.Box box))
            {
                box.Clear();
                bMenu.boxManager.boxes.Remove("imageBox");
            }
            if (entityDescription != null)
            {
                for (int i = 0; i < entityDescription.Length; i++)
                {
                    bMenu.pages[0].RemoveSubObject(entityDescription[i]);
                    entityDescription[i].RemoveSprites();
                }
            }
            if (entityCharacteristicLabels != null)
            {
                for (int i = 0; i < entityCharacteristicLabels.Length; i++)
                {
                    bMenu.pages[0].RemoveSubObject(entityCharacteristicLabels[i]);
                    entityCharacteristicLabels[i].RemoveSprites();
                }
            }
            if (foodMeter != null)
            {
                for (int i = 0; i < foodMeter.Length; i++)
                    foodMeter[i]?.RemoveFromContainer();
            }
            if (inv != null)
            {
                bMenu.pages[0].RemoveSubObject(inv);
                inv.RemoveSprites();
            }
        }
    }
}
