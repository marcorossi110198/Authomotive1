/// <summary>
/// SPEEDOMETER BEHAVIOUR - UI COMPONENT
/// 
/// FILE: Behaviours/SpeedometerBehaviour.cs
/// 
/// MonoBehaviour per Speedometer UI
/// IDENTICO al pattern ClusterDriveModeBehaviour seguendo modello Mercedes
/// Gestisce display velocità con animazioni smooth e cambio colori
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
	/// MonoBehaviour per Speedometer UI
	/// Segue ESATTAMENTE il pattern BaseMonoBehaviour del modello Mercedes
	/// </summary>
	public class SpeedometerBehaviour : BaseMonoBehaviour<ISpeedometerFeatureInternal>
	{
		#region Serialized Fields - UI COMPONENTS

		[Header("Speedometer Display - ASSIGN IN PREFAB")]
		[Tooltip("Testo principale che mostra la velocità numerica")]
		[SerializeField] private TextMeshProUGUI _speedValueText;

		[Tooltip("Testo che mostra l'unità di misura (km/h o mph)")]
		[SerializeField] private TextMeshProUGUI _speedUnitText;

		[Tooltip("Barra di progresso per visualizzazione grafica velocità")]
		[SerializeField] private Slider _speedProgressBar;

		[Tooltip("Background della barra di progresso per personalizzazione")]
		[SerializeField] private Image _progressBarBackground;

		[Tooltip("Fill della barra di progresso per cambio colori")]
		[SerializeField] private Image _progressBarFill;

		[Header("Optional Components - ASSIGN IF NEEDED")]
		[Tooltip("Testo per etichetta speedometer (opzionale)")]
		[SerializeField] private TextMeshProUGUI _speedometerLabel;

		[Tooltip("Background generale speedometer (opzionale)")]
		[SerializeField] private Image _speedometerBackground;

		#endregion

		#region Private Fields

		// Servizi
		private IBroadcaster _broadcaster;
		private IVehicleDataService _vehicleDataService;

		// Configurazione corrente
		private SpeedometerConfig _currentConfiguration;
		private SpeedometerData.SpeedUnit _currentSpeedUnit = SpeedometerData.SpeedUnit.KilometersPerHour;

		// Animation e smoothing
		private float _displayedSpeed = 0f;          // Velocità mostrata (smoothed)
		private float _targetSpeed = 0f;            // Velocità target dal servizio
		private Color _currentSpeedColor = Color.white;
		private Color _targetSpeedColor = Color.white;

		// Coroutines
		private Coroutine _speedAnimationCoroutine;
		private Coroutine _colorTransitionCoroutine;

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
			Debug.Log("[SPEEDOMETER] 🏎️ SpeedometerBehaviour inizializzato");

			// Ottieni servizi dal Client via feature
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();
			_vehicleDataService = client.Services.Get<IVehicleDataService>();

			// Ottieni configurazione iniziale dalla feature
			_currentConfiguration = _feature.GetCurrentConfiguration();
			_currentSpeedUnit = _feature.GetCurrentSpeedUnit();

			// Setup UI e validazione
			ValidateUIComponents();
			SetupInitialUI();
			SubscribeToEvents();
		}

		/// <summary>
		/// Avvio - Setup iniziale velocità
		/// </summary>
		protected override void ManagedStart()
		{
			Debug.Log("[SPEEDOMETER] ▶️ Speedometer UI avviata");

			// Imposta velocità iniziale
			_targetSpeed = _vehicleDataService.CurrentSpeed;
			_displayedSpeed = _targetSpeed;

			// Setup UI iniziale
			UpdateSpeedDisplay();
			UpdateProgressBar();
		}

		/// <summary>
		/// Update continuo - Animazioni smooth
		/// </summary>
		protected override void ManagedUpdate()
		{
			// Update smooth della velocità
			UpdateSmoothSpeed();

			// Update colori se necessario
			UpdateSpeedColors();

			// Update UI
			UpdateSpeedDisplay();
			UpdateProgressBar();

			// Performance tracking (optional)
			TrackPerformance();
		}

		/// <summary>
		/// Cleanup
		/// </summary>
		protected override void ManagedOnDestroy()
		{
			Debug.Log("[SPEEDOMETER] 🗑️ SpeedometerBehaviour distrutto");

			// Cleanup eventi
			UnsubscribeFromEvents();

			// Stop coroutines
			if (_speedAnimationCoroutine != null)
			{
				StopCoroutine(_speedAnimationCoroutine);
			}

			if (_colorTransitionCoroutine != null)
			{
				StopCoroutine(_colorTransitionCoroutine);
			}
		}

		#endregion

		#region Public Methods (Called by Feature)

		/// <summary>
		/// Applica configurazione speedometer
		/// Chiamato da SpeedometerFeature quando cambia modalità guida
		/// </summary>
		public void ApplyConfiguration(SpeedometerConfig config)
		{
			_currentConfiguration = config;

			// Aggiorna progress bar range
			if (_speedProgressBar != null)
			{
				_speedProgressBar.maxValue = config.MaxDisplaySpeed;
			}

			// Aggiorna responsiveness
			// (il damping verrà applicato in UpdateSmoothSpeed)

			Debug.Log($"[SPEEDOMETER] 🔧 Configurazione applicata: MaxSpeed={config.MaxDisplaySpeed}, Damping={config.ResponseDamping}");
		}

		/// <summary>
		/// Imposta unità di misura velocità
		/// Chiamato da SpeedometerFeature per cambio km/h ↔ mph
		/// </summary>
		public void SetSpeedUnit(SpeedometerData.SpeedUnit unit)
		{
			_currentSpeedUnit = unit;

			// Aggiorna label unità
			if (_speedUnitText != null)
			{
				_speedUnitText.text = SpeedometerData.GetUnitLabel(unit);
			}

			// Force update display con nuova unità
			UpdateSpeedDisplay();

			Debug.Log($"[SPEEDOMETER] 📏 Unità velocità aggiornata: {unit}");
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
			if (_speedValueText == null)
			{
				Debug.LogError("[SPEEDOMETER] ❌ _speedValueText non assegnato nel prefab!");
				missingComponents++;
			}

			if (_speedUnitText == null)
			{
				Debug.LogError("[SPEEDOMETER] ❌ _speedUnitText non assegnato nel prefab!");
				missingComponents++;
			}

			if (_speedProgressBar == null)
			{
				Debug.LogError("[SPEEDOMETER] ❌ _speedProgressBar non assegnato nel prefab!");
				missingComponents++;
			}

			// Componenti opzionali (warning non error)
			if (_progressBarFill == null)
			{
				Debug.LogWarning("[SPEEDOMETER] ⚠️ _progressBarFill non assegnato (cambio colore limitato)");
			}

			if (_speedometerLabel == null)
			{
				Debug.LogWarning("[SPEEDOMETER] ⚠️ _speedometerLabel non assegnato (opzionale)");
			}

			// Risultato validazione
			if (missingComponents == 0)
			{
				Debug.Log("[SPEEDOMETER] ✅ Tutti i componenti essenziali assegnati");
			}
			else
			{
				Debug.LogError($"[SPEEDOMETER] ❌ {missingComponents} componenti essenziali mancanti!");
			}
		}

		/// <summary>
		/// Setup iniziale dell'UI
		/// </summary>
		private void SetupInitialUI()
		{
			// Setup testo velocità
			if (_speedValueText != null)
			{
				_speedValueText.text = "0";
				_speedValueText.color = SpeedometerData.DEFAULT_SPEED_COLOR;
				// _speedValueText.fontSize = SpeedometerData.SPEED_TEXT_SIZE_LARGE;
			}

			// Setup testo unità
			if (_speedUnitText != null)
			{
				_speedUnitText.text = SpeedometerData.GetUnitLabel(_currentSpeedUnit);
				// _speedUnitText.fontSize = SpeedometerData.UNIT_TEXT_SIZE;
				_speedUnitText.color = SpeedometerData.DEFAULT_SPEED_COLOR;
			}

			// Setup progress bar
			if (_speedProgressBar != null)
			{
				_speedProgressBar.minValue = 0f;
				_speedProgressBar.maxValue = _currentConfiguration?.MaxDisplaySpeed ?? SpeedometerData.MAX_DISPLAY_SPEED;
				_speedProgressBar.value = 0f;
				_speedProgressBar.interactable = false;
			}

			// Setup colori progress bar
			if (_progressBarFill != null)
			{
				_progressBarFill.color = SpeedometerData.DEFAULT_SPEED_COLOR;
			}

			// Setup label opzionale
			if (_speedometerLabel != null && string.IsNullOrEmpty(_speedometerLabel.text))
			{
				_speedometerLabel.text = "SPEED";
			}

			Debug.Log("[SPEEDOMETER] ✅ UI iniziale configurata");
		}

		#endregion

		#region Event System

		/// <summary>
		/// Sottoscrizione agli eventi
		/// </summary>
		private void SubscribeToEvents()
		{
			// Sottoscrivi a eventi velocità dal VehicleDataService
			_vehicleDataService.OnSpeedChanged += OnSpeedChanged;

			// Sottoscrivi a eventi modalità per configurazione dinamica
			_broadcaster.Add<DriveModeChangedEvent>(OnDriveModeChanged);

			Debug.Log("[SPEEDOMETER] 📡 Eventi sottoscritti");
		}

		/// <summary>
		/// Rimozione sottoscrizioni eventi
		/// </summary>
		private void UnsubscribeFromEvents()
		{
			// Rimuovi eventi velocità
			if (_vehicleDataService != null)
			{
				_vehicleDataService.OnSpeedChanged -= OnSpeedChanged;
			}

			// Rimuovi eventi broadcaster
			if (_broadcaster != null)
			{
				_broadcaster.Remove<DriveModeChangedEvent>(OnDriveModeChanged);
			}

			Debug.Log("[SPEEDOMETER] 📡 Eventi rimossi");
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Gestisce cambio velocità dal VehicleDataService
		/// </summary>
		private void OnSpeedChanged(float newSpeed)
		{
			_targetSpeed = newSpeed;

			// Aggiorna colore target basato su velocità
			_targetSpeedColor = SpeedometerData.GetSpeedColor(newSpeed);
		}

		/// <summary>
		/// Gestisce cambio modalità guida
		/// </summary>
		private void OnDriveModeChanged(DriveModeChangedEvent e)
		{
			// La configurazione verrà aggiornata da SpeedometerFeature
			// Questo handler è per eventuali aggiornamenti UI specifici
			Debug.Log($"[SPEEDOMETER] 🔄 Modalità cambiata: {e.NewMode}");
		}

		#endregion

		#region Speed Animation & Updates

		/// <summary>
		/// Update smooth della velocità con damping configurabile
		/// </summary>
		private void UpdateSmoothSpeed()
		{
			if (Mathf.Approximately(_displayedSpeed, _targetSpeed))
				return;

			// Calcola damping basato su configurazione modalità
			float damping = _currentConfiguration?.ResponseDamping ?? SpeedometerData.COMFORT_RESPONSE_DAMPING;

			// Smooth lerp verso velocità target
			_displayedSpeed = Mathf.Lerp(_displayedSpeed, _targetSpeed, damping * Time.deltaTime * 10f);

			// Snap se molto vicini
			if (Mathf.Abs(_displayedSpeed - _targetSpeed) < 0.1f)
			{
				_displayedSpeed = _targetSpeed;
			}
		}

		/// <summary>
		/// Update smooth dei colori velocità
		/// </summary>
		private void UpdateSpeedColors()
		{
			if (_currentSpeedColor == _targetSpeedColor)
				return;

			// Transizione colore smooth
			_currentSpeedColor = Color.Lerp(_currentSpeedColor, _targetSpeedColor, Time.deltaTime * 5f);

			// Applica colore a UI elements
			if (_speedValueText != null)
				_speedValueText.color = _currentSpeedColor;

			if (_progressBarFill != null)
				_progressBarFill.color = _currentSpeedColor;
		}

		/// <summary>
		/// Aggiorna display velocità numerica
		/// </summary>
		private void UpdateSpeedDisplay()
		{
			if (_speedValueText == null) return;

			// Formatta velocità per unità corrente
			string formattedSpeed = SpeedometerData.FormatSpeed(_displayedSpeed, _currentSpeedUnit);
			_speedValueText.text = formattedSpeed;
		}

		/// <summary>
		/// Aggiorna progress bar velocità
		/// </summary>
		private void UpdateProgressBar()
		{
			if (_speedProgressBar == null) return;

			// Converte velocità per display progress bar (sempre km/h internamente)
			float progressValue = _displayedSpeed;
			_speedProgressBar.value = Mathf.Clamp(progressValue, 0f, _speedProgressBar.maxValue);
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
					Debug.LogWarning($"[SPEEDOMETER] ⚠️ Performance warning: {fps:F1} FPS");
				}

				_lastUpdateTime = Time.time;
				_frameCounter = 0;
			}
		}

		#endregion

		#region Development Notes

		/*
         * SPEEDOMETER BEHAVIOUR - IMPLEMENTAZIONE COMPLETA
         * 
         * PATTERN SEGUITO:
         * - BaseMonoBehaviour<T>: Pattern Mercedes per UI components
         * - SerializeField validation: Controllo componenti prefab
         * - Event subscription: Risponde a OnSpeedChanged
         * - Smooth animations: Lerp per velocità e colori
         * 
         * FUNZIONALITÀ IMPLEMENTATE:
         * 1. Display velocità numerica con unità
         * 2. Progress bar grafica
         * 3. Cambio colori basato su velocità
         * 4. Smooth animations configurabili
         * 5. Support km/h ↔ mph
         * 6. Configurazione per modalità guida
         * 
         * UI COMPONENTS RICHIESTI NEL PREFAB:
         * - TextMeshProUGUI per velocità numerica
         * - TextMeshProUGUI per unità misura  
         * - Slider per progress bar
         * - Image per fill progress bar (cambio colori)
         * 
         * PROSSIMO STEP:
         * - Creare prefab SpeedometerPrefab
         * - Testare integration con VehicleDataService
         * - Verificare smooth animations
         */

		#endregion
	}
}