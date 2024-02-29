using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using EnemiesScannerMod.Behaviours;
using HarmonyLib;
using LethalLib.Modules;
using UnityEngine;

namespace EnemiesScannerMod
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    public class PluginLoader : BaseUnityPlugin
    {
        private const string ModGuid = "Kirpichyov.EnemiesScanner";
        private const string ModName = "Kirpichyov's EnemiesScanner";
        private const string ModVersion = "1.0.6";
    
        private readonly Harmony _harmony = new Harmony(ModGuid);
        
        public static PluginLoader Instance { get; private set; }

        private void Awake()
        {
            PatchNetworking();
            
            Instance = this;
            ModLogger.SetInstance(Logger);
            ModVariables.SetInstance(new ModVariables());
            ModConfig.Init();
            AliasesConfig.Init();

            RegisterModNetworkManager();
            
            var modAssetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "enemies_scanner");
            var modBundle = AssetBundle.LoadFromFile(modAssetDir);

            RegisterEnemiesScannerItem(ref modBundle, ModConfig.ShopPriceNormalized);
            InitializeSoundVariables(ref modBundle);
            
            _harmony.PatchAll();
            
            ModLogger.Instance.LogInfo($"{ModName} loaded.");
        }

        private void RegisterEnemiesScannerItem(ref AssetBundle modBundle, int scannerPrice)
        {
            Item scannerItem = modBundle.LoadAsset<Item>("Assets/EnemiesScannerModding/EnemiesScannerItem.asset");
            scannerItem.requiresBattery = true;
            scannerItem.automaticallySetUsingPower = false;
            scannerItem.batteryUsage = ModConfig.BatteryCapacityNormalized;

            EnemiesScannerItem itemScript = scannerItem.spawnPrefab.AddComponent<EnemiesScannerItem>();
            itemScript.grabbable = true;
            itemScript.grabbableToEnemies = true;
            itemScript.insertedBattery = new Battery(false, 1f);
            itemScript.itemProperties = scannerItem;
            
            NetworkPrefabs.RegisterNetworkPrefab(scannerItem.spawnPrefab);
            Utilities.FixMixerGroups(scannerItem.spawnPrefab);
            
            TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
            node.clearPreviousText = true;
            node.displayText = "Allows to scan nearby enemies\n\n";
            ModVariables.Instance.ScannerShopItem = scannerItem;
            Items.RegisterShopItem(scannerItem, null, null, node, scannerPrice);
        }

        private void InitializeSoundVariables(ref AssetBundle modBundle)
        {
            ModVariables.Instance.RadarScanRound = modBundle.LoadAsset<AudioClip>("Assets/EnemiesScannerModding/RadarScanV2.wav");
            ModVariables.Instance.RadarWarningSound = modBundle.LoadAsset<AudioClip>("Assets/EnemiesScannerModding/RadarWarningV2.wav");
            ModVariables.Instance.RadarAlertSound = modBundle.LoadAsset<AudioClip>("Assets/EnemiesScannerModding/RadarAlertV2.wav");
            ModVariables.Instance.OverheatedSound = modBundle.LoadAsset<AudioClip>("Assets/EnemiesScannerModding/OverheatWithRobot.wav");
            ModVariables.Instance.RebootedSound = modBundle.LoadAsset<AudioClip>("Assets/EnemiesScannerModding/Rebooted.wav");
            ModVariables.Instance.NoPowerSound = modBundle.LoadAsset<AudioClip>("Assets/EnemiesScannerModding/OutOfBattery.ogg");
            ModVariables.Instance.TurnOnSound = modBundle.LoadAsset<AudioClip>("Assets/EnemiesScannerModding/detector_radio_on.ogg");
            ModVariables.Instance.TurnOffSound = modBundle.LoadAsset<AudioClip>("Assets/EnemiesScannerModding/detector_radio_off.ogg");
        }

        private void RegisterModNetworkManager()
        {
            var assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "enemiesscanner_netcodemod");
            var bundle = AssetBundle.LoadFromFile(assetDir);
            
            var netManagerPrefab = bundle.LoadAsset<GameObject>("Assets/EnemiesScannerNetcode/EnemiesScannerNetworkManager.prefab");
            netManagerPrefab.AddComponent<EnemiesScannerModNetworkManager>();
            
            ModVariables.Instance.ModNetworkManagerGameObject = netManagerPrefab;
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