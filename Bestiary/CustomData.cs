using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bestiary
{
    public static class CustomData
    {
        public class RoomData
        {
            public List<KillingNotify> notifies;
        }

        private static readonly ConditionalWeakTable<Room, RoomData> roomData = new ConditionalWeakTable<Room, RoomData>();
        public static RoomData GetData(this Room room) => roomData.GetValue(room, x => new RoomData());
    }
}
