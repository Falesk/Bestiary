using RWCustom;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bestiary
{
    public class KillingNotify : CosmeticSprite
    {
        public CreatureTemplate.Type creatureType;
        private const float screenEdgeOffsetX = 0.9f;
        private const float screenEdgeOffsetY = 0.1f;
        public Vector2 Pos => new Vector2(
            Custom.rainWorld.screenSize.x * screenEdgeOffsetX,
            Custom.rainWorld.screenSize.y * screenEdgeOffsetY
        );
        public int LifeSpan => 200;
        public int lifeTime, numberInQueue;
        public FLabel label;

        private string killText;
        private readonly Dictionary<AnimationType, AnimationTiming> animations;
        private readonly float[] animationProgress;

        private const float QueueOffset = 50f;

        public KillingNotify(Room _room, CreatureTemplate.Type victimType) : base()
        {
            if (Plugin.killingNotifyQueue.Count == 0)
                numberInQueue = 0;
            else
            {
                int maxV = -1;
                foreach (var note in Plugin.killingNotifyQueue)
                {
                    if (note.numberInQueue > maxV)
                        maxV = note.numberInQueue;
                }
                numberInQueue = ++maxV;
            }
            Plugin.killingNotifyQueue.Enqueue(this);
            creatureType = victimType;
            room = _room;
            pos = Pos + numberInQueue * new Vector2(0f, QueueOffset);

            animations = new Dictionary<AnimationType, AnimationTiming>()
            {
                [AnimationType.Icon] = new AnimationTiming(0, 20),
                [AnimationType.Line1] = new AnimationTiming(30, 30),
                [AnimationType.Line2] = new AnimationTiming(50, 30),
                [AnimationType.Text] = new AnimationTiming(90, 40),
                [AnimationType.SlideOut] = new AnimationTiming(180, 20)
            };
            animationProgress = new float[animations.Count];

            room.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom);
        }

        public override void Update(bool eu)
        {
            base.Update(eu);

            foreach (var anim in animations)
                animationProgress[(int)anim.Key] = anim.Value.GetProgress(lifeTime);

            if (++lifeTime > LifeSpan)
            {
                if (Plugin.killingNotifyQueue.Contains(this))
                    Plugin.killingNotifyQueue.Dequeue();
                Destroy();
            }
        }

        public override void Destroy()
        {
            label.RemoveFromContainer();
            base.Destroy();
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            IconSymbol.IconSymbolData iconSymbolData = new IconSymbol.IconSymbolData(creatureType, AbstractPhysicalObject.AbstractObjectType.Creature, 0);
            float linesWidth = 3f;

            sLeaser.sprites = new FSprite[]
            {
                new FSprite("Menu_Empty_Level_Thumb") { color = Color.black, anchorX = 0.2f },//background
                new FSprite(CreatureSymbol.SpriteNameOfCreature(iconSymbolData)) { color = CreatureSymbol.ColorOfCreature(iconSymbolData) },//Icon
                new FSprite("pixel") { scaleX = linesWidth, anchorX = 0, anchorY = 0 },//line 1
                new FSprite("pixel") { scaleX = linesWidth, anchorX = 1, anchorY = 0 },//line 2
            };

            string creatureName = Plugin.Translate(Plugin.ResolveCreatureName(creatureType.ToString()));
            killText = Plugin.Translate("$ was slain").Replace("$", creatureName);
            label = new FLabel(Custom.GetFont(), string.Empty)
            {
                color = Color.white,
                alignment = FLabelAlignment.Left
            };

            AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 slideOutDir = new Vector2(100f, 0f);
            Vector2 iconStartPos = pos - new Vector2(0f, 0.5f * QueueOffset);
            Vector2 iconPos = Vector2.Lerp(iconStartPos, pos, animationProgress[(int)AnimationType.Icon]) + animationProgress[(int)AnimationType.SlideOut] * slideOutDir;

            sLeaser.sprites[0].scaleX = 0.5f + 1.5f * animationProgress[(int)AnimationType.Icon];
            sLeaser.sprites[0].scaleY = 0.25f + 1f * animationProgress[(int)AnimationType.Icon];
            sLeaser.sprites[0].alpha = 0.5f * (animationProgress[(int)AnimationType.Icon] - animationProgress[(int)AnimationType.SlideOut]);
            sLeaser.sprites[0].SetPosition(iconPos);

            sLeaser.sprites[1].SetPosition(iconPos);
            sLeaser.sprites[1].alpha = animationProgress[(int)AnimationType.Icon];

            Vector2 scaleOfIcon = sLeaser.sprites[1].element.sourceSize;
            Vector2 line1Direction = new Vector2(1f, -1f) * scaleOfIcon, line2Direction = new Vector2(-1f, -1f) * scaleOfIcon;
            Vector2 line1Offset = -line1Direction / 2f, line2Offset = -line2Direction / 2f;
            float linesLength = scaleOfIcon.magnitude;
            sLeaser.sprites[2].rotation = Custom.VecToDeg(line1Direction);
            sLeaser.sprites[2].scaleY = Mathf.Lerp(0f, linesLength, animationProgress[(int)AnimationType.Line1]);
            sLeaser.sprites[2].SetPosition(pos + line1Offset + animationProgress[(int)AnimationType.SlideOut] * slideOutDir);

            sLeaser.sprites[3].rotation = Custom.VecToDeg(line2Direction);
            sLeaser.sprites[3].scaleY = Mathf.Lerp(0f, linesLength, animationProgress[(int)AnimationType.Line2]);
            sLeaser.sprites[3].SetPosition(pos + line2Offset + animationProgress[(int)AnimationType.SlideOut] * slideOutDir);

            if (animationProgress[(int)AnimationType.SlideOut] != 0f)
            {
                for (int i = 1; i < sLeaser.sprites.Length; i++)
                    sLeaser.sprites[i].alpha = 1f - animationProgress[(int)AnimationType.SlideOut];
            }

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
            label.RemoveFromContainer();
            newContatiner.AddChild(label);
        }

        private enum AnimationType
        {
            Icon,
            Line1,
            Line2,
            Text,
            SlideOut
        }

        private readonly struct AnimationTiming
        {
            public readonly int startFrame, duration;

            public AnimationTiming(int _startFrame, int _duration)
            {
                startFrame = _startFrame;
                duration = _duration;
            }

            public bool IsFinished(int t) => t > startFrame + duration;
            public bool IsActive(int t) => t >= startFrame && !IsFinished(t);
            public float GetProgress(int t) => IsActive(t) || IsFinished(t) ? Mathf.InverseLerp(startFrame, startFrame + duration, t) : 0f;
        }
    }
}
