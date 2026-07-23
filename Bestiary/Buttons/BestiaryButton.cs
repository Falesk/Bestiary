using Menu;
using UnityEngine;
using Bestiary.BMenu;

namespace Bestiary.Buttons
{
    public abstract class BestiaryButton
    {
        public ButtonManager owner;
        public SimpleButton button;
        public Rect Rectangle => new Rect(BestiaryMenu.Resolution * normilizedPos + BestiaryMenu.ResolutionOffset, BestiaryMenu.Resolution * normilizedSize);
        public Vector2 normilizedPos, normilizedSize;
        public string name;
        public string text;
        public FSprite icon;
        protected bool created;
        public bool IsVisible { get; private set; }

        public BestiaryButton(ButtonManager buttonManager, string name, Vector2 nPos, Vector2 nSize, string text, bool hasIcon)
        {
            owner = buttonManager;
            this.name = name;
            normilizedPos = nPos;
            normilizedSize = nSize;
            IsVisible = true;
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

        protected virtual void SetIcon()
        {
        }

        protected virtual void SetSelectables()
        {
        }

        public void SetPosition(Vector2 nPos)
        {
            normilizedPos = nPos;
            button.pos = Rectangle.position;
            icon?.SetPosition(Rectangle.position + Rectangle.size / 2f - BestiaryMenu.ResolutionOffset);
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

        public void Clear()
        {
            if (!created) return;
            owner.bMenu.pages[0].RemoveSubObject(button);
            button?.RemoveSprites();
            icon?.RemoveFromContainer();
        }

        public void ChangeVisibility(bool value)
        {
            for (int i = 0; i < button.roundedRect.sprites.Length; i++)
                button.roundedRect.sprites[i].isVisible = value;
            for (int i = 0; i < button.selectRect.sprites.Length; i++)
                button.selectRect.sprites[i].isVisible = value;
            if (icon != null)
                icon.alpha = value ? 1f : 0f;
        }
    }
}
