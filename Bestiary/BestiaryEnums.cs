namespace Bestiary
{
    public static class BestiaryEnums
    {
        public static ProcessManager.ProcessID Bestiary;
        public static Menu.Slider.SliderID DescriptionScroll;

        public static void RegisterValues()
        {
            Bestiary = new ProcessManager.ProcessID("Bestiary", true);
            DescriptionScroll = new Menu.Slider.SliderID("BestiaryDescriptionScroll", true);
        }

        public static void UnregisterValues()
        {
            Bestiary?.Unregister();
            Bestiary = null;

            DescriptionScroll?.Unregister();
            DescriptionScroll = null;
        }
    }
}
