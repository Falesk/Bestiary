using UnityEngine;
using Bestiary.BMenu;

namespace Bestiary.Buttons
{
    public class EntityButton : BestiaryButton
    {
        public static Vector2 ButtonSize => new Vector2(50f, 50f) / BestiaryMenu.Resolution;
        private readonly IconSymbol.IconSymbolData iconData;
        public readonly int index;

        public EntityButton(ButtonManager buttonManager, string name, Vector2 nPos, IconSymbol.IconSymbolData iconSymbolData)
            : base(buttonManager, name, nPos, ButtonSize, CreatureSymbol.SpriteNameOfCreature(iconSymbolData), true)
        {
            index = int.Parse(name.Substring(name.LastIndexOf('_') + 1));
            iconData = iconSymbolData;
            SetIcon();
        }

        protected override void SetIcon()
        {
            icon.color = CreatureSymbol.ColorOfCreature(iconData);
            icon.SetPosition(Rectangle.position + Rectangle.size / 2f - BestiaryMenu.ResolutionOffset);
        }

        public override void Action()
        {
            base.Action();
            owner.bMenu.entityManager.ButtonClicked(index);
        }
    }
}
