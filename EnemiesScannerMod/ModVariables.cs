using UnityEngine;

namespace EnemiesScannerMod
{
    internal class ModVariables
    {
        public static ModVariables Instance { get; private set; }
        
        public AudioClip RadarScanRound { get; set; }
        public AudioClip RadarAlertSound { get; set; }
        
        public static void SetInstance(ModVariables instance)
        {
            Instance = instance;
        }
    }
}