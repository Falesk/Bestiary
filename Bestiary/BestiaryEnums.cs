namespace Bestiary
{
    public static class BestiaryEnums
    {
        public static ProcessManager.ProcessID Bestiary;
        public static ProcessManager.ProcessID BestiarySleepMenu;

        public static void RegisterValues()
        {
            Bestiary = new ProcessManager.ProcessID("Bestiary", true);
            BestiarySleepMenu = new ProcessManager.ProcessID("BestiarySleepMenu", true);
        }

        public static void UnregisterValues()
        {
            ProcessManager.ProcessID bestiary = Bestiary;
            bestiary?.Unregister();
            Bestiary = null;

            ProcessManager.ProcessID bestiary2 = BestiarySleepMenu;
            bestiary2?.Unregister();
            BestiarySleepMenu = null;
        }
    }
}
