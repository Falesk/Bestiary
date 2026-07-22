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
        protected bool created;

        public BestiaryButton(ButtonManager buttonManager, string name, Vector2 nPos, Vector2 nSize, string text, bool hasIcon)
        {
            owner = buttonManager;
            this.name = name;
            normilizedPos = nPos;
            normilizedSize = nSize;
            if (hasIcon)
            {
                icon = new FSprite(text);
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

        public virtual void SetPosition(Vector2 nPos)
        {
            normilizedPos = nPos;
            button.pos = Rectangle.position;
            icon?.SetPosition(Rectangle.position + Rectangle.size / 2f - BM.ResolutionOffset);
        }

        public virtual void CreateButton()
        {
            if (created) return;

            button = new SimpleButton(owner.bMenu, owner.bMenu.pages[0], text, name, Rectangle.position, Rectangle.size);
            owner.bMenu.pages[0].subObjects.Add(button);
            if (icon != null)
                owner.bMenu.pages[0].Container.AddChild(icon);
            created = true;
            SetSelectables();
        }

        public virtual void Clear()
        {
            if (!created) return;
            owner.bMenu.pages[0].RemoveSubObject(button);
            button?.RemoveSprites();
            icon?.RemoveFromContainer();
        }
    }
}
