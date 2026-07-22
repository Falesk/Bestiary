using UnityEngine;
using System.Collections.Generic;

namespace Bestiary.BMenu.Buttons
{
    public class ButtonManager
    {
        public BM bMenu;
        public BackButton backButton;
        public SlugcatButton[] slugcatButtons;
        public List<EntityButton> entityButtons;
        public SliderButton upButton, downButton;
        public PagerButton nextButton, prevButton;
        public Vector2 firstSlugcatButtonPos, slugcatOffset;

        public ButtonManager(BM owner)
        {
            bMenu = owner;
            entityButtons = new List<EntityButton>();
        }

        public void CreateBackButton(Vector2 nPos)
        {
            backButton = new BackButton(this, nPos);
            backButton.CreateButton();
        }

        public void CreatePagerButtons()
        {
            Vector2 center = bMenu.boxManager.boxes["selectorBox"].normilizedPos + 0.75f * Vector2.right * bMenu.boxManager.boxes["selectorBox"].normilizedSize.x;
            center.y += 10f / BM.Resolution.y;

            nextButton = new PagerButton(this, "PAGER_NEXT", center + Vector2.right * 20f / BM.Resolution.x);
            nextButton.CreateButton();
            prevButton = new PagerButton(this, "PAGER_PREV", center + Vector2.left * 20f / BM.Resolution.x);
            prevButton.CreateButton();

            nextButton.UpdateSelectables();
            prevButton.UpdateSelectables();
            nextButton.button.buttonBehav.greyedOut = true;
            prevButton.button.buttonBehav.greyedOut = true;
        }

        public void CreateSliderButtons()
        {
            Vector2 nPos = firstSlugcatButtonPos + new Vector2(SlugcatButton.ButtonSize.x * 0.5f, SlugcatButton.ButtonSize.y) - 0.5f * SliderButton.ButtonSize + Vector2.up * SliderButton.ButtonSize.y;
            upButton = new SliderButton(this, "SLIDER_UP", nPos);
            upButton.CreateButton();
            upButton.button.buttonBehav.greyedOut = true;

            nPos = firstSlugcatButtonPos - slugcatOffset * (SlugcatManager.slugsInColumn - 1) + new Vector2(SlugcatButton.ButtonSize.x * 0.5f, 0) - 0.5f * SliderButton.ButtonSize + Vector2.down * SliderButton.ButtonSize.y;
            downButton = new SliderButton(this, "SLIDER_DOWN", nPos);
            downButton.CreateButton();
        }

        public void CreateSlugcatButtons(Vector2 firstPos, Vector2 offset)
        {

            firstSlugcatButtonPos = firstPos;
            slugcatOffset = offset;
            slugcatButtons = new SlugcatButton[bMenu.slugcatManager.Saves.Length];
            for (int i = 0; i < slugcatButtons.Length; i++)
            {
                slugcatButtons[i] = new SlugcatButton(this, firstSlugcatButtonPos + slugcatOffset * i, bMenu.slugcatManager.Saves[i], i);
                slugcatButtons[i].CreateButton();
            }

            RefreshSlugcats(0);
        }

        public void CreateEntityButton(int index, IconSymbol.IconSymbolData icon, EntityManager.EntityType entityType)
        {
            BoxManager.Box buttonBox = bMenu.boxManager.boxes["entitiesBox"];
            float xOffset = 0.5f * (buttonBox.normilizedSize.x - EntityManager.buttonsInRow * EntityButton.ButtonSize.x) / EntityManager.buttonsInRow;
            float yOffset = -0.5f * (buttonBox.normilizedSize.y - EntityManager.buttonsInColumn * EntityButton.ButtonSize.y) / EntityManager.buttonsInColumn;
            float nX = buttonBox.normilizedPos.x + (index % EntityManager.buttonsInRow) * buttonBox.normilizedSize.x / EntityManager.buttonsInRow + xOffset;
            float nY = buttonBox.normilizedPos.y + buttonBox.normilizedSize.y - EntityButton.ButtonSize.y - (index / EntityManager.buttonsInRow) * (buttonBox.normilizedSize.y / EntityManager.buttonsInColumn) + yOffset;

            Vector2 nPos = new Vector2(nX, nY);

            string idName = entityType == EntityManager.EntityType.Creature ? $"ENTITY_CRIT_{index}" : $"ENTITY_ITEM_{index}";

            EntityButton button = new EntityButton(this, idName, nPos, icon, entityType);
            button.CreateButton();
            entityButtons.Add(button);
        }

        public void ClearEntityButtons()
        {
            for (int i = 0; i < entityButtons.Count; i++)
                entityButtons[i].Clear();
            entityButtons.Clear();
        }

        public void RefreshSlugcats(int slugcatSlideNum)
        {
            for (int i = 0; i < slugcatButtons.Length; i++)
            {
                slugcatButtons[i].button.buttonBehav.greyedOut = i < slugcatSlideNum || i - slugcatSlideNum >= SlugcatManager.slugsInColumn/* || slugcats[i].kills == null*/;
                slugcatButtons[i].ChangeVisibility(!slugcatButtons[i].button.buttonBehav.greyedOut);
                slugcatButtons[i].SetPosition(firstSlugcatButtonPos + (slugcatSlideNum - i) * slugcatOffset);
            }
        }

        public void Clear()
        {
            backButton?.Clear();
            for (int i = 0; i < slugcatButtons.Length; i++)
                slugcatButtons[i]?.Clear();
            upButton?.Clear();
            downButton?.Clear();
            nextButton?.Clear();
            prevButton?.Clear();
        }

        public void SlugcatButtonToggles(int selectedSlugcat)
        {
            for (int i = 0; i < slugcatButtons.Length; i++)
                slugcatButtons[i].button.toggled = false;
            for (int i = 0; i < entityButtons.Count; i++)
                entityButtons[i].button.toggled = false;
            if (selectedSlugcat > 0 && selectedSlugcat < slugcatButtons.Length)
                slugcatButtons[selectedSlugcat].button.toggled = true;
        }

        public void EntityButtonToggles(int selectedEntity)
        {
            for (int i = 0; i < slugcatButtons.Length; i++)
                slugcatButtons[i].button.toggled = false;
            for (int i = 0; i < entityButtons.Count; i++)
                entityButtons[i].button.toggled = false;
            int selectedButton = selectedEntity - EntityManager.buttonsInColumn * EntityManager.buttonsInRow * bMenu.entityManager.CurrentPage;
            if (selectedButton > -1 && selectedButton < entityButtons.Count)
                entityButtons[selectedButton].button.toggled = true;
        }
    }
}
