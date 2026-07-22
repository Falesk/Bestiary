using UnityEngine;
using Menu;

namespace Bestiary.BMenu
{
    public class ButtonManager
    {
        public BM bMenu;
        public BackButton backButton;
        public SlugcatButton[] slugcatButtons;
        private bool backButtonLoaded, slugcatButtonsLoaded;

        public Vector2 firstSlugcatButtonPos, slugcatOffset;

        public ButtonManager(BM owner)
        {
            bMenu = owner;
        }

        public void CreateBackButton(Vector2 nPos, Vector2 nSize)
        {
            if (backButtonLoaded) return;
            backButton = new BackButton(this, nPos, nSize);
            backButton.CreateButton();
            backButtonLoaded = true;
        }

        public void CreateSlugcatButtons(Vector2 firstButtonPos, Vector2 offset)
        {
            if (slugcatButtonsLoaded) return;
            slugcatButtonsLoaded = true;

            firstSlugcatButtonPos = firstButtonPos;
            slugcatOffset = offset;
            slugcatButtons = new SlugcatButton[bMenu.slugcatManager.Slugcats.Length];
            for (int i = 0; i < slugcatButtons.Length; i++)
            {
                slugcatButtons[i] = new SlugcatButton(this, firstSlugcatButtonPos + slugcatOffset * i, bMenu.slugcatManager.Slugcats[i], i);
                slugcatButtons[i].CreateButton();
            }

            RefreshSlugcats(0);
        }

        public void RefreshSlugcats(int slugcatSlideNum)
        {
            for (int i = 0; i < slugcatButtons.Length; i++)
            {
                slugcatButtons[i].button.buttonBehav.greyedOut = i < slugcatSlideNum || i - slugcatSlideNum >= SlugcatManager.slugsInColumn/* || slugcats[i].kills == null*/;
                ToggleButtonVisibility(slugcatButtons[i].button, !slugcatButtons[i].button.buttonBehav.greyedOut);
                slugcatButtons[i].SetPosition(firstSlugcatButtonPos + (slugcatSlideNum - i) * slugcatOffset);
                //slugcatButtons[i].icon.SetPosition(slugcatButtons[i].button.pos + slugcatButtons[i].button.size / 2f);
                slugcatButtons[i].icon.alpha = slugcatButtons[i].button.buttonBehav.greyedOut ? 0f : 1f;
            }
        }

        public void UpdateButtonToggles(int choosedSlugcat)
        {
            for (int i = 0; i < slugcatButtons.Length; i++)
                slugcatButtons[i].button.toggled = false;
            //for (int i = 0; i < entityButtons.Length; i++)
            //    entityButtons[i].toggled = false;
            slugcatButtons[choosedSlugcat].button.toggled = true;
        }

        public void Clear()
        {
            if (backButtonLoaded)
                backButton.Clear();
            if (slugcatButtonsLoaded)
                for (int i = 0; i < slugcatButtons.Length; i++)
                    slugcatButtons[i].Clear();
        }

        private void ToggleButtonVisibility(SimpleButton button, bool isVisible)
        {
            for (int i = 0; i < button.roundedRect.sprites.Length; i++)
                button.roundedRect.sprites[i].isVisible = isVisible;
            for (int i = 0; i < button.selectRect.sprites.Length; i++)
                button.selectRect.sprites[i].isVisible = isVisible;
        }
    }
}
