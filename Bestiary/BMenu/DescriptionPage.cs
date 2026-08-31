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
                    characteristic = new CreatureCharacteristic(info);
                    InitCreaturePage(iconData);
                    break;
                case EntityManager.EntityType.Slugcat:
                    SlugcatStats.Name name = bMenu.slugcatManager.Saves[bMenu.slugcatManager.SelectedSlugcat].name;
                    SaveState save = bMenu.manager.rainWorld.progression.GetOrInitiateSaveState(name, null, bMenu.manager.menuSetup, false);
                    int deaths = save.deathPersistentSaveData.deaths;
                    int cycles = save.cycleNumber;
                    characteristic = new SlugcatCharacteristic(name, deaths, cycles - 1);
                    InitSlugcatPage();
                    break;
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
            icon = new FSprite(CreatureSymbol.SpriteNameOfCreature(iconData))
            {
                color = CreatureSymbol.ColorOfCreature(iconData),
                scale = 2
            };
            icon.SetPosition(DescBox.position + new Vector2(50f, DescBox.size.y - 50f) - BestiaryMenu.ResolutionOffset);
            bMenu.pages[0].Container.AddChild(icon);

            Vector2 pos = DescBox.position + new Vector2(100f, DescBox.size.y - 50f);
            _entityName = new MenuLabel(bMenu, bMenu.pages[0], Plugin.ResolveCreatureName(name), pos, Vector2.zero, true);
            _entityName.label.alignment = FLabelAlignment.Left;
            bMenu.pages[0].subObjects.Add(_entityName);

            GetGeneralInfo();
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

            string imageName = $"bestiary_{name.ToLower()}";
            if (Futile.atlasManager._allElementsByName.TryGetValue(imageName, out FAtlasElement element))
            {
                image = new FSprite(element);
                image.scale = Mathf.Min(0.9f * bMenu.boxManager.boxes["imageBox"].Rectangle.size.x / image.element.sourceSize.x,
                    0.9f * bMenu.boxManager.boxes["imageBox"].Rectangle.size.y / image.element.sourceSize.y);
            }
            else if (IsInv)
            {
                InvImg();
                return;
            }
            else image = new FSprite("Sandbox_QuestionMark") { color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.Black), scale = 2 };

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
                    Plugin.descriptionContainer.Creatures.TryGetValue(name.ToLower(), out description);
                    break;
                case EntityManager.EntityType.Slugcat:
                    Plugin.descriptionContainer.Slugcats.TryGetValue(name.ToLower(), out description);
                    break;
            }
            description = description.Trim();

            //MenuLabel label = new MenuLabel(bMenu, bMenu.pages[0], string.Empty, Vector2.zero, Vector2.zero, false);
            FLabel label = new FLabel(RWCustom.Custom.GetFont(), string.Empty);
            string[] words = description.Split(new char[] { ' ' });
            float fieldLength = DescBox.width * 0.92f;
            List<MenuLabel> labels = new List<MenuLabel>();

            Action<string, string> appendLabel = (txt, word) =>
            {
                Vector2 pos = new Vector2(DescBox.position.x + 30f, _entityDescriptionLabel.pos.y - 27f * (labels.Count + 1.25f));
                MenuLabel l = new MenuLabel(bMenu, bMenu.pages[0], txt, pos, Vector2.zero, false);
                l.label.alignment = FLabelAlignment.Left;
                labels.Add(l);
                bMenu.pages[0].subObjects.Add(l);
                label.text = word + " ";
            };

            for (int i = 0; i < words.Length; i++)
            {
                string txt = label.text;
                label.text += words[i];
                if (label.textRect.width > fieldLength)
                    appendLabel(txt, words[i]);
                else if (i != words.Length - 1) label.text += " ";
            }
            appendLabel(label.text, string.Empty);

            entityDescription = labels.ToArray();
        }

        private bool IsInv => name == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel.value;

        private void Inv()
        {
            string line = Plugin.Translate("Thanks Andrew.");
            entityDescription = new MenuLabel[1];

            Vector2 pos = new Vector2(DescBox.position.x + 30f, _entityDescriptionLabel.pos.y - 45f);
            entityDescription[0] = new MenuLabel(bMenu, bMenu.pages[0], line, pos, Vector2.one, false);
            entityDescription[0].label.alignment = FLabelAlignment.Left;
            entityDescription[0].label.color = Color.red;
            bMenu.pages[0].subObjects.Add(entityDescription[0]);
        }

        public void Clear()
        {
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
