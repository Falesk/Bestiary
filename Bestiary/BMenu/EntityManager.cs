using Menu;
using System.Collections.Generic;
using UnityEngine;

namespace Bestiary.BMenu
{
    public class EntityManager
    {
        public BestiaryMenu bMenu;
        public const int buttonsInColumn = 8, buttonsInRow = 4;
        public int PagesTotal => Mathf.CeilToInt(EntitiesTotal / (float)(buttonsInRow * buttonsInColumn));
        public int CurrentPage => _entityPageNum;
        public int EntitiesTotal => _currentEntities.Count;
        public int SelectedEntity { get; private set; }

        private int _entityPageNum;
        private readonly MenuLabel _emptinessLabel, _pageLabel;
        private readonly List<SaveInfo.Info> _currentEntities;

        public EntityManager(BestiaryMenu owner)
        {
            bMenu = owner;
            SelectedEntity = -1;
            _entityPageNum = 0;

            _currentEntities = new List<SaveInfo.Info>();

            Rect box = bMenu.boxManager.boxes["selectorBox"].Rectangle;
            Vector2 labelPos = box.position + box.size * 0.5f;
            _emptinessLabel = new MenuLabel(bMenu, bMenu.pages[0], Plugin.Translate("[ No Entries ]"), labelPos, Vector2.zero, false);
            _emptinessLabel.label.alignment = FLabelAlignment.Center;
            bMenu.pages[0].subObjects.Add(_emptinessLabel);

            labelPos = box.position + new Vector2(box.size.x * 0.5f, 25f);
            _pageLabel = new MenuLabel(bMenu, bMenu.pages[0], string.Empty, labelPos, Vector2.zero, false);
            _pageLabel.label.alignment = FLabelAlignment.Right;
            bMenu.pages[0].subObjects.Add(_pageLabel);

            UpdatePageLabel(true);
        }

        public SaveInfo.Info GetEntityByIndex(int index) => _currentEntities[index + buttonsInRow * buttonsInColumn * _entityPageNum];
        public SaveInfo.Info GetEntityByRealIndex(int index) => _currentEntities[index];

        public void UpdateEmptinessLabel(bool show) => _emptinessLabel.text = show ? Plugin.Translate("[ No Entries ]") : string.Empty;
        public void UpdatePageLabel(bool show) => _pageLabel.text = show ?
            Plugin.Translate("Page $ of %")
            .Replace("$", Mathf.Max(_entityPageNum + 1, PagesTotal).ToString())
            .Replace("%", PagesTotal.ToString()) : string.Empty;

        public void LoadEntities(SaveInfo save)
        {
            _currentEntities.Clear();
            bMenu.buttonManager.ClearEntityButtons();

            if ((save.kills == null || save.kills.Count == 0) && (save.items == null || save.items.Count == 0))
            {
                UpdateEmptinessLabel(true);
                return;
            }
            UpdateEmptinessLabel(false);

            if (save.kills != null)
            {
                for (int i = 0; i < save.kills.Count; i++)
                    _currentEntities.Add(save.kills[i]);
            }

            if (save.items != null)
            {
                for (int i = 0; i < save.items.Count; i++)
                    _currentEntities.Add(save.items[i]);
            }

            for (int i = 0; i < buttonsInRow * buttonsInColumn; i++)
            {
                if (_currentEntities.Count <= i + buttonsInRow * buttonsInColumn * _entityPageNum) continue;

                IconSymbol.IconSymbolData data = _currentEntities[i + buttonsInRow * buttonsInColumn * _entityPageNum].iconData;
                bMenu.buttonManager.CreateEntityButton(i, data, data.critType == CreatureTemplate.Type.StandardGroundCreature ?
                    (data.itemType == AbstractPhysicalObject.AbstractObjectType.Creature ? EntityType.None : EntityType.Item) : EntityType.Creature);
            }
        }

        public void ButtonClicked(int index)
        {
            SelectedEntity = index + buttonsInRow * buttonsInColumn * _entityPageNum;
            bMenu.buttonManager.EntityButtonToggles(SelectedEntity);
        }

        public void PagerClicked(bool next)
        {
            if (next && (_entityPageNum + 1) * buttonsInRow * buttonsInColumn < EntitiesTotal)
                _entityPageNum++;
            else _entityPageNum -= (_entityPageNum == 0) ? 0 : 1;

            if (bMenu.slugcatManager.SelectedSlugcat != -1)
                LoadEntities(bMenu.slugcatManager.Saves[bMenu.slugcatManager.SelectedSlugcat]);
            UpdatePagerButtons();
            UpdatePageLabel(true);
        }

        public void UpdatePagerButtons()
        {
            bool flag = (_entityPageNum + 1) * buttonsInRow * buttonsInColumn >= EntitiesTotal;
            bMenu.buttonManager.nextButton.button.buttonBehav.greyedOut = flag;
            bMenu.buttonManager.nextButton.icon.color = flag ? Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey) : Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White);
            bMenu.buttonManager.prevButton.button.buttonBehav.greyedOut = _entityPageNum == 0;
            bMenu.buttonManager.prevButton.icon.color = _entityPageNum == 0 ? Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey) : Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White);
        }

        public void SetEntityPageNum(int value) => _entityPageNum = value;
        public void SetSelectedEntity(int value) => SelectedEntity = value;

        public void Clear()
        {
            bMenu.pages[0].RemoveSubObject(_emptinessLabel);
            _emptinessLabel.RemoveSprites();
            bMenu.pages[0].RemoveSubObject(_pageLabel);
            _pageLabel.RemoveSprites();
            bMenu.buttonManager.ClearEntityButtons();
        }

        public enum EntityType
        {
            Creature,
            Slugcat,
            Item,
            Iterator,
            None
        }
    }
}
