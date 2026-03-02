using UnityEngine;

namespace Bestiary
{
    public class KillingNotify : CosmeticSprite
    {
        public CreatureTemplate.Type creatureType;
        public Vector2 Pos => new Vector2(1200f, 200f);
        public int LifeTime => 100;

        public KillingNotify(Room _room, CreatureTemplate.Type victimType) : base()
        {
            creatureType = victimType;
            room = _room;

            room.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom);
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (!room.BeingViewed)
                Destroy();
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            IconSymbol.IconSymbolData iconSymbolData = new IconSymbol.IconSymbolData(creatureType, AbstractPhysicalObject.AbstractObjectType.Creature, 0);

            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new FSprite(CreatureSymbol.SpriteNameOfCreature(iconSymbolData))
            {
                color = CreatureSymbol.ColorOfCreature(iconSymbolData)
            };

            AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            foreach (FSprite sprite in sLeaser.sprites)
                sprite.SetPosition(pos - camPos);
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            newContatiner = newContatiner ?? rCam.ReturnFContainer("HUD");
            foreach (FSprite sprite in sLeaser.sprites)
            {
                sprite.RemoveFromContainer();
                newContatiner.AddChild(sprite);
            }
        }
    }
}
