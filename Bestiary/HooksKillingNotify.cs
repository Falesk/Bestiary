using System.Linq;
using UnityEngine;

namespace Bestiary
{
    public static class HooksKillingNotify
    {
        public static void Init()
        {
            On.Room.ctor += Room_ctor;
            On.Room.CleanOutObjectNotInThisRoom += Room_CleanOutObjectNotInThisRoom;
            On.Room.AddObject += Room_AddObject;
            On.SocialEventRecognizer.Killing += SocialEventRecognizer_Killing;
            On.RoomCamera.ChangeRoom += RoomCamera_ChangeRoom;
            On.Player.ProcessDebugInputs += Player_ProcessDebugInputs;
        }

        private static void Player_ProcessDebugInputs(On.Player.orig_ProcessDebugInputs orig, Player self)
        {
            orig(self);
            if (Input.GetKeyDown("p"))
            {
                int rand = (int)Mathf.Lerp(0, StaticWorld.creatureTemplates.Length, Random.value);
                self.room.AddObject(new KillingNotify(self.room, StaticWorld.creatureTemplates[rand].type));
            }
        }

        private static void Room_ctor(On.Room.orig_ctor orig, Room self, RainWorldGame game, World world, AbstractRoom abstractRoom, bool devUI)
        {
            orig(self, game, world, abstractRoom, devUI);
            if (self.GetData() is CustomData.RoomData roomData)
                roomData.notifies = new System.Collections.Generic.List<KillingNotify>();
        }

        private static void Room_CleanOutObjectNotInThisRoom(On.Room.orig_CleanOutObjectNotInThisRoom orig, Room self, UpdatableAndDeletable obj)
        {
            if (obj is KillingNotify notify && self.GetData() is CustomData.RoomData roomData)
                roomData.notifies.Remove(notify);
            orig(self, obj);
        }

        private static void Room_AddObject(On.Room.orig_AddObject orig, Room self, UpdatableAndDeletable obj)
        {
            orig(self, obj);
            if (obj is KillingNotify notify && self.GetData() is CustomData.RoomData roomData)
            {
                for (int i = 0; i < roomData.notifies.Count; i++)
                    roomData.notifies[i].AscendNotify();
                roomData.notifies.Add(notify);
            }
        }

        private static void RoomCamera_ChangeRoom(On.RoomCamera.orig_ChangeRoom orig, RoomCamera self, Room newRoom, int cameraPosition)
        {
            Room prevRoom = self.room;
            orig(self, newRoom, cameraPosition);
            if (prevRoom != null && prevRoom.GetData() is CustomData.RoomData roomData)
            {
                foreach (KillingNotify notify in roomData.notifies)
                {
                    prevRoom?.RemoveObject(notify);
                    notify.killLabel?.RemoveFromContainer();
                    notify.killLabelShadow?.RemoveFromContainer();
                    newRoom.AddObject(notify);
                }
            }
        }

        private static void SocialEventRecognizer_Killing(On.SocialEventRecognizer.orig_Killing orig, SocialEventRecognizer self, Creature killer, Creature victim)
        {
            if (killer is Player player && player.SessionRecord != null && player.room.GetData() is CustomData.RoomData roomData &&
                !RWCustom.Custom.rainWorld.progression.currentSaveState.kills.Any(x => x.Key.critType == victim.Template.type))
            {
                player.room.AddObject(new KillingNotify(player.room, victim.Template.type));
            }
            orig(self, killer, victim);
        }
    }
}
