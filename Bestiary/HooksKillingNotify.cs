using System.Linq;
using UnityEngine;

namespace Bestiary
{
    public static class HooksKillingNotify
    {
        public static void Init()
        {
            On.SocialEventRecognizer.Killing += SocialEventRecognizer_Killing;
            On.RoomCamera.ChangeRoom += RoomCamera_ChangeRoom;
            On.Player.ProcessDebugInputs += Player_ProcessDebugInputs;
        }

        private static void Player_ProcessDebugInputs(On.Player.orig_ProcessDebugInputs orig, Player self)
        {
            orig(self);
            if (Input.GetKeyDown("p"))
            {
                self.room.AddObject(new KillingNotify(self.room, CreatureTemplate.Type.GreenLizard));
            }
        }

        private static void RoomCamera_ChangeRoom(On.RoomCamera.orig_ChangeRoom orig, RoomCamera self, Room newRoom, int cameraPosition)
        {
            Room prevRoom = self.room;
            orig(self, newRoom, cameraPosition);
            foreach (KillingNotify notify in Plugin.killingNotifyQueue)
            {
                prevRoom?.RemoveObject(notify);
                newRoom.AddObject(notify);
            }
        }

        private static void SocialEventRecognizer_Killing(On.SocialEventRecognizer.orig_Killing orig, SocialEventRecognizer self, Creature killer, Creature victim)
        {
            if (killer is Player player && player.SessionRecord != null && !player.SessionRecord.kills.Any(x => x.symbolData.critType == victim.Template.type))
                player.room.AddObject(new KillingNotify(player.room, victim.Template.type));
            orig(self, killer, victim);
        }
    }
}
