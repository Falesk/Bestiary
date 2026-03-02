namespace Bestiary
{
    public static class HooksKillingNotify
    {
        public static void Init()
        {
            On.SocialEventRecognizer.Killing += SocialEventRecognizer_Killing;
        }

        private static void SocialEventRecognizer_Killing(On.SocialEventRecognizer.orig_Killing orig, SocialEventRecognizer self, Creature killer, Creature victim)
        {
            orig(self, killer, victim);
            if (killer is Player player && player.SessionRecord != null)
                player.room.AddObject(new KillingNotify(player.room, victim.Template.type));
        }
    }
}
