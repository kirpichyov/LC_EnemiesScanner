using System;
using LethalLib.Modules;
using Unity.Collections;
using Unity.Netcode;

namespace EnemiesScannerMod
{
    internal sealed class EnemiesScannerModNetworkManager : NetworkBehaviour
    {
        public static EnemiesScannerModNetworkManager Instance { get; private set; }
        
        public NetworkVariable<int> ShopPrice { get; } = new NetworkVariable<int>(20);
        public NetworkVariable<bool> EnableOverheat { get; } = new NetworkVariable<bool>(false);
        public NetworkVariable<int> OverheatTime { get; } = new NetworkVariable<int>(120);
        public NetworkVariable<int> OverheatCooldownTime { get; } = new NetworkVariable<int>(120);
        public NetworkVariable<bool> EnableScanRadiusLimit { get; } = new NetworkVariable<bool>(false);
        public NetworkVariable<float> ScanRadiusLimit { get; } = new NetworkVariable<float>(50f);
        public NetworkVariable<float> BatteryCapacity { get; } = new NetworkVariable<float>(600f);
        public NetworkVariable<FixedString512Bytes> ScannerBlackList { get; } = new NetworkVariable<FixedString512Bytes>(string.Empty);
        public string[] ScannerBlackListNormalized { get; private set; }

        private int _shopPriceSyncValue;
        private float _batteryCapacitySyncValue;
        private FixedString512Bytes _scannerBlackListSyncValue;
        
        private void Awake()
        {
            Instance = this;

            if (GameNetworkManager.Instance.isHostingGame)
            {
                ShopPrice.Value = ModConfig.ShopPriceNormalized;
                EnableOverheat.Value = ModConfig.EnableOverheat.Value;
                OverheatTime.Value = ModConfig.OverheatTime.Value;
                OverheatCooldownTime.Value = ModConfig.OverheatCooldownTime.Value;
                EnableScanRadiusLimit.Value = ModConfig.EnableScanRadiusLimit.Value;
                ScanRadiusLimit.Value = ModConfig.ScanRadiusNormalized;
                BatteryCapacity.Value = ModConfig.BatteryCapacityNormalized;
                ScannerBlackList.Value = ModConfig.ScannerBlackListNonNull;
                ModLogger.Instance.LogInfo("Host sending config to clients");
            }
            else
            {
                _shopPriceSyncValue = ShopPrice.Value;
                _batteryCapacitySyncValue = BatteryCapacity.Value;
                _scannerBlackListSyncValue = ModConfig.ScannerBlackListNonNull;
            }
            
            ScannerBlackListNormalized = ExtractScannerBlackList(ModConfig.ScannerBlackListNonNull);
            
            ModLogger.Instance.LogDebug("ModNetworkManager Awake");
        }

        private void Update()
        {
            if (GameNetworkManager.Instance.isHostingGame)
            {
                return;
            }

            if (_shopPriceSyncValue != Instance.ShopPrice.Value)
            {
                ModLogger.Instance.LogInfo($"Shop price sync in progress. Local was {_shopPriceSyncValue} | Server is {Instance.ShopPrice.Value}");
                _shopPriceSyncValue = Instance.ShopPrice.Value;
                Items.UpdateShopItemPrice(ModVariables.Instance.ScannerShopItem, Instance.ShopPrice.Value);
            }

            if (Math.Abs(_batteryCapacitySyncValue - Instance.BatteryCapacity.Value) > 0.1f)
            {
                ModLogger.Instance.LogInfo($"Battery capacity sync in progress. Local was {_batteryCapacitySyncValue} | Server is {Instance.BatteryCapacity.Value}");
                _batteryCapacitySyncValue = Instance.BatteryCapacity.Value;
                ModVariables.Instance.ScannerShopItem.batteryUsage = Instance.BatteryCapacity.Value;
            }

            if (_scannerBlackListSyncValue != Instance.ScannerBlackList.Value)
            {
                ModLogger.Instance.LogInfo($"Scanner black list sync in progress. Local was {_scannerBlackListSyncValue} | Server is {Instance.ScannerBlackList.Value}");
                _scannerBlackListSyncValue = Instance.ScannerBlackList.Value;
                ScannerBlackListNormalized = ExtractScannerBlackList(Instance.ScannerBlackList.Value.ToString());
            }
        }

        private string[] ExtractScannerBlackList(string raw)
        {
            return raw.ToLowerInvariant().Split(';', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}