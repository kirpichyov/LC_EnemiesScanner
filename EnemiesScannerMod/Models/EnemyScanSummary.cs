using UnityEngine;

namespace EnemiesScannerMod.Models
{
    internal sealed class EnemyScanSummary
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public float Distance { get; set; }
        public char UpDownIndicator { get; set; }
        public RelativeLevel RelativeLevel { get; set; }
        public bool IsOutsideType { get; set; }
        public DangerLevel DangerLevel { get; set; }

        public static EnemyScanSummary CreateFromEnemy(EnemyAI enemy, Vector3 playerPosition)
        {
            var position = enemy.transform.position;
            var relativeLevel = GetRelativeLevel(position, playerPosition);
            var distance = Vector3.Distance(position, playerPosition);

            return new EnemyScanSummary()
            {
                Name = enemy.name.Replace("(Clone)", string.Empty),
                Position = position,
                Distance = distance,
                RelativeLevel = relativeLevel,
                UpDownIndicator = GetUpDownIndicator(relativeLevel),
                IsOutsideType = enemy.enemyType.isOutsideEnemy,
                DangerLevel = GetDangerLevel(distance),
            };
        }

        public static EnemyScanSummary CreateFromTurret(Turret enemy, Vector3 playerPosition)
        {
            var position = enemy.transform.position;
            var relativeLevel = GetRelativeLevel(position, playerPosition);
            var distance = Vector3.Distance(position, playerPosition);

            return new EnemyScanSummary
            {
                Name = enemy.name
                    .Replace("(Clone)", string.Empty)
                    .Replace("Script", string.Empty),
                Position = position,
                Distance = distance,
                RelativeLevel = relativeLevel,
                UpDownIndicator = GetUpDownIndicator(relativeLevel),
                IsOutsideType = false,
                DangerLevel = GetDangerLevel(distance),
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
        
        private static DangerLevel GetDangerLevel(float distance)
        {
            if (distance <= 8f)
            {
                return DangerLevel.Death;
            }
            
            if (distance > 8f && distance <= 12f)
            {
                return DangerLevel.Danger;
            }

            if (distance > 12f && distance <= 20f)
            {
               return DangerLevel.Near;
            }

            if (distance > 20f && distance <= 30f)
            {
                return DangerLevel.Far;
            }

            return DangerLevel.TooFar;
        }
    }
}