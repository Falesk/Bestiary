using UnityEngine;

namespace Bestiary.BMenu
{
    public class SlugcatButton : BestiaryButton
    {
        public SlugcatStats.Name slug;
        public SlugcatButton(ButtonManager buttonManager, SlugcatStats.Name slugName, int index, Vector2 nPos)
            : base(buttonManager, $"SLUGCAT_{index}", nPos, 0.05f * new Vector2(BM.Resolution.y / BM.Resolution.x, 1f), slugName == null ? "Sandbox_SmallQuestionmark" : "Kill_Slugcat", true)
        {
            slug = slugName;
        }

        protected override void CreateIcon()
        {
            base.CreateIcon();
            icon.color = slug != null ? PlayerGraphics.DefaultSlugcatColor(slug) : Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey);
            icon.SetPosition(button.pos + button.size / 2f);
            //FSprite slugSpite = new FSprite(hasSave ? "Kill_Slugcat" : "Sandbox_SmallQuestionmark")
            //{
            //    color = hasSave ? PlayerGraphics.DefaultSlugcatColor(name) : MenuRGB(MenuColors.DarkGrey),
            //    x = slugButton.pos.x + buttonSize / 2f,
            //    y = slugButton.pos.y + buttonSize / 2f
            //};
        }

        public override void Action()
        {
            base.Action();
        }
    }
}
