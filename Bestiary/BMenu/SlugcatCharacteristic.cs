using System.Collections.Generic;
using UnityEngine;

namespace Bestiary.BMenu
{
    public class SlugcatCharacteristic : ICharacteristic
    {
        public SlugcatStats.Name slugcat;
        public float spearDamageMin, spearDamageMax, spearStun;
        public float maulDamage, maulStun;
        public int minFood, maxFood;
        public Diet diet;
        public float agility, mass, lungCapacity1, lungCapacity;
        public int cycles, deathsTotal;

        public SlugcatCharacteristic(SlugcatStats.Name name, int deaths, int cycles)
        {
            slugcat = name;
            SlugcatStats stats = new SlugcatStats(name, false);
            spearStun = 0.5f;
            GetSpearDamage(stats.throwingSkill);
            diet = GetDiet(name);
            minFood = stats.foodToHibernate;
            maxFood = stats.maxFood;

            if (SlugcatStats.SlugcatCanMaul(name))
            {
                maulDamage = 1f;
                maulStun = 0.375f;
            }

            agility = (stats.runspeedFac + stats.corridorClimbSpeedFac + stats.poleClimbSpeedFac) / 3f;
            mass = stats.bodyWeightFac;

            GetLungCapacity(stats.lungsFac);

            if (name == SlugcatStats.Name.Red)
                this.cycles = MoreSlugcats.MMF.cfgHunterCycles.Value - cycles - 1;
            else this.cycles = cycles;
            deathsTotal = deaths;
        }

        public string[] GenerateLines()
        {
            List<string> lines = new List<string>();

            string spearDmg = Plugin.Translate("Spear damage:") + " ";
            if (spearDamageMax == spearDamageMin)
                spearDmg += spearDamageMax.ToString();
            else if (slugcat == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Gourmand)
                spearDmg += $"{spearDamageMin} " + Plugin.Translate("(exhausted)") + $"; {spearDamageMax}";
            else spearDmg += $"{spearDamageMin}-{spearDamageMax}";
            spearDmg += " " + Plugin.Translate("(Stun $ s)").Replace("$", $"{spearStun}");
            lines.Add(spearDmg);

            if (maulDamage > 0)
                lines.Add(Plugin.Translate("Maul damage: $").Replace("$", $"{maulDamage} ") + Plugin.Translate("(Stun $ s)").Replace("$", $"{maulStun}"));
            lines.Add(Plugin.Translate("Food pips:"));
            lines.Add(string.Empty);
            lines.Add(string.Empty);

            lines.Add(Plugin.Translate("Diet:") + $" {Plugin.Translate(GetDietName(diet))}");

            lines.Add(Plugin.Translate("Agility: $").Replace("$", $"{agility:F2}"));
            lines.Add(Plugin.Translate("Mass: $").Replace("$", $"{mass:F2}"));
            lines.Add(Plugin.Translate("Lung capacity: ~$ s").Replace("$", $"{lungCapacity:F2}"));
            lines.Add(Plugin.Translate("Cycles lived: $").Replace("$", $"{cycles}"));
            lines.Add(Plugin.Translate("Deaths total: $").Replace("$", $"{deathsTotal}"));

            return lines.ToArray();
        }

        private static string GetDietName(Diet d)
        {
            switch (d)
            {
                case Diet.herbivorous: return "Herbivorous";
                case Diet.carnivorous: return "Carnivorous";
                default: return "Omnivorous";
            }
        }

        private void GetLungCapacity(float l)
        {
            int generalLungs = Mathf.FloorToInt((2 * 40 * 9 * 1.28f) / (3f * l));
            int exhaustedLungs = Mathf.FloorToInt((40 * 4.5f * 1.28f) / (3f * l));
            int drownedLungs = Mathf.FloorToInt((40 * 4.5f * 1.5f * 1.28f) / (3f * l));

            float lungCapacity1 = (generalLungs + exhaustedLungs) * 0.025f;
            float lungCapacity2 = (generalLungs + drownedLungs) * 0.025f;
            lungCapacity = (lungCapacity1 + lungCapacity2) * 0.5f;
        }

        private static Diet GetDiet(SlugcatStats.Name name)
        {
            int meatNourishment = SlugcatStats.NourishmentOfObjectEaten(name, new JellyFish(
                new AbstractConsumable(null, AbstractPhysicalObject.AbstractObjectType.JellyFish, null, new WorldCoordinate(), new EntityID(), 0, 0, null)));
            int fruitNourishment = SlugcatStats.NourishmentOfObjectEaten(name, new DangleFruit(
                new DangleFruit.AbstractDangleFruit(null, null, new WorldCoordinate(), new EntityID(), 0, 0, false, null)));
            if (meatNourishment == -1)
                return Diet.herbivorous;
            else if (fruitNourishment < 4)
                return Diet.carnivorous;
            return Diet.omnivorous;
        }

        private void GetSpearDamage(int throwingSkill)
        {
            if (throwingSkill == 0)
            {
                spearDamageMin = 0.6f;
                spearDamageMax = 0.9f;
            }
            else if (throwingSkill == 2)
            {
                if (slugcat == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Gourmand)
                {
                    spearDamageMin = 0.3f;
                    spearDamageMax = 3f;
                }
                else spearDamageMin = spearDamageMax = 1.25f;
            }
            else spearDamageMin = spearDamageMax = 1f;
        }

        public enum Diet
        {
            herbivorous,
            omnivorous,
            carnivorous
        }
    }
}
