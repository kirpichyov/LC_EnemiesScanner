using UnityEngine;

namespace EnemiesScannerMod
{
    internal sealed class ModVariables
    {
        public static ModVariables Instance { get; private set; }
        
        public AudioClip RadarScanRound { get; set; }
        public AudioClip RadarWarningSound { get; set; }
        public AudioClip RadarAlertSound { get; set; }
        
        public static void SetInstance(ModVariables instance)
        {
            Instance = instance;
        }
    }
}