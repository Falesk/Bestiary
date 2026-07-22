using UnityEngine;

namespace Bestiary.BMenu.Buttons
{
    public class EntityButton : BestiaryButton
    {
        public static Vector2 ButtonSize => new Vector2(50f, 50f) / BM.Resolution;
        public readonly EntityManager.EntityType entityType;
        private readonly IconSymbol.IconSymbolData iconData;
        public readonly int index;

        public EntityButton(ButtonManager buttonManager, string name, Vector2 nPos, IconSymbol.IconSymbolData iconSymbolData, EntityManager.EntityType type)
            : base(buttonManager, name, nPos, ButtonSize, CreatureSymbol.SpriteNameOfCreature(iconSymbolData), true)
        {
            index = int.Parse(name.Substring(name.LastIndexOf('_') + 1));
            entityType = type;
            iconData = iconSymbolData;
            SetIcon();
        }

        protected override void SetIcon()
        {
            icon.color = CreatureSymbol.ColorOfCreature(iconData);
            icon.SetPosition(Rectangle.position + Rectangle.size / 2f - BM.ResolutionOffset);
        }

        public override void Action()
        {
            base.Action();
            owner.bMenu.entityManager.ButtonClicked(index);
        }
    }
}
