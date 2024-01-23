using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using EnemiesScannerMod.Behaviours;
using LethalLib.Modules;
using UnityEngine;

namespace EnemiesScannerMod
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    public class PluginLoader : BaseUnityPlugin
    {
        private const string ModGuid = "Kirpichyov.EnemiesScanner";
        private const string ModName = "Kirpichyov's EnemiesScanner";
        private const string ModVersion = "1.0.1";
    
        public static PluginLoader Instance { get; private set; }

        private void Awake()
        {
            PatchNetworking();
            
            Instance = this;
            ModLogger.SetInstance(Logger);
            ModVariables.SetInstance(new ModVariables());
        
            var modAssetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "enemies_scanner");
            var modBundle = AssetBundle.LoadFromFile(modAssetDir);

            RegisterEnemiesScannerItem(ref modBundle);
            InitializeSoundVariables(ref modBundle);
            
            ModLogger.Instance.LogInfo($"{ModName} loaded.");
        }

        private void RegisterEnemiesScannerItem(ref AssetBundle modBundle)
        {
            Item scannerItem = modBundle.LoadAsset<Item>("Assets/EnemiesScannerModding/EnemiesScannerItem.asset");
            scannerItem.batteryUsage = 1000f;

            EnemiesScannerItem itemScript = scannerItem.spawnPrefab.AddComponent<EnemiesScannerItem>();
            itemScript.grabbable = true;
            itemScript.grabbableToEnemies = true;
            // TODO: issue, on save load scanner is charged. But without this line scanner has zero battery. 
            itemScript.insertedBattery = new Battery(false, 1f);
            itemScript.itemProperties = scannerItem;
            
            NetworkPrefabs.RegisterNetworkPrefab(scannerItem.spawnPrefab);
            Utilities.FixMixerGroups(scannerItem.spawnPrefab);
            
            TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
            node.clearPreviousText = true;
            node.displayText = "Allows to scan nearby enemies\n\n";
            Items.RegisterShopItem(scannerItem, null, null, node, 20);
        }

        private void InitializeSoundVariables(ref AssetBundle modBundle)
        {
            AudioClip radarScanSound = modBundle.LoadAsset<AudioClip>("Assets/EnemiesScannerModding/RadarScan.wav");
            AudioClip radarAlertSound = modBundle.LoadAsset<AudioClip>("Assets/EnemiesScannerModding/RadarAlert.wav");
            ModVariables.Instance.RadarScanRound = radarScanSound;
            ModVariables.Instance.RadarAlertSound = radarAlertSound;
        }

        private void PatchNetworking()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
        
        public void BindConfig<T>(ref ConfigEntry<T> config, string section, string key, T defaultValue, string description = "")
        {
            config = Config.Bind(section, key, defaultValue, description);
        }
    }
}