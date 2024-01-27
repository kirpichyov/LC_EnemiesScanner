﻿using UnityEngine;

namespace EnemiesScannerMod
{
    internal sealed class ModVariables
    {
        public static ModVariables Instance { get; private set; }
        
        public AudioClip RadarScanRound { get; set; }
        public AudioClip RadarWarningSound { get; set; }
        public AudioClip RadarAlertSound { get; set; }
        public AudioClip OverheatedSound { get; set; }
        public AudioClip RebootedSound { get; set; }
        
        public GameObject ModNetworkManagerGameObject { get; set; }
        
        public static void SetInstance(ModVariables instance)
        {
            Instance = instance;
        }
    }
}