using UnityEngine;
using Bestiary.BMenu;

namespace Bestiary.Buttons
{
    public class EntityButton : BestiaryButton
    {
        public static Vector2 ButtonSize => new Vector2(50f, 50f) / BestiaryMenu.Resolution;
        private readonly IconSymbol.IconSymbolData iconData;
        public readonly int index;
        public readonly EntityManager.EntityType entityType;

        public virtual bool NewlyUnlocked { get; set; }

        public EntityButton(ButtonManager buttonManager, string name, Vector2 nPos, IconSymbol.IconSymbolData iconSymbolData, EntityManager.EntityType entityType)
            : base(buttonManager, name, nPos, ButtonSize, entityType == EntityManager.EntityType.Creature ?
                  CreatureSymbol.SpriteNameOfCreature(iconSymbolData) :
                  ItemSymbol.SpriteNameForItem(iconSymbolData.itemType, iconSymbolData.intData), true)
        {
            index = int.Parse(name.Substring(name.LastIndexOf('_') + 1));
            iconData = iconSymbolData;
            this.entityType = entityType;
            SetIcon();
        }

        protected override void SetIcon()
        {
            icon.color = entityType == EntityManager.EntityType.Creature ? CreatureSymbol.ColorOfCreature(iconData) : ItemSymbol.ColorForItem(iconData.itemType, iconData.intData);
            icon.SetPosition(Rectangle.position + Rectangle.size / 2f - BestiaryMenu.ResolutionOffset);
        }

        public override void CreateButton()
        {
            base.CreateButton();
            if (NewlyUnlocked)
                button.rectColor = Menu.Menu.MenuColor(Menu.Menu.MenuColors.SaturatedGold);
            else button.rectColor = Menu.Menu.MenuColor(Menu.Menu.MenuColors.White);
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
