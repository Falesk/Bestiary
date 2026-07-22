using Menu;
using UnityEngine;

namespace Bestiary.BMenu
{
    public abstract class BestiaryButton
    {
        public ButtonManager owner;
        public SimpleButton button;
        public Rect Rectangle => new Rect(BM.Resolution * normilizedPos + BM.ResolutionOffset, BM.Resolution * normilizedSize);
        public Vector2 normilizedPos, normilizedSize;
        public string name;
        public string text;
        public FSprite icon;

        public BestiaryButton(ButtonManager buttonManager, string name, Vector2 nPos, Vector2 nSize, string text, bool hasIcon)
        {
            owner = buttonManager;
            this.name = name;
            normilizedPos = nPos;
            normilizedSize = nSize;
            if (hasIcon)
            {
                CreateIcon();
                this.text = string.Empty;
            }
            else
            {
                icon = null;
                this.text = text;
            }
        }

        public virtual void Action()
        {
            owner.bMenu.PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);
        }

        protected virtual void SetSelectables()
        {
        }

        protected virtual void CreateIcon()
        {
            icon = new FSprite(text);
        }

        public virtual void CreateButton()
        {
            button = new SimpleButton(owner.bMenu, owner.bMenu.pages[0], text, name, Rectangle.position, Rectangle.size);
            owner.bMenu.pages[0].subObjects.Add(button);
            if (icon != null)
                owner.bMenu.pages[0].Container.AddChild(icon);
        }

        public virtual void Clear()
        {
            owner.bMenu.pages[0].RemoveSubObject(button);
            button?.RemoveSprites();
            icon?.RemoveFromContainer();
        }
    }
}
