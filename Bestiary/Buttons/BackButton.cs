using UnityEngine;
using Bestiary.BMenu;

namespace Bestiary.Buttons
{
    public class BackButton : BestiaryButton
    {
        public static Vector2 ButtonSize => new Vector2(110f, 30f) / BestiaryMenu.Resolution;

        public BackButton(ButtonManager buttonManager, Vector2 nPos)
            : base(buttonManager, "BACK", nPos, ButtonSize, Plugin.Translate("BACK"), false)
        {
        }

        public override void CreateButton()
        {
            base.CreateButton();
            owner.bMenu.backObject = button;
        }

        public override void Action()
        {
            owner.bMenu.Exit();
        }

        protected override void SetSelectables()
        {
            button.nextSelectable[0] = button;
            button.nextSelectable[2] = button;
            button.nextSelectable[3] = button;
        }
    }
}
