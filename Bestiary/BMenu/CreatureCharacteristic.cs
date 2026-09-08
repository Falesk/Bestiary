using System.Collections.Generic;

namespace Bestiary.BMenu
{
    public class CreatureCharacteristic : ICharacteristic
    {
        public float hp, damage, biteChance;
        public int foodPoints, score, kills;
        public CreatureTemplate.Relationship.Type behaviour;
        public bool IsLizard => biteChance != default;

        private bool hasTemplate;

        private readonly string healthOverride;
        private readonly string pointsPerKillOverride;
        private readonly string totalPointsOverride;

        public CreatureCharacteristic(SaveInfo.Info.KilledInfo killedInfo)
        {
            kills = killedInfo?.kills ?? 0;
            score = -1;

            if (killedInfo == null)
                return;

            healthOverride = killedInfo.healthOverride;
            pointsPerKillOverride = killedInfo.pointsPerKillOverride;
            totalPointsOverride = killedInfo.totalPointsOverride;

            CreatureTemplate cTemplate =
                StaticWorld.GetCreatureTemplate(killedInfo.iconData.critType);

            if (cTemplate == null)
                return;

            hasTemplate = true;

            hp = cTemplate.baseDamageResistance;
            foodPoints = cTemplate.meatPoints;
            score = BestiaryMenu.GetKillScore(killedInfo.iconData);

            if (cTemplate.relationships != null &&
                CreatureTemplate.Type.Slugcat.Index >= 0 &&
                CreatureTemplate.Type.Slugcat.Index < cTemplate.relationships.Length)
            {
                behaviour =
                    cTemplate.relationships[
                        CreatureTemplate.Type.Slugcat.Index
                    ].type;
            }

            if (cTemplate.breedParameters is LizardBreedParams breedParams)
            {
                damage = breedParams.biteDamage;
                biteChance = breedParams.biteDamageChance;
            }
        }

        public string[] GenerateLines()
        {
            List<string> lines = new List<string>();

            if (!hasTemplate)
            {
                lines.Add(
                    Plugin.Translate("Kill count: %")
                        .Replace("%", kills.ToString())
                );

                lines.Add(
                    Plugin.Translate("No CreatureTemplate data available for this entry.")
                );

                return lines.ToArray();
            }

            if (damage != default)
            {
                lines.Add(
                    Plugin.Translate("Damage: %")
                        .Replace("%", damage.ToString())
                );
            }

            if (IsLizard)
            {
                lines.Add(
                    Plugin.Translate("Deadly Bite Chance: %")
                        .Replace("%", $"{biteChance * 100f:F1}%")
                );
            }

            lines.Add(
                Plugin.Translate("Kill count: %")
                    .Replace("%", kills.ToString())
            );

            if (foodPoints != 0)
            {
                lines.Add(
                    Plugin.Translate("Restores % food pips")
                        .Replace("%", foodPoints.ToString())
                );
            }

            lines.Add(
                Plugin.Translate("Health: %")
                    .Replace(
                        "%",
                        !string.IsNullOrEmpty(healthOverride)
                            ? Plugin.Translate(healthOverride)
                            : hp.ToString()
                    )
            );

            if (behaviour != null)
            {
                lines.Add(
                    Plugin.Translate("Behaviour") +
                    ": " +
                    Plugin.Translate($"behav-{behaviour.value}")
                );
            }

            lines.Add(
                Plugin.Translate("Points per kill: %")
                    .Replace(
                        "%",
                        !string.IsNullOrEmpty(pointsPerKillOverride)
                            ? Plugin.Translate(pointsPerKillOverride)
                            : (score == -1 ? "?" : score.ToString())
                    )
            );

            lines.Add(
                Plugin.Translate("Total points: %")
                    .Replace(
                        "%",
                        !string.IsNullOrEmpty(totalPointsOverride)
                            ? totalPointsOverride
                            : (
                                score == -1
                                    ? "?"
                                    : (score * kills).ToString()
                            )
                    )
            );

            return lines.ToArray();
        }
    }
}