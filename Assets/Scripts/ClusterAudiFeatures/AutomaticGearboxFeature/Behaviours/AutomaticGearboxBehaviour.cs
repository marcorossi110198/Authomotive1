/// <summary>
/// AutomaticGearbox BEHAVIOUR - UI COMPONENT
/// 
/// FILE: Behaviours/AutomaticGearboxBehaviour.cs
/// 
/// MonoBehaviour per AutomaticGearbox UI
/// IDENTICO al pattern SpeedometerBehaviour seguendo modello Mercedes
/// Gestisce display RPM, marce, red zone warnings con animazioni smooth
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using ClusterAudi;
using ClusterAudiFeatures;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// MonoBehaviour per AutomaticGearbox UI
	/// Segue ESATTAMENTE il pattern BaseMonoBehaviour del modello Mercedes
	/// </summary>
	public class AutomaticGearboxBehaviour : BaseMonoBehaviour<IAutomaticGearboxFeatureInternal>
	{
		#region Serialized Fields - UI COMPONENTS

		[Header("AutomaticGearbox Display - ASSIGN IN PREFAB")]
		[Tooltip("Testo principale che mostra i RPM numerici")]
		[SerializeField] private TextMeshProUGUI _rpmValueText;

		[Tooltip("Testo che mostra l'unità di misura (RPM)")]
		[SerializeField] private TextMeshProUGUI _rpmUnitText;

		[Tooltip("Barra di progresso per visualizzazione grafica RPM")]
		[SerializeField] private Slider _rpmProgressBar;

		[Tooltip("Fill della barra di progresso per cambio colori")]
		[SerializeField] private Image _progressBarFill;

		[Header("Gear Display - ASSIGN IN PREFAB")]
		[Tooltip("Testo che mostra la marcia corrente")]
		[SerializeField] private TextMeshProUGUI _gearValueText;

		[Tooltip("Background marcia per personalizzazione")]
		[SerializeField] private Image _gearBackground;

		[Header("Red Zone Warnings - ASSIGN IN PREFAB")]
		[Tooltip("Immagine warning per red zone")]
		[SerializeField] private Image _redZoneWarningImage;

		[Tooltip("Testo warning red zone")]
		[SerializeField] private TextMeshProUGUI _redZoneWarningText;

		[Header("Shift Indicator - ASSIGN IN PREFAB")]
		[Tooltip("Indicatore shift up/down (opzionale)")]
		[SerializeField] private Image _shiftIndicator;

		[Tooltip("Testo indicatore shift")]
		[SerializeField] private TextMeshProUGUI _shiftIndicatorText;

		[Header("Optional Components - ASSIGN IF NEEDED")]
		[Tooltip("Background generale AutomaticGearbox (opzionale)")]
		[SerializeField] private Image _automaticgearboxBackground;

		[Tooltip("Testo per etichetta AutomaticGearbox (opzionale)")]
		[SerializeField] private TextMeshProUGUI _automaticgearboxLabel;

		#endregion

		#region Private Fields

		// Servizi
		private IBroadcaster _broadcaster;
		private IVehicleDataService _vehicleDataService;

		// Configurazione corrente
		private AutomaticGearboxConfig _currentConfiguration;

		// Animation e smoothing
		private float _displayedRPM = 800f;              // RPM mostrati (smoothed)
		private float _targetRPM = 800f;                // RPM target dal servizio
		private int _displayedGear = 0;                 // Marcia mostrata
		private int _targetGear = 0;                    // Marcia target dal servizio

		// Colori
		private Color _currentRPMColor = Color.white;
		private Color _targetRPMColor = Color.white;

		// Red Zone Management
		private bool _isInRedZone = false;
		private bool _isRedZoneFlashing = false;
		private bool _showRedZoneWarning = false;

		// Coroutines
		private Coroutine _rpmAnimationCoroutine;
		private Coroutine _colorTransitionCoroutine;
		private Coroutine _redZoneFlashCoroutine;
		private Coroutine _gearTransitionCoroutine;

		// Performance tracking
		private float _lastUpdateTime;
		private int _frameCounter;

		#endregion

		#region BaseMonoBehaviour Override

		/// <summary>
		/// Inizializzazione - IDENTICA al pattern Mercedes
		/// </summary>
		protected override void ManagedAwake()
		{
			Debug.Log("[AutomaticGearbox] 🏁 AutomaticGearboxBehaviour inizializzato");

			// Ottieni servizi dal Client via feature
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();
			_vehicleDataService = client.Services.Get<IVehicleDataService>();

			// Ottieni configurazione iniziale dalla feature
			_currentConfiguration = _feature.GetCurrentConfiguration();

			// Setup UI e validazione
			ValidateUIComponents();
			SetupInitialUI();
			SubscribeToEvents();
		}

		/// <summary>
		/// Avvio - Setup iniziale RPM
		/// </summary>
		protected override void ManagedStart()
		{
			Debug.Log("[AutomaticGearbox] ▶️ AutomaticGearbox UI avviata");

			// Imposta valori iniziali
			_targetRPM = _vehicleDataService.CurrentRPM;
			_displayedRPM = _targetRPM;
			_targetGear = _vehicleDataService.CurrentGear;
			_displayedGear = _targetGear;

			// Setup UI iniziale
			UpdateRPMDisplay();
			UpdateGearDisplay();
			UpdateProgressBar();
		}

		/// <summary>
		/// Update continuo - Animazioni smooth
		/// </summary>
		protected override void ManagedUpdate()
		{
			// Update smooth dei RPM
			UpdateSmoothRPM();

			// Update colori se necessario
			UpdateRPMColors();

			// Update red zone check
			UpdateRedZoneStatus();

			// Update UI
			UpdateRPMDisplay();
			UpdateGearDisplay();
			UpdateProgressBar();

			// Performance tracking (optional)
			TrackPerformance();
		}

		/// <summary>
		/// Cleanup
		/// </summary>
		protected override void ManagedOnDestroy()
		{
			Debug.Log("[AutomaticGearbox] 🗑️ AutomaticGearboxBehaviour distrutto");

			// Cleanup eventi
			UnsubscribeFromEvents();

			// Stop coroutines
			StopAllCoroutines();
		}

		#endregion

		#region Public Methods (Called by Feature)

		/// <summary>
		/// Applica configurazione AutomaticGearbox
		/// Chiamato da AutomaticGearboxFeature quando cambia modalità guida
		/// </summary>
		public void ApplyConfiguration(AutomaticGearboxConfig config)
		{
			_currentConfiguration = config;

			// Aggiorna progress bar range
			if (_rpmProgressBar != null)
			{
				_rpmProgressBar.maxValue = config.MaxDisplayRPM;
			}

			// Aggiorna visibility componenti
			UpdateComponentVisibility();

			Debug.Log($"[AutomaticGearbox] 🔧 Configurazione applicata: MaxRPM={config.MaxDisplayRPM}, RedZone={config.ShowRedZone}");
		}

		/// <summary>
		/// Imposta red zone warning
		/// Chiamato da AutomaticGearboxFeature per warnings
		/// </summary>
		public void SetRedZoneWarning(bool showWarning)
		{
			_showRedZoneWarning = showWarning;

			if (_redZoneWarningImage != null)
			{
				_redZoneWarningImage.gameObject.SetActive(showWarning);
			}

			if (_redZoneWarningText != null)
			{
				_redZoneWarningText.gameObject.SetActive(showWarning);
			}
		}

		/// <summary>
		/// Abilita/disabilita shift indicator
		/// </summary>
		public void SetShiftIndicatorEnabled(bool enabled)
		{
			if (_shiftIndicator != null)
			{
				_shiftIndicator.gameObject.SetActive(enabled);
			}

			if (_shiftIndicatorText != null)
			{
				_shiftIndicatorText.gameObject.SetActive(enabled);
			}
		}

		#endregion

		#region UI Setup Methods

		/// <summary>
		/// Validazione componenti UI - IDENTICA al pattern Mercedes
		/// </summary>
		private void ValidateUIComponents()
		{
			int missingComponents = 0;

			// Componenti essenziali
			if (_rpmValueText == null)
			{
				Debug.LogError("[AutomaticGearbox] ❌ _rpmValueText non assegnato nel prefab!");
				missingComponents++;
			}

			if (_rpmUnitText == null)
			{
				Debug.LogError("[AutomaticGearbox] ❌ _rpmUnitText non assegnato nel prefab!");
				missingComponents++;
			}

			if (_rpmProgressBar == null)
			{
				Debug.LogError("[AutomaticGearbox] ❌ _rpmProgressBar non assegnato nel prefab!");
				missingComponents++;
			}

			if (_gearValueText == null)
			{
				Debug.LogError("[AutomaticGearbox] ❌ _gearValueText non assegnato nel prefab!");
				missingComponents++;
			}

			// Componenti opzionali (warning non error)
			if (_progressBarFill == null)
			{
				Debug.LogWarning("[AutomaticGearbox] ⚠️ _progressBarFill non assegnato (cambio colore limitato)");
			}

			if (_redZoneWarningImage == null)
			{
				Debug.LogWarning("[AutomaticGearbox] ⚠️ _redZoneWarningImage non assegnato (red zone warnings limitati)");
			}

			// Risultato validazione
			if (missingComponents == 0)
			{
				Debug.Log("[AutomaticGearbox] ✅ Tutti i componenti essenziali assegnati");
			}
			else
			{
				Debug.LogError($"[AutomaticGearbox] ❌ {missingComponents} componenti essenziali mancanti!");
			}
		}

		/// <summary>
		/// Setup iniziale dell'UI
		/// </summary>
		private void SetupInitialUI()
		{
			// Setup testo RPM
			if (_rpmValueText != null)
			{
				_rpmValueText.text = "800";
				_rpmValueText.color = AutomaticGearboxData.DEFAULT_RPM_COLOR;
			}

			// Setup testo unità
			if (_rpmUnitText != null)
			{
				_rpmUnitText.text = AutomaticGearboxData.RPM_UNIT_LABEL;
				_rpmUnitText.color = AutomaticGearboxData.DEFAULT_RPM_COLOR;
			}

			// Setup progress bar
			if (_rpmProgressBar != null)
			{
				_rpmProgressBar.minValue = 0f;
				_rpmProgressBar.maxValue = _currentConfiguration?.MaxDisplayRPM ?? AutomaticGearboxData.MAX_DISPLAY_RPM;
				_rpmProgressBar.value = AutomaticGearboxData.IDLE_RPM;
				_rpmProgressBar.interactable = false;
			}

			// Setup colori progress bar
			if (_progressBarFill != null)
			{
				_progressBarFill.color = AutomaticGearboxData.IDLE_RPM_COLOR;
			}

			// Setup gear display
			if (_gearValueText != null)
			{
				_gearValueText.text = "P";
				_gearValueText.color = AutomaticGearboxData.GEAR_ACTIVE_COLOR;
			}

			// Setup red zone warnings (inizialmente nascosti)
			SetRedZoneWarning(false);

			// Setup shift indicator (inizialmente nascosto)
			SetShiftIndicatorEnabled(false);

			// Setup label opzionale
			if (_automaticgearboxLabel != null && string.IsNullOrEmpty(_automaticgearboxLabel.text))
			{
				_automaticgearboxLabel.text = "RPM";
			}

			Debug.Log("[AutomaticGearbox] ✅ UI iniziale configurata");
		}

		/// <summary>
		/// Aggiorna visibility componenti basata su configurazione
		/// </summary>
		private void UpdateComponentVisibility()
		{
			// Red zone visibility
			bool showRedZone = _currentConfiguration?.ShowRedZone ?? true;
			if (_redZoneWarningImage != null && !_showRedZoneWarning)
			{
				_redZoneWarningImage.gameObject.SetActive(false);
			}

			// Shift indicator visibility
			bool showShiftIndicator = _currentConfiguration?.ShowShiftIndicator ?? false;
			SetShiftIndicatorEnabled(showShiftIndicator);

			// Gear display visibility  
			bool showGearDisplay = _currentConfiguration?.ShowGearDisplay ?? true;
			if (_gearValueText != null)
			{
				_gearValueText.gameObject.SetActive(showGearDisplay);
			}
		}

		#endregion

		#region Event System

		/// <summary>
		/// Sottoscrizione agli eventi
		/// </summary>
		private void SubscribeToEvents()
		{
			// Sottoscrivi a eventi RPM dal VehicleDataService
			_vehicleDataService.OnRPMChanged += OnRPMChanged;
			_vehicleDataService.OnGearChanged += OnGearChanged;

			// Sottoscrivi a eventi modalità per configurazione dinamica
			_broadcaster.Add<DriveModeChangedEvent>(OnDriveModeChanged);

			Debug.Log("[AutomaticGearbox] 📡 Eventi sottoscritti");
		}

		/// <summary>
		/// Rimozione sottoscrizioni eventi
		/// </summary>
		private void UnsubscribeFromEvents()
		{
			// Rimuovi eventi RPM e Gear
			if (_vehicleDataService != null)
			{
				_vehicleDataService.OnRPMChanged -= OnRPMChanged;
				_vehicleDataService.OnGearChanged -= OnGearChanged;
			}

			// Rimuovi eventi broadcaster
			if (_broadcaster != null)
			{
				_broadcaster.Remove<DriveModeChangedEvent>(OnDriveModeChanged);
			}

			Debug.Log("[AutomaticGearbox] 📡 Eventi rimossi");
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Gestisce cambio RPM dal VehicleDataService
		/// </summary>
		private void OnRPMChanged(float newRPM)
		{
			_targetRPM = newRPM;

			// Aggiorna colore target basato su RPM
			_targetRPMColor = AutomaticGearboxData.GetRPMColor(newRPM);

			// Check red zone per warnings
			if (AutomaticGearboxData.IsInRedZone(newRPM))
			{
				HandleRedZoneEntry();
			}
			else
			{
				HandleRedZoneExit();
			}
		}

		/// <summary>
		/// Gestisce cambio marcia dal VehicleDataService
		/// </summary>
		private void OnGearChanged(int newGear)
		{
			_targetGear = newGear;

			// Anima transizione marcia se necessario
			if (_gearTransitionCoroutine != null)
			{
				StopCoroutine(_gearTransitionCoroutine);
			}

			_gearTransitionCoroutine = StartCoroutine(AnimateGearTransition());
		}

		/// <summary>
		/// Gestisce cambio modalità guida
		/// </summary>
		private void OnDriveModeChanged(DriveModeChangedEvent e)
		{
			// La configurazione verrà aggiornata da AutomaticGearboxFeature
			Debug.Log($"[AutomaticGearbox] 🔄 Modalità cambiata: {e.NewMode}");
		}

		#endregion

		#region RPM Animation & Updates

		/// <summary>
		/// Update smooth dei RPM con damping configurabile
		/// </summary>
		private void UpdateSmoothRPM()
		{
			if (Mathf.Approximately(_displayedRPM, _targetRPM))
				return;

			// Calcola damping basato su configurazione modalità
			float damping = _currentConfiguration?.ResponseDamping ?? AutomaticGearboxData.COMFORT_RESPONSE_DAMPING;

			// Smooth lerp verso RPM target
			_displayedRPM = Mathf.Lerp(_displayedRPM, _targetRPM, damping * Time.deltaTime * 10f);

			// Snap se molto vicini
			if (Mathf.Abs(_displayedRPM - _targetRPM) < 10f)
			{
				_displayedRPM = _targetRPM;
			}
		}

		/// <summary>
		/// Update smooth dei colori RPM
		/// </summary>
		private void UpdateRPMColors()
		{
			if (_currentRPMColor == _targetRPMColor)
				return;

			// Transizione colore smooth
			_currentRPMColor = Color.Lerp(_currentRPMColor, _targetRPMColor, Time.deltaTime * 5f);

			// Applica colore a UI elements
			if (_rpmValueText != null)
				_rpmValueText.color = _currentRPMColor;

			if (_progressBarFill != null)
				_progressBarFill.color = _currentRPMColor;
		}

		/// <summary>
		/// Aggiorna display RPM numerici
		/// </summary>
		private void UpdateRPMDisplay()
		{
			if (_rpmValueText == null) return;

			// Formatta RPM per display
			string formattedRPM = AutomaticGearboxData.FormatRPM(_displayedRPM);
			_rpmValueText.text = formattedRPM;
		}

		/// <summary>
		/// Aggiorna progress bar RPM
		/// </summary>
		private void UpdateProgressBar()
		{
			if (_rpmProgressBar == null) return;

			// Aggiorna valore progress bar
			_rpmProgressBar.value = Mathf.Clamp(_displayedRPM, 0f, _rpmProgressBar.maxValue);
		}

		/// <summary>
		/// Aggiorna display marcia
		/// </summary>
		private void UpdateGearDisplay()
		{
			if (_gearValueText == null) return;

			// Formatta marcia per display
			string formattedGear = AutomaticGearboxData.FormatGear(_displayedGear);
			_gearValueText.text = formattedGear;
		}

		#endregion

		#region Red Zone Management

		/// <summary>
		/// Aggiorna stato red zone
		/// </summary>
		private void UpdateRedZoneStatus()
		{
			bool wasInRedZone = _isInRedZone;
			_isInRedZone = AutomaticGearboxData.IsInRedZone(_displayedRPM);

			// Gestisci transizioni red zone
			if (_isInRedZone && !wasInRedZone)
			{
				HandleRedZoneEntry();
			}
			else if (!_isInRedZone && wasInRedZone)
			{
				HandleRedZoneExit();
			}
		}

		/// <summary>
		/// Gestisce entrata in red zone
		/// </summary>
		private void HandleRedZoneEntry()
		{
			if (!_currentConfiguration.ShowRedZone) return;

			Debug.Log("[AutomaticGearbox] 🚨 RED ZONE ENTERED!");

			// Mostra warning
			SetRedZoneWarning(true);

			// Avvia flash se abilitato
			if (_currentConfiguration.RedZoneFlashEnabled && !_isRedZoneFlashing)
			{
				_redZoneFlashCoroutine = StartCoroutine(RedZoneFlashEffect());
			}

			// Update warning text
			if (_redZoneWarningText != null)
			{
				_redZoneWarningText.text = "RED LINE!";
				_redZoneWarningText.color = AutomaticGearboxData.RED_ZONE_COLOR;
			}
		}

		/// <summary>
		/// Gestisce uscita da red zone
		/// </summary>
		private void HandleRedZoneExit()
		{
			Debug.Log("[AutomaticGearbox] ✅ RED ZONE EXITED");

			// Nascondi warning
			SetRedZoneWarning(false);

			// Stoppa flash
			if (_redZoneFlashCoroutine != null)
			{
				StopCoroutine(_redZoneFlashCoroutine);
				_redZoneFlashCoroutine = null;
				_isRedZoneFlashing = false;
			}
		}

		/// <summary>
		/// Effetto flash red zone
		/// </summary>
		private IEnumerator RedZoneFlashEffect()
		{
			_isRedZoneFlashing = true;

			while (_isInRedZone && _currentConfiguration.RedZoneFlashEnabled)
			{
				// Flash warning image
				if (_redZoneWarningImage != null)
				{
					_redZoneWarningImage.color = AutomaticGearboxData.RED_ZONE_FLASH_COLOR;
					yield return new WaitForSeconds(0.1f);
					_redZoneWarningImage.color = Color.clear;
					yield return new WaitForSeconds(0.1f);
				}
				else
				{
					yield return new WaitForSeconds(0.2f);
				}
			}

			_isRedZoneFlashing = false;
		}

		#endregion

		#region Gear Animation

		/// <summary>
		/// Anima transizione marcia
		/// </summary>
		private IEnumerator AnimateGearTransition()
		{
			float duration = 0.3f;
			float elapsedTime = 0f;

			// Animazione scale per evidenziare cambio marcia
			Vector3 originalScale = _gearValueText.transform.localScale;
			Vector3 targetScale = originalScale * 1.2f;

			// Scale up
			while (elapsedTime < duration / 2f)
			{
				elapsedTime += Time.deltaTime;
				float t = elapsedTime / (duration / 2f);
				_gearValueText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
				yield return null;
			}

			// Cambia marcia nel mezzo dell'animazione
			_displayedGear = _targetGear;

			// Scale down
			elapsedTime = 0f;
			while (elapsedTime < duration / 2f)
			{
				elapsedTime += Time.deltaTime;
				float t = elapsedTime / (duration / 2f);
				_gearValueText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
				yield return null;
			}

			_gearValueText.transform.localScale = originalScale;
			_gearTransitionCoroutine = null;
		}

		#endregion

		#region Performance Tracking

		/// <summary>
		/// Tracking performance per debug (opzionale)
		/// </summary>
		private void TrackPerformance()
		{
			_frameCounter++;

			if (Time.time - _lastUpdateTime >= 1.0f) // Ogni secondo
			{
				float fps = _frameCounter / (Time.time - _lastUpdateTime);

				if (fps < 30f) // Warning se FPS bassi
				{
					Debug.LogWarning($"[AutomaticGearbox] ⚠️ Performance warning: {fps:F1} FPS");
				}

				_lastUpdateTime = Time.time;
				_frameCounter = 0;
			}
		}

		#endregion

		#region Development Notes

		/*
         * AutomaticGearbox BEHAVIOUR - IMPLEMENTAZIONE COMPLETA
         * 
         * PATTERN SEGUITO:
         * - BaseMonoBehaviour<T>: Pattern Mercedes per UI components
         * - SerializeField validation: Controllo componenti prefab
         * - Event subscription: Risponde a OnRPMChanged + OnGearChanged
         * - Smooth animations: Lerp per RPM, colori, marce
         * 
         * FUNZIONALITÀ IMPLEMENTATE:
         * 1. Display RPM numerici con unità
         * 2. Progress bar grafica con colori
         * 3. Display marcia corrente con animazioni
         * 4. Red zone warnings con flash effect
         * 5. Shift indicators opzionali
         * 6. Configurazione per modalità guida
         * 7. Integration completa con VehicleDataService
         * 
         * UI COMPONENTS RICHIESTI NEL PREFAB:
         * - TextMeshProUGUI per RPM numerici
         * - TextMeshProUGUI per unità RPM
         * - Slider per progress bar  
         * - Image per fill progress bar (cambio colori)
         * - TextMeshProUGUI per marcia corrente
         * - Image per red zone warnings
         * - Componenti opzionali per shift indicators
         * 
         * DIFFERENZE da SpeedometerBehaviour:
         * - Red zone management completo
         * - Gear display con animazioni
         * - Flash effects per warnings
         * - Range colori specifico per RPM
         * 
         * PROSSIMO STEP:
         * - Creare prefab AutomaticGearboxPrefab
         * - Testare integration con VehicleDataService
         * - Verificare automatic transmission logic
         */

		#endregion
	}
}