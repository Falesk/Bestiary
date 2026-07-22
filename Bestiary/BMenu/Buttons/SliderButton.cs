using UnityEngine;

namespace Bestiary.BMenu.Buttons
{
    public class SliderButton : BestiaryButton
    {
        public static Vector2 ButtonSize => new Vector2(25f, 25f) / BM.Resolution;
        public readonly bool down;

        public SliderButton(ButtonManager buttonManager, string name, Vector2 nPos) : base(buttonManager, name, nPos, ButtonSize, "Menu_Symbol_Arrow", true)
        {
            down = name.Contains("DOWN");
            SetIcon();
        }

        public override void Action()
        {
            owner.bMenu.slugcatManager.SliderClicked(down);
        }

        protected override void SetIcon()
        {
            icon.SetPosition(Rectangle.position + Rectangle.size / 2f - BM.ResolutionOffset);
            icon.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White);
            if (down) icon.rotation = 180f;
        }

        protected override void SetSelectables()
        {
            if (down)
                button.nextSelectable[3] = owner.backButton.button;
            else button.nextSelectable[1] = button;
            button.nextSelectable[0] = button;
            button.nextSelectable[2] = button;
        }
    }
}
