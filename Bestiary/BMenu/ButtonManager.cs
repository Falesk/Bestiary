using UnityEngine;

namespace Bestiary.BMenu
{
    public class ButtonManager
    {
        public BM bMenu;
        public BackButton backButton;

        public ButtonManager(BM owner)
        {
            bMenu = owner;
        }

        public void CreateBackButton(Vector2 nPos, Vector2 nSize)
        {
            backButton = new BackButton(this, nPos, nSize);
            backButton.CreateButton();
        }

        public void Clear()
        {
            backButton.Clear();
        }
    }
}
