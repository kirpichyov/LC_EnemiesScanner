using UnityEngine;

namespace EnemiesScannerMod.Models
{
    internal sealed class EnemyScanSummary
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public float Distance { get; set; }
        public string Location { get; set; }
        public char UpDownIndicator { get; set; }
        public RelativeLevel RelativeLevel { get; set; }

        public static EnemyScanSummary CreateFromEnemy(EnemyAI enemy, Vector3 playerPosition)
        {
            var position = enemy.serverPosition;
            var relativeLevel = GetRelativeLevel(position, playerPosition);

            return new EnemyScanSummary()
            {
                Name = enemy.name.Replace("(Clone)", string.Empty),
                Position = position,
                Distance = Vector3.Distance(position, playerPosition),
                Location = enemy.isOutside ? "OUT" : "IN",
                RelativeLevel = relativeLevel,
                UpDownIndicator = GetUpDownIndicator(relativeLevel)
            };
        }

        public static EnemyScanSummary CreateFromTurret(Turret enemy, Vector3 playerPosition)
        {
            var position = enemy.transform.position;
            var relativeLevel = GetRelativeLevel(position, playerPosition);

            return new EnemyScanSummary
            {
                Name = enemy.name
                    .Replace("(Clone)", string.Empty)
                    .Replace("Script", string.Empty),
                Position = position,
                Distance = Vector3.Distance(position, playerPosition),
                Location = "N/A",
                RelativeLevel = relativeLevel,
                UpDownIndicator = GetUpDownIndicator(relativeLevel),
            };
        }

        private static char GetUpDownIndicator(RelativeLevel level)
        {
            switch (level)
            {
                case RelativeLevel.Lower:
                    return 'v';
                case RelativeLevel.Upper:
                    return '^';
                default:
                    return '-';
            }
        }

        private static RelativeLevel GetRelativeLevel(Vector3 a, Vector3 b)
        {
            const float threshold = 5f;
            var diffY = a.y - b.y;

            if (diffY < 0 && diffY <= -threshold)
            {
                return RelativeLevel.Lower;
            }

            if (diffY > 0 && diffY >= threshold)
            {
                return RelativeLevel.Upper;
            }

            return RelativeLevel.Same;
        }
    }
}