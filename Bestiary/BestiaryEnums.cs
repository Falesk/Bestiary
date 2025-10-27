namespace Bestiary
{
    public static class BestiaryEnums
    {
        public static ProcessManager.ProcessID Bestiary;

        public static void RegisterValues()
        {
            Bestiary = new ProcessManager.ProcessID("Bestiary", true);
        }

        public static void UnregisterValues()
        {
            ProcessManager.ProcessID bestiary = Bestiary;
            bestiary?.Unregister();
            Bestiary = null;
        }
    }
}
