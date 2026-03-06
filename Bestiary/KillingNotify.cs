using RWCustom;
using System.Collections.Generic;
using UnityEngine;

namespace Bestiary
{
    public class KillingNotify : CosmeticSprite
    {
        public CreatureTemplate.Type creatureType;
        private const float screenEdgeOffsetX = 0.83f;
        private const float screenEdgeOffsetY = 0.15f;
        public Vector2 Pos => new Vector2(
            Custom.rainWorld.screenSize.x * screenEdgeOffsetX,
            Custom.rainWorld.screenSize.y * screenEdgeOffsetY
        );
        public int LifeSpan => 200;
        public int lifeTime, numberInQueue;
        public FLabel killLabel, killLabelShadow;

        private string killText;
        private readonly Dictionary<AnimationType, AnimationTiming> animations;
        private readonly float[] animationProgress;

        private const float QueueOffset = 50f;

        private int BackSprite => 0;
        private int IconSprite => 2;

        public KillingNotify(Room room, CreatureTemplate.Type victimType) : base()
        {
            int maxV = -1;
            foreach (var note in Plugin.killingNotifyQueue)
            {
                if (note.numberInQueue > maxV)
                    maxV = note.numberInQueue;
            }
            numberInQueue = maxV + 1;
            Plugin.killingNotifyQueue.Enqueue(this);
            creatureType = victimType;
            this.room = room;
            pos = Pos + numberInQueue * new Vector2(0f, QueueOffset);

            animations = new Dictionary<AnimationType, AnimationTiming>()
            {
                [AnimationType.Icon] = new AnimationTiming(0, 20, AnimationTiming.ProgressionType.Quadratic),
                [AnimationType.Line1] = new AnimationTiming(30, 25, AnimationTiming.ProgressionType.Cubic),
                [AnimationType.Line2] = new AnimationTiming(45, 25, AnimationTiming.ProgressionType.Cubic),
                [AnimationType.Text] = new AnimationTiming(85, 40, AnimationTiming.ProgressionType.Linear),
                [AnimationType.SlideOut] = new AnimationTiming(180, 20, AnimationTiming.ProgressionType.Quadratic)
            };
            animationProgress = new float[animations.Count];
        }

        public override void Update(bool eu)
        {
            base.Update(eu);

            foreach (var anim in animations)
                animationProgress[(int)anim.Key] = anim.Value.GetProgress(lifeTime);

            if (animationProgress[(int)AnimationType.Text] > 0f)
            {
                int val = (int)Mathf.Lerp(0, killText.Length, animationProgress[(int)AnimationType.Text]);
                killLabel.text = killText.Substring(0, val);
                killLabelShadow.text = killLabel.text;
            }

            if (lifeTime == 5)
                room.PlaySound(SoundID.HUD_Food_Meter_Deplete_Plop_A, 0, 1.2f, Mathf.Lerp(0.9f, 1.1f, Random.value));

            if (++lifeTime > LifeSpan)
            {
                if (Plugin.killingNotifyQueue.Contains(this))
                    Plugin.killingNotifyQueue.Dequeue();
                Destroy();
            }
        }

        public override void Destroy()
        {
            killLabel?.RemoveFromContainer();
            killLabelShadow?.RemoveFromContainer();
            base.Destroy();
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            IconSymbol.IconSymbolData iconSymbolData = new IconSymbol.IconSymbolData(creatureType, AbstractPhysicalObject.AbstractObjectType.Creature, 0);
            float linesWidth = 3f;

            sLeaser.sprites = new FSprite[]
            {
                new FSprite("Menu_Empty_Level_Thumb") { color = Color.black, anchorX = 0.1f },//background
                new FSprite(CreatureSymbol.SpriteNameOfCreature(iconSymbolData)) { color = Color.black },//IconShadow
                new FSprite(CreatureSymbol.SpriteNameOfCreature(iconSymbolData)) { color = CreatureSymbol.ColorOfCreature(iconSymbolData) },//Icon
                new FSprite("pixel") { scaleX = linesWidth, anchorX = 0, anchorY = 0, color = Color.black },//line 1 Shadow
                new FSprite("pixel") { scaleX = linesWidth, anchorX = 1, anchorY = 0, color = Color.black },//line 2 Shadow
                new FSprite("pixel") { scaleX = linesWidth, anchorX = 0, anchorY = 0, color = Color.white * 0.95f },//line 1
                new FSprite("pixel") { scaleX = linesWidth, anchorX = 1, anchorY = 0, color = Color.white * 0.95f },//line 2
            };

            string creatureName = Plugin.Translate(Plugin.ResolveCreatureName(creatureType.ToString()));
            killText = Plugin.Translate("$ was slain").Replace("$", creatureName);
            killLabel = new FLabel(Custom.GetFont(), string.Empty)
            {
                color = Color.white,
                alignment = FLabelAlignment.Left
            };
            killLabelShadow = new FLabel(Custom.GetFont(), string.Empty)
            {
                color = Color.black,
                alignment = FLabelAlignment.Left
            };

            AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            float distToRightEdge = (1 - screenEdgeOffsetX) * Custom.rainWorld.screenSize.x;
            Vector2 shadowOffset = new Vector2(1f, -1f);
            Vector2 scaleOfBackground = sLeaser.sprites[0].element.sourceSize;
            Vector2 slideOutDir = Vector2.right * distToRightEdge;
            Vector2 slideOutProgression = animationProgress[(int)AnimationType.SlideOut] * slideOutDir;
            Vector2 iconStartPos = pos - new Vector2(0f, 0.5f * QueueOffset);
            Vector2 iconPos = Vector2.Lerp(iconStartPos, pos, animationProgress[(int)AnimationType.Icon]) + slideOutProgression;

            sLeaser.sprites[BackSprite].scaleX = (1 / scaleOfBackground.x) * distToRightEdge * 1.2f;
            sLeaser.sprites[BackSprite].scaleY = (1 / scaleOfBackground.y) * QueueOffset;
            sLeaser.sprites[BackSprite].alpha = 0.2f * (animationProgress[(int)AnimationType.Icon] - animationProgress[(int)AnimationType.SlideOut]);
            sLeaser.sprites[BackSprite].SetPosition(iconPos);

            sLeaser.sprites[IconSprite].SetPosition(iconPos);
            sLeaser.sprites[IconSprite].alpha = animationProgress[(int)AnimationType.Icon];
            sLeaser.sprites[ShadowSprite(IconSprite)].SetPosition(iconPos + shadowOffset);
            sLeaser.sprites[ShadowSprite(IconSprite)].alpha = animationProgress[(int)AnimationType.Icon];

            Vector2 scaleOfIcon = sLeaser.sprites[1].element.sourceSize;
            Vector2 line1Direction = new Vector2(1f, -1f) * scaleOfIcon, line2Direction = new Vector2(-1f, -1f) * scaleOfIcon;
            Vector2 line1Offset = -line1Direction / 2f, line2Offset = -line2Direction / 2f;
            float linesLength = scaleOfIcon.magnitude;
            sLeaser.sprites[LineSprite(0)].rotation = Custom.VecToDeg(line1Direction);
            sLeaser.sprites[LineSprite(0)].scaleY = Mathf.Lerp(0f, linesLength, animationProgress[(int)AnimationType.Line1]);
            sLeaser.sprites[LineSprite(0)].SetPosition(pos + line1Offset + slideOutProgression);

            sLeaser.sprites[LineSprite(1)].rotation = Custom.VecToDeg(line2Direction);
            sLeaser.sprites[LineSprite(1)].scaleY = Mathf.Lerp(0f, linesLength, animationProgress[(int)AnimationType.Line2]);
            sLeaser.sprites[LineSprite(1)].SetPosition(pos + line2Offset + slideOutProgression);

            sLeaser.sprites[ShadowSprite(LineSprite(0))].rotation = Custom.VecToDeg(line1Direction);
            sLeaser.sprites[ShadowSprite(LineSprite(0))].scaleY = Mathf.Lerp(0f, linesLength, animationProgress[(int)AnimationType.Line1]);
            sLeaser.sprites[ShadowSprite(LineSprite(0))].SetPosition(pos + line1Offset + slideOutProgression + shadowOffset);

            sLeaser.sprites[ShadowSprite(LineSprite(1))].rotation = Custom.VecToDeg(line2Direction);
            sLeaser.sprites[ShadowSprite(LineSprite(1))].scaleY = Mathf.Lerp(0f, linesLength, animationProgress[(int)AnimationType.Line2]);
            sLeaser.sprites[ShadowSprite(LineSprite(1))].SetPosition(pos + line2Offset + slideOutProgression + shadowOffset);

            Vector2 textPosOffset = Vector2.right * 30;
            killLabel.SetPosition(pos + textPosOffset + slideOutProgression);
            killLabelShadow.SetPosition(pos + textPosOffset + slideOutProgression + shadowOffset);

            if (animationProgress[(int)AnimationType.SlideOut] != 0f)
            {
                for (int i = 1; i < sLeaser.sprites.Length; i++)
                    sLeaser.sprites[i].alpha = 1f - animationProgress[(int)AnimationType.SlideOut];
                killLabel.alpha = 1f - animationProgress[(int)AnimationType.SlideOut];
                killLabelShadow.alpha = 1f - animationProgress[(int)AnimationType.SlideOut];
            }

            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }

        private int LineSprite(int lineNum) => 5 + lineNum;
        private int ShadowSprite(int spriteNum) => (spriteNum > 4) ? (spriteNum - 2) : (spriteNum - 1);

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            newContatiner = newContatiner ?? rCam.ReturnFContainer("HUD");
            foreach (FSprite sprite in sLeaser.sprites)
            {
                sprite.RemoveFromContainer();
                newContatiner.AddChild(sprite);
            }
            killLabelShadow.RemoveFromContainer();
            newContatiner.AddChild(killLabelShadow);
            killLabel.RemoveFromContainer();
            newContatiner.AddChild(killLabel);
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
            public readonly ProgressionType progressionType;

            public AnimationTiming(int startFrame, int duration, ProgressionType progressionType = ProgressionType.Linear)
            {
                this.startFrame = startFrame;
                this.duration = duration;
                this.progressionType = progressionType;
            }

            public float GetProgress(int t)
            {
                if (t < startFrame) return 0f;
                if (t >= startFrame + duration) return 1f;

                float nt = Mathf.InverseLerp(startFrame, startFrame + duration, t);

                if (progressionType == ProgressionType.Quadratic)
                    return 1 - (1 - nt) * (1 - nt);
                if (progressionType == ProgressionType.Cubic)
                    return 1 - (1 - nt) * (1 - nt) * (1 - nt);
                return nt;
            }

            public enum ProgressionType
            {
                Linear,
                Quadratic,
                Cubic
            }
        }
    }
}
