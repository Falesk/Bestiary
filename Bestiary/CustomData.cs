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

        public class MiscProgressionCustomData
        {
            public List<IconSymbol.IconSymbolData> savedObjects;
        }

        private static readonly ConditionalWeakTable<PlayerProgression.MiscProgressionData, MiscProgressionCustomData> miscProgrCData = new ConditionalWeakTable<PlayerProgression.MiscProgressionData, MiscProgressionCustomData>();
        public static MiscProgressionCustomData GetData(this PlayerProgression.MiscProgressionData mProgression) => miscProgrCData.GetValue(mProgression, x => new MiscProgressionCustomData());
    }
}
