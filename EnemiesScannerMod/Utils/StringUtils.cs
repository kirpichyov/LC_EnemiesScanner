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
    }
}