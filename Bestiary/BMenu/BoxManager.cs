using UnityEngine;
using Menu;
using System.Collections.Generic;

namespace Bestiary.BMenu
{
    public class BoxManager
    {
        public BM bMenu;
        public Dictionary<string, Box> boxes;

        public BoxManager(BM owner)
        {
            bMenu = owner;
            boxes = new Dictionary<string, Box>();
        }

        public void CreateBox(string name, Vector2 normPos, Vector2 normSize, Color color, float alpha = 1)
        {
            Box b = new Box(this, normPos, normSize)
            {
                fillColor = color,
                fillAlpha = alpha
            };

            boxes.Add(name, b);
            b.Init();
        }

        public void Clear()
        {
            foreach (var box in boxes.Values)
                box.Clear();
            boxes.Clear();
        }

        public class Box
        {
            public BoxManager owner;
            public Rect Rectangle => new Rect(BM.Resolution * normilizedPos + BM.ResolutionOffset, BM.Resolution * normilizedSize);
            public Vector2 normilizedPos, normilizedSize;
            public RoundedRect boxBorder;
            public Color fillColor;
            public float fillAlpha;

            public Box(BoxManager owner, Vector2 nPos, Vector2 nSize)
            {
                this.owner = owner;
                normilizedPos = nPos;
                normilizedSize = nSize;
                fillColor = Color.black;
                fillAlpha = 1f;
            }

            public void Init()
            {
                boxBorder = new RoundedRect(owner.bMenu, owner.bMenu.pages[0], Rectangle.position, Rectangle.size, true);

                for (int i = 0; i < boxBorder.SideSprite(0); i++)
                    boxBorder.sprites[i].color = fillColor;
                boxBorder.fillAlpha = fillAlpha;
                owner.bMenu.pages[0].subObjects.Add(boxBorder);
            }

            public void Clear()
            {
                owner.bMenu.pages[0].RemoveSubObject(boxBorder);
                boxBorder?.RemoveSprites();
            }
        }
    }
}
