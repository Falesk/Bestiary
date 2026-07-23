using UnityEngine;
using Bestiary.BMenu;

namespace Bestiary.Buttons
{
    public class SlugcatButton : BestiaryButton
    {
        public SaveInfo save;
        public int index;
        public static Vector2 ButtonSize => new Vector2(40f, 40f) / BestiaryMenu.Resolution;

        public SlugcatButton(ButtonManager buttonManager, Vector2 nPos, SaveInfo slugInfo, int index)
            : base(buttonManager, $"SLUGCAT_{index}", nPos, ButtonSize, slugInfo.kills != null ? "Kill_Slugcat" : "Sandbox_SmallQuestionmark", true)
        {
            save = slugInfo;
            this.index = index;
            SetIcon();
        }

        protected override void SetIcon()
        {
            icon.color = save.kills != null ? PlayerGraphics.DefaultSlugcatColor(save.name) : Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey);
            icon.SetPosition(Rectangle.position + Rectangle.size / 2f - BestiaryMenu.ResolutionOffset);
        }

        public override void Action()
        {
            base.Action();
            owner.bMenu.entityManager.LoadEntities(save);
            owner.bMenu.slugcatManager.ButtonClicked(index);
        }
    }
}