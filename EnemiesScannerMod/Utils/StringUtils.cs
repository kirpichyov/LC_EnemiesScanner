using EnemiesScannerMod.Models;

namespace EnemiesScannerMod.Utils
{
    internal static class StringUtils
    {
        public static string GetCloseIndicator(DangerLevel dangerLevel)
        {
            return dangerLevel switch
            {
                DangerLevel.Death => "<color=#ff0000>LETHAL</color>",
                DangerLevel.Danger => "<color=#ff6f00>DANGER</color>",
                DangerLevel.Near => "<color=#fff200>NEAR</color>",
                DangerLevel.Far => "<color=#ffffff>FAR</color>",
                _ => "TOO FAR"
            };
        }
        
        public static string GetActiveOrDisableString(bool value)
        {
            return value ? "activated" : "disabled";
        }
        
        public static string SanitizeEnemyDisplayName(string original)
        {
            return original
                .Replace("(Clone)", string.Empty)
                .Replace("Script", string.Empty)
                .Replace("Enemy", string.Empty)
                .Replace("AI", string.Empty)
                .Replace("Prefab", string.Empty);
        }
        
        public static string SanitizeEnemyTypeName(string original)
        {
            return original
                .Replace("Script", string.Empty)
                .Replace("Enemy", string.Empty)
                .Replace("AI", string.Empty);
        }
    }
}