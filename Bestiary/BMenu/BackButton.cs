using UnityEngine;

namespace Bestiary.BMenu
{
    public class BackButton : BestiaryButton
    {
        public BackButton(ButtonManager buttonManager, Vector2 nPos, Vector2 nSize)
            : base(buttonManager, "BACK", nPos, nSize, buttonManager.bMenu.Translate("BACK"), false)
        {
        }

        public override void CreateButton()
        {
            base.CreateButton();
            owner.bMenu.backObject = button;
            SetSelectables();
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
