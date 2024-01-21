namespace EnemiesScannerMod.Utils
{
    internal static class StringUtils
    {
        public static string GetCloseIndicator(float distance)
        {
            if (distance <= 5f)
            {
                return "<color=#ff0000>GG</color>";
            }
            
            if (distance > 5f && distance <= 10f)
            {
                return "<color=#ff6f00>DANGER</color>";
            }

            if (distance > 10f && distance <= 20f)
            {
                return "<color=#fff200>NEAR</color>";
            }

            if (distance > 20f && distance <= 30f)
            {
                return "<color=#ffffff>FAR</color>";
            }

            return "TOO FAR";
        }
        
        public static string GetActiveOrDisableString(bool value)
        {
            return value ? "activated" : "disabled";
        }
    }
}