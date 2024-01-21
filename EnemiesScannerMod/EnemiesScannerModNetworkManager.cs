using Unity.Netcode;

namespace EnemiesScannerMod
{
    internal class EnemiesScannerModNetworkManager : NetworkBehaviour
    {
        public static EnemiesScannerModNetworkManager Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
            
            ModLogger.Instance.LogDebug("ModNetworkManager Awake");
        }
    }
}