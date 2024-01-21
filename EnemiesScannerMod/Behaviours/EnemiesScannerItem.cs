using System;
using System.Linq;
using System.Text;
using EnemiesScannerMod.Models;
using EnemiesScannerMod.Utils;
using GameNetcodeStuff;
using TMPro;
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
        
        private readonly Color _noDangerColor = new Color(0.153f, 1f, 0f, 1f);
        private readonly Color _dangerColor = new Color(1f, 0.130f, 0f, 1f);
        private readonly Color _warningColor = new Color(1f, 0.548f, 0f, 1f);
        private Light _ledIndicator;
        private Light _screenLight;
        private TextMeshProUGUI _text;
        private GameObject _scanAnimationHolder;
        private DateTime _lastScan = DateTime.MinValue;
        private AudioSource _audioSource;

        private void Awake()
        {
            var lights = GetComponentsInChildren<Light>();
            foreach (var light in lights)
            {
                light.enabled = false;
            }

            var containers = GetComponentsInChildren<Transform>();

            _scanAnimationHolder = containers
                .First(t => t.gameObject.name == "ScreenCanvas")
                .gameObject
                .GetComponentsInChildren<Transform>()
                .First(t => t.gameObject.name == "ScanAnimation")
                .gameObject;
            
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _audioSource = GetComponent<AudioSource>();
            _ledIndicator = lights.First(l => l.gameObject.name == "Indicator");
            _screenLight = lights.First(l => l.gameObject.name == "ScreenLight");
            _ledIndicator.intensity = 1f;
            _screenLight.intensity = 1f;
            _text.enabled = false;
            _scanAnimationHolder.SetActive(false);
            _text.fontSize = 16f;
            _text.SetText(DefaultText);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            ModLogger.Instance.LogInfo($"used={used} | buttonDown={buttonDown}");
            if (buttonDown)
            {
                if (playerHeldBy != null)
                {
                    ModLogger.Instance.LogInfo($"Held by {playerHeldBy.playerUsername}");
                    isBeingUsed = !isBeingUsed;
                    SwitchLed(isBeingUsed ? LedState.NoDanger : LedState.Disabled);
                    _screenLight.enabled = isBeingUsed;
                    _text.enabled = isBeingUsed;
                    _scanAnimationHolder.SetActive(isBeingUsed);
                }
            }
        }

        public override void UseUpBatteries()
        {
            base.UseUpBatteries();
            SwitchLed(LedState.Disabled);
            _screenLight.enabled = false;
            _text.enabled = false;
            _scanAnimationHolder.SetActive(false);
        }

        public override void PocketItem()
        {
            base.PocketItem();
            SwitchLed(LedState.Disabled);
            _screenLight.enabled = false;
            _text.enabled = false;
            _scanAnimationHolder.SetActive(false);
        }

        public override void EquipItem()
        {
            base.EquipItem();
            if (isBeingUsed)
            {
                SwitchLed(LedState.NoDanger);
                _screenLight.enabled = true;
                _text.enabled = true;
                _scanAnimationHolder.SetActive(true);
                _text.SetText(DefaultText);
            }
        }

        public override void Update()
        {
            base.Update();
            if (isBeingUsed && IsDelayPass(_lastScan, 1f) && !ReferenceEquals(playerHeldBy, null))
            {
                ScanEnemies(playerHeldBy);
            }
        }

        private void SwitchLed(LedState ledState)
        {
            if (ledState is LedState.Disabled)
            {
                _ledIndicator.enabled = false;
                return;
            }

            if (ledState is LedState.NoDanger)
            {
                _ledIndicator.color = _noDangerColor;
                _ledIndicator.enabled = true;
                return;
            }

            if (ledState is LedState.Warning)
            {
                _ledIndicator.color = _warningColor;
                _ledIndicator.enabled = true;
                return;
            }

            if (ledState is LedState.Danger)
            {
                _ledIndicator.color = _dangerColor;
                _ledIndicator.enabled = true;
                return;
            }
        }
        
        private void ScanEnemies(PlayerControllerB currentPlayer)
        {
            _lastScan = DateTime.UtcNow;

            _audioSource.PlayOneShot(ModVariables.Instance.RadarScanRound, 0.2f);
            
            var playerPosition = currentPlayer.transform.position;

            var enemyAIs = FindObjectsOfType<EnemyAI>()
                .Where(enemy => !enemy.isEnemyDead)
                .Select(enemy => EnemyScanSummary.CreateFromEnemy(enemy, playerPosition));

            var turrets = FindObjectsOfType<Turret>()
                .Select(enemy => EnemyScanSummary.CreateFromTurret(enemy, playerPosition));

            var aggregateIterable = enemyAIs.Concat(turrets);
            var summary = aggregateIterable.OrderByDescending(s => s.Distance)
                .Take(5)
                .ToArray();

            if (summary.Length == 0)
            {
                return;
            }

            if (summary.Any(s => s.RelativeLevel == RelativeLevel.Same && s.Distance <= 10f))
            {
                _audioSource.PlayOneShot(ModVariables.Instance.RadarAlertSound, 0.6f);
            }

            var sb = new StringBuilder();

            ModLogger.Instance.LogDebug($"Player position: {playerPosition}");
            foreach (var s in summary)
            {
                ModLogger.Instance.LogDebug($"[SCAN] {s.Name} | {s.Distance}/{s.Position} | {s.UpDownIndicator}");
                sb.AppendLine($"{StringUtils.GetCloseIndicator(s.Distance)} | {s.Name} | {s.Distance} | {s.UpDownIndicator}");
            }
            
            // TODO: Make UI instead pure text (3 images of nearby enemies) +description of distance, danger etc
            _text.SetText(sb.ToString());
        }
        
        private static bool IsDelayPass(DateTime reference, float? customDelaySeconds = null)
        {
            return (DateTime.UtcNow - reference).TotalSeconds > (customDelaySeconds ?? 1f);
        }
    }
}