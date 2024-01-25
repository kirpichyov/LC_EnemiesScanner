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
        
        public static void Init()
        {
            PluginLoader.Instance.BindConfig(ref EnablePingSound, GeneralSectionName, "Enable scanner ping sound", true, EnablePingSound_Description);
            PluginLoader.Instance.BindConfig(ref ShowTopEnemiesCount, GeneralSectionName, "Count of the nearest enemies shown", 5, ShowTopEnemiesCount_Description);
            PluginLoader.Instance.BindConfig(ref DisableOutsideFilter, ExperimentalSectionName, "Disable filtering by outside enemies", false, DisableOutsideFilter_Description);
        }
        
        public static ConfigEntry<bool> EnablePingSound;
        public static ConfigEntry<int> ShowTopEnemiesCount;
        public static ConfigEntry<bool> DisableOutsideFilter;

        public static int ShowTopEnemiesCountNormalized => Math.Clamp(ShowTopEnemiesCount.Value, 1, 8);
    }
}