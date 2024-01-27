using LethalLib.Modules;
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

        private int _shopPriceSyncValue;
        
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
                ScanRadiusLimit.Value = ModConfig.ScanRadiusLimit.Value;
                ModLogger.Instance.LogInfo("Host sending config to clients");
            }
            else
            {
                _shopPriceSyncValue = ShopPrice.Value;
            }
            
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
        }
    }
}