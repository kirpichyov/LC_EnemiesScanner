using System;
using System.Collections;
using System.Linq;
using System.Text;
using EnemiesScannerMod.Models;
using EnemiesScannerMod.Utils;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

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
        
        private const string DefaultScreenText = "Scan in progress...";
        private const string DefaultCounterText = "0";

        private readonly Type[] _excludeEnemies =
        {
            typeof(DocileLocustBeesAI),
            typeof(DoublewingAI),
        };
        
        private Light _defaultLed;
        private Light _warningLed;
        private Light _dangerLed;
        private Light[] _leds;
        
        private TextMeshProUGUI _screenText;
        private TextMeshProUGUI _counterText;
        private DateTime _lastScan = DateTime.MinValue;
        private AudioSource _audioSource;
        
        private float _heatValue;
        private Coroutine _cooldownCoroutine;
        private Image _heatImage;

        private Canvas _screenCanvas;
        private Canvas _counterCanvas;
        private GameObject _heatUI;

        private bool IsOverheat => EnemiesScannerModNetworkManager.Instance.EnableOverheat.Value &&
                                   _heatValue >= EnemiesScannerModNetworkManager.Instance.OverheatTime.Value;
        
        private void Awake()
        {
            const float otherLedIntensity = 1f;
            const float dangerLedIntensity = 2f;
            
            var lights = GetComponentsInChildren<Light>(includeInactive: true);
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

            _audioSource = GetComponent<AudioSource>();

            var texts = GetComponentsInChildren<TextMeshProUGUI>();
            var canvases = GetComponentsInChildren<Canvas>();
            
            _screenText = texts.First(t => t.gameObject.name == "ScreenText");
            _screenCanvas = canvases.First(t => t.gameObject.name == "ScreenCanvas");
            _screenCanvas.enabled = false;
            _screenText.fontSize = 16f;
            _screenText.SetText(DefaultScreenText);
            
            _counterText = texts.First(t => t.gameObject.name == "CounterText");
            _counterCanvas = canvases.First(t => t.gameObject.name == "CounterCanvas");
            _counterCanvas.enabled = false;
            _counterText.fontSize = 16f;
            _counterText.SetText(DefaultCounterText);

            _heatImage = GetComponentsInChildren<Image>().First(i => i.gameObject.name == "OverheatImageRed");
            _heatUI = GetComponentsInChildren<RectTransform>().First(t => t.gameObject.name == "OverheatUI").gameObject;
            
            _heatValue = 0f;
            _cooldownCoroutine = null;
        }

        public override void Start()
        {
            base.Start();

            var isOverheatEnabled = EnemiesScannerModNetworkManager.Instance.EnableOverheat.Value;
            _heatUI.SetActive(isOverheatEnabled);
            _heatImage.enabled = isOverheatEnabled;
            _heatImage.fillAmount = 0f;
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            ItemActivateInternal(used, buttonDown, isLocal: false);
        }

        public override void UseUpBatteries()
        {
            base.UseUpBatteries();
            _audioSource.PlayOneShot(ModVariables.Instance.NoPowerSound, 0.7f);
            DisableLeds();
            TurnOff(used: false);
        }

        public override void PocketItem()
        {
            base.PocketItem();
            TurnOff(used: isBeingUsed);
        }

        public override void EquipItem()
        {
            base.EquipItem();
            if (isBeingUsed)
            {
                SwitchLed(LedState.NoDanger);
                _screenCanvas.enabled = true;
                _counterCanvas.enabled = true;
                _screenText.SetText(DefaultScreenText);
            }
        }

        public override void Update()
        {
            base.Update();
            
            if (IsOverheat)
            {
                if (isBeingUsed)
                {
                    _audioSource.PlayOneShot(ModVariables.Instance.OverheatedSound, 0.5f);
                    TurnOff(used: false);
                }
                
                return;
            }
            
            if (isBeingUsed && IsDelayPass(_lastScan, 1f))
            {
                ScanEnemies();
            }
            
            if (EnemiesScannerModNetworkManager.Instance.EnableOverheat.Value)
            {
                if (isBeingUsed)
                {
                    var newOverheatValue = Mathf.Clamp(_heatValue + Time.deltaTime, 0f, 
                        EnemiesScannerModNetworkManager.Instance.OverheatTime.Value);
                    SetHeat(newOverheatValue);
                }
                else
                {
                    var newOverheatValue = Mathf.Clamp(_heatValue - Time.deltaTime, 0f,
                        EnemiesScannerModNetworkManager.Instance.OverheatTime.Value);
                    SetHeat(newOverheatValue);
                }
            }
        }
        
        private void ItemActivateInternal(bool used, bool buttonDown, bool isLocal)
        {
            if (!buttonDown)
            {
                return;
            }

            // TODO: Do we need this check?
            if (playerHeldBy == null)
            {
                return;
            }
           
            isBeingUsed = !isBeingUsed;

            if (isBeingUsed && IsOverheat)
            {
                _audioSource.PlayOneShot(ModVariables.Instance.OverheatedSound, 0.5f);
                TurnOff(used: false);
                return;
            }
            
            _audioSource.PlayOneShot(isBeingUsed
                ? ModVariables.Instance.TurnOnSound
                : ModVariables.Instance.TurnOffSound);
                    
            SwitchLed(isBeingUsed ? LedState.NoDanger : LedState.Disabled);
            _screenCanvas.enabled = isBeingUsed && !isPocketed;
            _counterCanvas.enabled = isBeingUsed && !isPocketed;

            if (!isLocal)
            {
                ItemActivateServerRpc(used, buttonDown: true);
            }
        }
        
        private void TurnOff(bool used)
        {
            if (!used)
            {
                isBeingUsed = false;
            }
            
            DisableLeds();
            _screenCanvas.enabled = false;
            _counterCanvas.enabled = false;
        }

        private void SwitchLed(LedState ledState)
        {
            DisableLeds();

            if (isPocketed)
            {
                return;
            }
            
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
            
            var selfPosition = transform.position;
            var isOutside = !isInFactory;
            
            ModLogger.Instance.LogDebug($"Player position: {selfPosition}");

            var outsideFilter = ModConfig.DisableOutsideFilter.Value
                ? (Func<EnemyAI, bool>)(enemy => true)
                : enemy => isOutside ? enemy.isOutside : !enemy.isOutside;
            
            var outsideFilterTurret = ModConfig.DisableOutsideFilter.Value
                ? (Func<Turret, bool>)(enemy => true)
                : enemy => !isOutside;

            var radiusLimitFilter = EnemiesScannerModNetworkManager.Instance.EnableScanRadiusLimit.Value
                ? (Func<EnemyScanSummary, bool>)(summary => summary.Distance <= EnemiesScannerModNetworkManager.Instance.ScanRadiusLimit.Value)
                : summary => true;

            var enemyAIs = FindObjectsOfType<EnemyAI>()
                .Where(enemy => !_excludeEnemies.Contains(enemy.GetType()))
                .Where(enemy => !enemy.isEnemyDead)
                .Where(outsideFilter)
                .Select(enemy => EnemyScanSummary.CreateFromEnemy(enemy, selfPosition));

            var turrets = FindObjectsOfType<Turret>()
                .Where(outsideFilterTurret)
                .Select(enemy => EnemyScanSummary.CreateFromTurret(enemy, selfPosition));

            var aggregate = enemyAIs.Concat(turrets)
                .Where(radiusLimitFilter)
                .ToArray();
            
            var summary = aggregate
                .OrderBy(enemy => enemy.Distance)
                .Take(ModConfig.ShowTopEnemiesCountNormalized)
                .ToArray();

            _counterText.SetText($"{aggregate.Length}");
            
            if (summary.Length == 0)
            {
                _screenText.SetText("No enemies nearby...");
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
                _audioSource.PlayOneShot(ModVariables.Instance.RadarWarningSound, 0.5f);
            }
            else
            {
                if (ModConfig.EnablePingSound.Value)
                {
                    _audioSource.PlayOneShot(ModVariables.Instance.RadarScanRound, 0.3f);
                }
            }

            var sb = new StringBuilder();

            foreach (var s in summary)
            {
                ModLogger.Instance.LogDebug($"[SCAN] {s.Name} | {s.Distance}/{s.Position} | {s.UpDownIndicator}");
                AppendScannerEntryLine(sb, s);
            }

            _screenText.SetText(sb.ToString());
        }
        
        private void SetHeat(float newValue)
        {
            _heatValue = newValue;
            var heatFillAmount = _heatValue / EnemiesScannerModNetworkManager.Instance.OverheatTime.Value;
            _heatImage.fillAmount = Mathf.Clamp(heatFillAmount, 0f, EnemiesScannerModNetworkManager.Instance.OverheatTime.Value);

            if (IsOverheat)
            {
                _cooldownCoroutine ??= StartCoroutine(StartCooldown());
            }
        }
        
        private IEnumerator StartCooldown()
        {
            yield return new WaitForSeconds(EnemiesScannerModNetworkManager.Instance.OverheatCooldownTime.Value);
            SetHeat(0f);
            _cooldownCoroutine = null;
            _audioSource.PlayOneShot(ModVariables.Instance.RebootedSound, 0.5f);
        }
        
        private static void AppendScannerEntryLine(StringBuilder stringBuilder, EnemyScanSummary s)
        {
            stringBuilder.Append($"{StringUtils.GetCloseIndicator(s.DangerLevel)} | ");
            stringBuilder.Append($"{s.Name} | ");

            if (ModConfig.EnableExactDistance.Value)
            {
                stringBuilder.Append($"{s.Distance:F2} | ");
            }

            stringBuilder.Append(s.UpDownIndicator);
            stringBuilder.AppendLine();
        }

        
        private static bool IsDelayPass(DateTime reference, float? customDelaySeconds = null)
        {
            return (DateTime.UtcNow - reference).TotalSeconds > (customDelaySeconds ?? 1f);
        }
    }
}