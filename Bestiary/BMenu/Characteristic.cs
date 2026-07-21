using System.Collections.Generic;

namespace Bestiary.BMenu
{
    public struct Characteristic
    {
        public float hp, damage, biteChance;
        public int foodPoints, score, kills;
        public CreatureTemplate.Relationship.Type behaviour;
        public bool IsLizard => biteChance != default;

        public string[] GenerateLines()
        {
            List<string> lines = new List<string>();

            if (damage != default)
                lines.Add(Plugin.Translate("Damage: %").Replace("%", damage.ToString()));
            if (IsLizard)
                lines.Add(Plugin.Translate("Deadly Bite Chance: %").Replace("%", $"{biteChance * 100f:F1}%"));
            lines.Add(Plugin.Translate("Kill count: %").Replace("%", kills.ToString()));
            if (foodPoints != 0)
                lines.Add(Plugin.Translate("Restores % food pips to carnivorous slugcats").Replace("%", foodPoints.ToString()));
            else lines.Add(Plugin.Translate("Doesn't restore food pips"));
            lines.Add(Plugin.Translate("Health: %").Replace("%", hp.ToString()));
            lines.Add(Plugin.Translate("Behaviour") + ": " + Plugin.Translate($"behav-{behaviour.value}"));
            lines.Add(Plugin.Translate("Points per kill: %").Replace("%", score == -1 ? "?" : score.ToString()));
            lines.Add(Plugin.Translate("Total points: %").Replace("%", score == -1 ? "?" : (score * kills).ToString()));
            return lines.ToArray();
        }
    }
}
