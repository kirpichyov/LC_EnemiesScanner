using System;
using BepInEx.Configuration;

namespace EnemiesScannerMod
{
    internal static class ModConfig
    {
        private const string GeneralSectionName = "General";
        private const string ExperimentalSectionName = "Experimental";
        
        private const string EnablePingSound_Description =
            "Determines if ping sound should be enabled. Ping sound is played on each scan.";
        
        private const string ShowTopEnemiesCount_Description =
            "Determines how many the nearest enemies should be shown on the scanner. " +
            "Min value is 1 and max value is 8.";
        
        private const string DisableOutsideFilter_Description =
            "Determines if the scanner should only display the enemies inside your location on the map. " +
            "If filter enabled then if you in factory, only enemies inside factory displayed and outside enemies are ignored and vice versa. " +
            "Otherwise, both enemies are always shown. " +
            "This feature maybe needed for moons from the mods where enemies that shouldn't be outside are actually spawned outside.";

        private const string ShopPrice_Description =
            "Determines how much scanner will cost. Min value is 1 and max value is 1000.";

        private const string EnableOverheat_Description =
            "Determines if overheat should be enabled, so scanning will be temporary disabled on overheat. Heat is decreased when the scanner is turned off.";
        
        private const string OverheatTime_Description =
            "Determines how much time in seconds scanner could continuously work without overheat. " +
            "Scanner overheat option should be enabled first. Min value is 5 and max value is 1800.";
        
        private const string OverheatCooldownTime_Description =
            "Determines how much time in seconds is needed to cool down on overheat. " +
            "Scanner overheat option should be enabled first. Min value is 5 and max value is 1800.";
        
        private const string EnableExactDistance_Description =
            "Determines if the exact distance to the enemy should be shown in the scanner.";
        
        private const string EnableScanRadiusLimit_Description =
            "Determines if the scan radius should be limited to a specific value.";
        
        private const string ScanRadiusLimit_Description =
            "Determines the max radius (in meters) for a scanner. " +
            "Radius limit option should be enabled first. Min value is 5 and max value is 2000.";

        private const string BatteryCapacity_Description =
            "Determines the battery usage (in seconds). Min value is 5 and max value is 1000.";

        private const string ScannerBlackList_Description =
            "Determines the list of enemies to exclude from scanning. Names should be separated by a semi-colon symbol. List is case-insensitive. You can find the list of supported names in readme.";
        
        private const string ScannerBlackList_DefaultValue = "DocileLocustBees;Doublewing;Turret;";
        
        public static void Init()
        {
            PluginLoader.Instance.BindConfig(ref EnablePingSound, GeneralSectionName, "Enable scanner ping sound", true, EnablePingSound_Description);
            PluginLoader.Instance.BindConfig(ref ShowTopEnemiesCount, GeneralSectionName, "Count of the nearest enemies shown", 5, ShowTopEnemiesCount_Description);
            PluginLoader.Instance.BindConfig(ref DisableOutsideFilter, ExperimentalSectionName, "Disable filtering by outside enemies", false, DisableOutsideFilter_Description);
            PluginLoader.Instance.BindConfig(ref ShopPrice, GeneralSectionName, "Scanner shop price", 20, ShopPrice_Description);
            PluginLoader.Instance.BindConfig(ref EnableOverheat, GeneralSectionName, "Enable scanner overheat", false, EnableOverheat_Description);
            PluginLoader.Instance.BindConfig(ref OverheatTime, GeneralSectionName, "Scanner overheat timer (seconds)", 120, OverheatTime_Description);
            PluginLoader.Instance.BindConfig(ref OverheatCooldownTime, GeneralSectionName, "Scanner cooldown timer (seconds)", 60, OverheatCooldownTime_Description);
            PluginLoader.Instance.BindConfig(ref EnableExactDistance, GeneralSectionName, "Show the exact distance to the enemy", true, EnableExactDistance_Description);
            PluginLoader.Instance.BindConfig(ref EnableScanRadiusLimit, GeneralSectionName, "Limit the scan radius to a specific value", false, EnableScanRadiusLimit_Description);
            PluginLoader.Instance.BindConfig(ref ScanRadiusLimit, GeneralSectionName, "Scan radius limit (meters)", 50f, ScanRadiusLimit_Description);
            PluginLoader.Instance.BindConfig(ref BatteryCapacity, GeneralSectionName, "Battery capacity", 600f /*10min*/, BatteryCapacity_Description);
            PluginLoader.Instance.BindConfig(ref ScannerBlackList, GeneralSectionName, "Enemies scan black list", ScannerBlackList_DefaultValue, ScannerBlackList_Description);
        }
        
        public static ConfigEntry<bool> EnablePingSound;
        public static ConfigEntry<int> ShowTopEnemiesCount;
        public static ConfigEntry<bool> DisableOutsideFilter;
        public static ConfigEntry<int> ShopPrice;
        public static ConfigEntry<bool> EnableOverheat;
        public static ConfigEntry<int> OverheatTime;
        public static ConfigEntry<int> OverheatCooldownTime;
        public static ConfigEntry<bool> EnableExactDistance;
        public static ConfigEntry<bool> EnableScanRadiusLimit;
        public static ConfigEntry<float> ScanRadiusLimit;
        public static ConfigEntry<float> BatteryCapacity;
        public static ConfigEntry<string> ScannerBlackList;

        public static int ShowTopEnemiesCountNormalized => Math.Clamp(ShowTopEnemiesCount.Value, 1, 8);
        public static int ShopPriceNormalized => Math.Clamp(ShopPrice.Value, 1, 1000);
        public static float ScanRadiusNormalized => Math.Clamp(ScanRadiusLimit.Value, 5, 2000);
        public static float BatteryCapacityNormalized => Math.Clamp(BatteryCapacity.Value, 5, 1000);
        public static string ScannerBlackListNonNull => ScannerBlackList.Value ?? string.Empty;
    }
}