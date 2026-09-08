using UnityEngine;
using Bestiary.BMenu;

namespace Bestiary.Buttons
{
    public class EntityButton : BestiaryButton
    {
        public static Vector2 ButtonSize => new Vector2(50f, 50f) / BestiaryMenu.Resolution;

        private readonly SaveInfo.Info info;
        private readonly SaveInfo.Info.KilledInfo creatureInfo;
        private readonly IconSymbol.IconSymbolData iconData;
        private string customIconElement;
        private int lastAnimatedVariant = -1;

        public readonly int index;
        public readonly EntityManager.EntityType entityType;

        public virtual bool NewlyUnlocked { get; set; }

        public EntityButton(
            ButtonManager buttonManager,
            string name,
            Vector2 nPos,
            SaveInfo.Info info,
            EntityManager.EntityType entityType)
            : base(
                buttonManager,
                name,
                nPos,
                ButtonSize,
                InitialSpriteName(info, entityType),
                true)
        {
            index = int.Parse(name.Substring(name.LastIndexOf('_') + 1));
            this.info = info;
            iconData = info.iconData;
            creatureInfo = info as SaveInfo.Info.KilledInfo;
            this.entityType = entityType;

            if (creatureInfo != null)
                BestiaryAssets.TryGetCustomElement(creatureInfo.customIconPath, out customIconElement);

            SetIcon();
        }

        private static string InitialSpriteName(SaveInfo.Info info, EntityManager.EntityType entityType)
        {
            if (entityType == EntityManager.EntityType.Creature)
                return CreatureSymbol.SpriteNameOfCreature(info.iconData);

            return ItemSymbol.SpriteNameForItem(info.iconData.itemType, info.iconData.intData);
        }

        protected override void SetIcon()
        {
            if (entityType == EntityManager.EntityType.Creature)
            {
                if (!string.IsNullOrEmpty(customIconElement))
                {
                    icon.SetElementByName(customIconElement);
                    icon.color = Color.white;
                    icon.scale = 1f;
                }
                else
                {
                    ApplyCreatureVariant(GetCurrentCreatureVariant());
                }
            }
            else
            {
                icon.color = ItemSymbol.ColorForItem(iconData.itemType, iconData.intData);
            }

            icon.SetPosition(Rectangle.position + Rectangle.size / 2f - BestiaryMenu.ResolutionOffset);
        }

        private IconSymbol.IconSymbolData GetCurrentCreatureVariant()
        {
            if (creatureInfo != null &&
                creatureInfo.iconVariants != null &&
                creatureInfo.iconVariants.Count > 0)
            {
                return creatureInfo.iconVariants[0];
            }

            return iconData;
        }

        private void ApplyCreatureVariant(IconSymbol.IconSymbolData data)
        {
            try
            {
                icon.SetElementByName(CreatureSymbol.SpriteNameOfCreature(data));
                icon.color = CreatureSymbol.ColorOfCreature(data);
                icon.scale = 1f;
            }
            catch (System.Exception ex)
            {
                Plugin.logger.LogWarning(
                    $"[Bestiary] Failed to render creature icon {data.critType?.value} data={data.intData}: {ex.Message}");
            }
        }

        public void UpdateAnimatedIcon()
        {
            if (entityType != EntityManager.EntityType.Creature ||
                creatureInfo == null ||
                !string.IsNullOrEmpty(customIconElement) ||
                creatureInfo.iconVariants == null ||
                creatureInfo.iconVariants.Count <= 1)
            {
                return;
            }

            int variant = CreatureCatalog.GetAnimatedVariantIndex(creatureInfo.iconVariants.Count);
            if (variant == lastAnimatedVariant)
                return;

            lastAnimatedVariant = variant;
            ApplyCreatureVariant(creatureInfo.iconVariants[variant]);
            icon.SetPosition(Rectangle.position + Rectangle.size / 2f - BestiaryMenu.ResolutionOffset);
        }

        public override void CreateButton()
        {
            base.CreateButton();
            if (NewlyUnlocked)
                button.rectColor = Menu.Menu.MenuColor(Menu.Menu.MenuColors.SaturatedGold);
            else
                button.rectColor = Menu.Menu.MenuColor(Menu.Menu.MenuColors.White);
        }

        public override void Action()
        {
            base.Action();
            NewlyUnlocked = false;
            button.rectColor = Menu.Menu.MenuColor(Menu.Menu.MenuColors.White);
            owner.bMenu.entityManager.ButtonClicked(index);
        }
    }
}
