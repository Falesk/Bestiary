using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Menu;

namespace Bestiary.BMenu
{
    public class CreatureDescriptionPage
    {
        private readonly string name;
        private readonly BestiaryMenu menu;
        private readonly RoundedRect imageBox;
        private readonly FSprite icon, image;
        private readonly MenuLabel entityName, entityDescriptionLabel;
        private MenuLabel[] entityDescription, entityCharacteristicLabels;
        public readonly Rect descriptionBoxRect;
        public Characteristic characteristic;

        public CreatureDescriptionPage(BestiaryMenu owner, IconSymbol.IconSymbolData iconData, Rect descRect)
        {
            menu = owner;
            name = iconData.critType.ToString();
            icon = new FSprite(CreatureSymbol.SpriteNameOfCreature(iconData))
            {
                color = CreatureSymbol.ColorOfCreature(iconData),
                scale = 2f
            };
            descriptionBoxRect = descRect;
            icon.SetPosition(descriptionBoxRect.position + new Vector2(50f, descriptionBoxRect.y - 50f));
            menu.pages[0].Container.AddChild(icon);

            entityName = new MenuLabel(menu, menu.pages[0], Plugin.ResolveCreatureName(name), descriptionBoxRect.position + new Vector2(100f, descriptionBoxRect.y - 50f), Vector2.one, true);
            entityName.label.alignment = FLabelAlignment.Left;
            menu.pages[0].subObjects.Add(entityName);

            Vector2 descrPos = descriptionBoxRect.position + descriptionBoxRect.x / 2f * Vector2.right + descriptionBoxRect.y / 2.2f * Vector2.up;
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
            else image = new FSprite("Sandbox_QuestionMark") { color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.Black) };
            image.SetPosition(boxPos + boxSize / 2f);
            menu.pages[0].Container.AddChild(image);

            GetDescription();
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
                Vector2 pos = new Vector2(descriptionBoxRect.position.x + 40f, entityDescriptionLabel.pos.y - 30f * (i + 1.5f));
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
                Vector2 pos = descriptionBoxRect.position + new Vector2(30f, descriptionBoxRect.y - 100f - 20f * i);
                entityCharacteristicLabels[i] = new MenuLabel(menu, menu.pages[0], lines[i], pos, Vector2.one, false);
                entityCharacteristicLabels[i].label.alignment = FLabelAlignment.Left;
                menu.pages[0].subObjects.Add(entityCharacteristicLabels[i]);
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
    }
}
