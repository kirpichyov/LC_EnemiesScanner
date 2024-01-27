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
            "Determines if overheat should be enabled so scanning will be temporary disabled on overheat.";
        
        private const string OverheatTime_Description =
            "Determines how much time in seconds scanner could work without overheat. Min value is 5 and max value is 1800.";
        
        private const string OverheatCooldownTime_Description =
            "Determines how much time in seconds is needed to cool down on overheat. Min value is 5 and max value is 1800.";
        
        private const string EnableExactDistance_Description =
            "Determines if the exact distance to the enemy should be shown in the scanner.";
        
        public static void Init()
        {
            PluginLoader.Instance.BindConfig(ref EnablePingSound, GeneralSectionName, "Enable scanner ping sound", true, EnablePingSound_Description);
            PluginLoader.Instance.BindConfig(ref ShowTopEnemiesCount, GeneralSectionName, "Count of the nearest enemies shown", 5, ShowTopEnemiesCount_Description);
            PluginLoader.Instance.BindConfig(ref DisableOutsideFilter, ExperimentalSectionName, "Disable filtering by outside enemies", false, DisableOutsideFilter_Description);
            PluginLoader.Instance.BindConfig(ref ShopPrice, GeneralSectionName, "Scanner shop price", 20, ShopPrice_Description);
            PluginLoader.Instance.BindConfig(ref EnableOverheat, GeneralSectionName, "Enable scanner overheat", false, EnableOverheat_Description);
            PluginLoader.Instance.BindConfig(ref OverheatTime, GeneralSectionName, "Scanner overheat timer (seconds)", 120, OverheatTime_Description);
            PluginLoader.Instance.BindConfig(ref OverheatCooldownTime, GeneralSectionName, "Scanner cooldown timer (seconds)", 120, OverheatCooldownTime_Description);
            PluginLoader.Instance.BindConfig(ref EnableExactDistance, GeneralSectionName, "Show the exact distance to the enemy", true, EnableExactDistance_Description);
        }
        
        public static ConfigEntry<bool> EnablePingSound;
        public static ConfigEntry<int> ShowTopEnemiesCount;
        public static ConfigEntry<bool> DisableOutsideFilter;
        public static ConfigEntry<int> ShopPrice;
        public static ConfigEntry<bool> EnableOverheat;
        public static ConfigEntry<int> OverheatTime;
        public static ConfigEntry<int> OverheatCooldownTime;
        public static ConfigEntry<bool> EnableExactDistance;

        public static int ShowTopEnemiesCountNormalized => Math.Clamp(ShowTopEnemiesCount.Value, 1, 8);
        public static int ShopPriceNormalized => Math.Clamp(ShopPrice.Value, 1, 1000);
    }
}