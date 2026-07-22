using UnityEngine;

namespace Bestiary.BMenu
{
    public class SlugcatButton : BestiaryButton
    {
        public SlugcatInfo slug;
        public int index;
        public static Vector2 ButtonSize => 0.05f * new Vector2(BM.Resolution.y / BM.Resolution.x, 1f);
        //public static Vector2 FirstButtonPos => new Vector2(0.04f, 0.825f);

        public SlugcatButton(ButtonManager buttonManager, Vector2 nPos, SlugcatInfo slugInfo, int index)
            : base(buttonManager, $"SLUGCAT_{index}", nPos, ButtonSize, slugInfo.kills != null ? "Kill_Slugcat" : "Sandbox_SmallQuestionmark", true)
        {
            slug = slugInfo;
            this.index = index;
            SetIcon();
        }

        private void SetIcon()
        {
            icon.color = slug.kills != null ? PlayerGraphics.DefaultSlugcatColor(slug.name) : Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey);
            icon.SetPosition(Rectangle.position + Rectangle.size / 2f - BM.ResolutionOffset);
        }

        protected override void SetSelectables()
        {
            button.nextSelectable[0] = button;
            button.nextSelectable[2] = button;
        }

        public override void Action()
        {
            base.Action();
            owner.bMenu.slugcatManager.Action(index);
        }
    }
}
