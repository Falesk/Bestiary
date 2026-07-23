using Menu;
using System.IO;
using UnityEngine;

namespace Bestiary.BMenu
{
    public class DescriptionPage
    {
        public BestiaryMenu bMenu;
        public string name;
        public FSprite icon, image;
        private MenuLabel _entityName, _entityDescriptionLabel, _emptinessLabel;
        public readonly EntityManager.EntityType entityType;
        public MenuLabel[] entityDescription, entityCharacteristicLabels;
        public Rect DescBox => bMenu.boxManager.boxes["descriptionBox"].Rectangle;
        public ICharacteristic characteristic;

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
                    break;
                case EntityManager.EntityType.Item:
                    break;
                case EntityManager.EntityType.Iterator:
                    break;
                default:
                    InitEmptiness();
                    break;
            }
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

            string imageName = $"description_{name.ToLower()}";
            if (Futile.atlasManager._allElementsByName.TryGetValue(imageName, out FAtlasElement element))
            {
                image = new FSprite(element);
                image.scale = Mathf.Min(0.9f * bMenu.boxManager.boxes["imageBox"].Rectangle.size.x / image.element.sourceSize.x,
                    0.9f * bMenu.boxManager.boxes["imageBox"].Rectangle.size.y / image.element.sourceSize.y);
            }
            else image = new FSprite("Sandbox_QuestionMark") { color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.Black), scale = 2 };

            image.SetPosition(bMenu.boxManager.boxes["imageBox"].Rectangle.center - BestiaryMenu.ResolutionOffset);
            bMenu.pages[0].Container.AddChild(image);
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
                entityCharacteristicLabels[i] = new MenuLabel(bMenu, bMenu.pages[0], lines[i], pos, Vector2.one, false);
                entityCharacteristicLabels[i].label.alignment = FLabelAlignment.Left;
                bMenu.pages[0].subObjects.Add(entityCharacteristicLabels[i]);
            }
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
            string[] lines;
            string path = AssetManager.ResolveFilePath($"{RWCustom.Custom.rainWorld.inGameTranslator.SpecificTextFolderDirectory()}{Path.DirectorySeparatorChar}{name.ToLower()}.txt");
            if (File.Exists(path))
                lines = File.ReadAllText(path).Split(new string[] { "<LINE>" }, System.StringSplitOptions.None);
            else lines = new string[] { "CAN'T FIND A CREATURE DESCRIPTION" };

            entityDescription = new MenuLabel[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                Vector2 pos = new Vector2(DescBox.position.x + 30f, _entityDescriptionLabel.pos.y - 30f * (i + 1.5f));
                entityDescription[i] = new MenuLabel(bMenu, bMenu.pages[0], lines[i], pos, Vector2.one, false);
                entityDescription[i].label.alignment = FLabelAlignment.Left;
                bMenu.pages[0].subObjects.Add(entityDescription[i]);
            }
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
                for (int i = 0; i < entityDescription.Length; i++)
                {
                    bMenu.pages[0].RemoveSubObject(entityDescription[i]);
                    entityDescription[i].RemoveSprites();
                }
            if (entityCharacteristicLabels != null)
                for (int i = 0; i < entityCharacteristicLabels.Length; i++)
                {
                    bMenu.pages[0].RemoveSubObject(entityCharacteristicLabels[i]);
                    entityCharacteristicLabels[i].RemoveSprites();
                }
        }
    }
}
