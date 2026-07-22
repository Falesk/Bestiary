using UnityEngine;

namespace Bestiary.BMenu.Buttons
{
    public class PagerButton : BestiaryButton
    {
        public static Vector2 ButtonSize => new Vector2(25f, 25f) / BM.Resolution;
        public readonly bool next;

        public PagerButton(ButtonManager buttonManager, string name, Vector2 nPos) : base(buttonManager, name, nPos, ButtonSize, "Menu_Symbol_Arrow", true)
        {
            next = name.Contains("NEXT");
            SetIcon();
        }

        public override void Action()
        {
            owner.bMenu.entityManager.PagerClicked(next);
            owner.EntityButtonToggles(owner.bMenu.entityManager.SelectedEntity);
        }

        protected override void SetIcon()
        {
            icon.SetPosition(Rectangle.position + Rectangle.size / 2f - BM.ResolutionOffset);
            icon.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey);
            if (next) icon.rotation = 90f;
            else icon.rotation = -90f;
        }

        public void UpdateSelectables()
        {
            if (next)
                button.nextSelectable[0] = owner.prevButton.button;
            else button.nextSelectable[2] = owner.nextButton.button;
            button.nextSelectable[3] = owner.backButton.button;
        }
    }
}
