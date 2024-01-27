using Unity.Netcode;

namespace EnemiesScannerMod
{
    internal sealed class EnemiesScannerModNetworkManager : NetworkBehaviour
    {
        public static EnemiesScannerModNetworkManager Instance { get; private set; }
        
        public NetworkVariable<int> ShopPrice = new NetworkVariable<int>(20);
        public NetworkVariable<bool> EnableOverheat = new NetworkVariable<bool>(false);
        public NetworkVariable<int> OverheatTime = new NetworkVariable<int>(120);
        public NetworkVariable<int> OverheatCooldownTime = new NetworkVariable<int>(120);
        
        private void Awake()
        {
            Instance = this;

            if (GameNetworkManager.Instance.isHostingGame)
            {
                ShopPrice.Value = ModConfig.ShopPriceNormalized;
                EnableOverheat.Value = ModConfig.EnableOverheat.Value;
                OverheatTime.Value = ModConfig.OverheatTime.Value;
                OverheatCooldownTime.Value = ModConfig.OverheatCooldownTime.Value;
                ModLogger.Instance.LogInfo("Host sending config to clients");
            }
            
            ModLogger.Instance.LogDebug("ModNetworkManager Awake");
        }
    }
}