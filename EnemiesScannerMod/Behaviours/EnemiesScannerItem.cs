using System;
using System.Linq;
using System.Text;
using EnemiesScannerMod.Models;
using EnemiesScannerMod.Utils;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace EnemiesScannerMod.Behaviours
{
    public class EnemiesScannerItem : PhysicsProp
    {
        private enum LedState
        {
            Disabled,
            NoDanger,
            Warning,
            Danger,
        }
        
        private const string DefaultText = "No enemies nearby";
        
        private Light _defaultLed;
        private Light _warningLed;
        private Light _dangerLed;
        private Light[] _leds;
        
        private TextMeshProUGUI _text;
        private GameObject _scanAnimationHolder;
        private DateTime _lastScan = DateTime.MinValue;
        private AudioSource _audioSource;

        private void Awake()
        {
            const float otherLedIntensity = 1f;
            const float dangerLedIntensity = 2f;
            
            var lights = GetComponentsInChildren<Light>();
            foreach (var light in lights)
            {
                light.enabled = false;
            }

            _defaultLed = lights.First(l => l.gameObject.name == "DefaultLed");
            _defaultLed.intensity = otherLedIntensity;
            
            _warningLed = lights.First(l => l.gameObject.name == "WarningLed");
            _warningLed.intensity = otherLedIntensity;
            
            _dangerLed = lights.First(l => l.gameObject.name == "DangerLed");
            _dangerLed.intensity = dangerLedIntensity;

            _leds = new[] { _defaultLed, _warningLed, _dangerLed };

            var containers = GetComponentsInChildren<Transform>();

            _audioSource = GetComponent<AudioSource>();

            _scanAnimationHolder = containers
                .First(t => t.gameObject.name == "ScreenCanvas")
                .gameObject
                .GetComponentsInChildren<Transform>()
                .First(t => t.gameObject.name == "ScanAnimation")
                .gameObject;
            
            _scanAnimationHolder.SetActive(false);
            
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _text.enabled = false;
            _text.fontSize = 16f;
            _text.SetText(DefaultText);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            ItemActivateInternal(used, buttonDown, isLocal: false);
        }

        public override void UseUpBatteries()
        {
            base.UseUpBatteries();
            SwitchLed(LedState.Disabled);
            _text.enabled = false;
            _scanAnimationHolder.SetActive(false);
        }

        public override void PocketItem()
        {
            base.PocketItem();
            SwitchLed(LedState.Disabled);
            _text.enabled = false;
            _scanAnimationHolder.SetActive(false);
        }

        public override void EquipItem()
        {
            base.EquipItem();
            if (isBeingUsed)
            {
                SwitchLed(LedState.NoDanger);
                _text.enabled = true;
                _scanAnimationHolder.SetActive(true);
                _text.SetText(DefaultText);
            }
        }

        public override void Update()
        {
            base.Update();
            if (isBeingUsed && IsDelayPass(_lastScan, 1f))
            {
                ScanEnemies();
            }
        }
        
        private void ItemActivateInternal(bool used, bool buttonDown, bool isLocal)
        {
            if (buttonDown)
            {
                if (playerHeldBy != null)
                {
                    ModLogger.Instance.LogInfo($"Held by {playerHeldBy.playerUsername}");
                    isBeingUsed = !isBeingUsed;
                    SwitchLed(isBeingUsed ? LedState.NoDanger : LedState.Disabled);
                    _text.enabled = isBeingUsed;
                    _scanAnimationHolder.SetActive(isBeingUsed);

                    if (!isLocal)
                    {
                        ItemActivateServerRpc(used, buttonDown: true);
                    }
                }
            }
        }

        private void SwitchLed(LedState ledState)
        {
            if (isPocketed)
            {
                return;
            }
            
            DisableLeds();
            
            if (ledState is LedState.Disabled)
            {
                return;
            }

            if (ledState is LedState.NoDanger)
            {
                _defaultLed.enabled = true;
                return;
            }

            if (ledState is LedState.Warning)
            {
                _warningLed.enabled = true;
                return;
            }

            if (ledState is LedState.Danger)
            {
                _dangerLed.enabled = true;
                return;
            }
        }

        private void DisableLeds()
        {
            foreach (var led in _leds)
            {
                led.enabled = false;
            }
        }

        [ServerRpc]
        public void ItemActivateServerRpc(bool used, bool buttonDown)
        {
            var networkManager = base.NetworkManager;
            if (networkManager == null || !networkManager.IsListening)
            {
                return;
            }

            ItemActivateClientRpc(used, buttonDown);
        }
        
        [ClientRpc]
        public void ItemActivateClientRpc(bool used, bool buttonDown)
        {
            var networkManager = base.NetworkManager;
            if (networkManager == null || !networkManager.IsListening)
            {
                return;
            }

            if (base.IsOwner)
            {
                return;
            }

            ItemActivateInternal(used, buttonDown, isLocal: true);
        }
        
        private void ScanEnemies()
        {
            _lastScan = DateTime.UtcNow;

            _audioSource.PlayOneShot(ModVariables.Instance.RadarScanRound, 0.3f);
            
            var selfPosition = transform.position;
            var isOutside = !isInFactory;

            var enemyAIs = FindObjectsOfType<EnemyAI>()
                .Where(enemy => !enemy.isEnemyDead)
                .Select(enemy => EnemyScanSummary.CreateFromEnemy(enemy, selfPosition));

            var turrets = FindObjectsOfType<Turret>()
                .Select(enemy => EnemyScanSummary.CreateFromTurret(enemy, selfPosition));

            var aggregateIterable = enemyAIs.Concat(turrets);

            var summary = aggregateIterable
                .Where(enemy => isOutside ? enemy.IsOutsideType : !enemy.IsOutsideType)
                .OrderByDescending(enemy => enemy.Distance)
                .Take(5)
                .ToArray();

            if (summary.Length == 0)
            {
                return;
            }

            SwitchLed(LedState.NoDanger);
            
            if (summary.Any(s => s.RelativeLevel == RelativeLevel.Same && s.DangerLevel is DangerLevel.Death))
            {
                SwitchLed(LedState.Danger);
                _audioSource.PlayOneShot(ModVariables.Instance.RadarAlertSound, 0.6f);
            }
            else if (summary.Any(s => s.RelativeLevel == RelativeLevel.Same && s.DangerLevel is DangerLevel.Danger))
            {
                SwitchLed(LedState.Warning);
            }

            var sb = new StringBuilder();

            ModLogger.Instance.LogDebug($"Player position: {selfPosition}");
            foreach (var s in summary)
            {
                ModLogger.Instance.LogDebug($"[SCAN] {s.Name} | {s.Distance}/{s.Position} | {s.UpDownIndicator}");
                sb.AppendLine($"{StringUtils.GetCloseIndicator(s.DangerLevel)} | {s.Name} | {s.Distance:F2} | {s.UpDownIndicator}");
            }

            _text.SetText(sb.ToString());
        }
        
        private static bool IsDelayPass(DateTime reference, float? customDelaySeconds = null)
        {
            return (DateTime.UtcNow - reference).TotalSeconds > (customDelaySeconds ?? 1f);
        }
    }
}