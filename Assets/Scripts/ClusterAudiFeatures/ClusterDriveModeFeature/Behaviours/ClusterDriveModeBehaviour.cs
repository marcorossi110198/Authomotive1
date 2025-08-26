/// <summary>
/// CLUSTER DRIVE MODE BEHAVIOUR - VERSIONE SENZA BOTTONI UI
/// 
/// FILE: Behaviours/ClusterDriveModeBehaviour.cs
/// 
/// MonoBehaviour per Cluster Drive Mode UI - SOLO DISPLAY
/// IDENTICO al pattern Mercedes ma senza interazione bottoni
/// Controllo solo tramite F1-F3 dagli stati FSM
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
	/// MonoBehaviour per Cluster Drive Mode UI
	/// VERSIONE DISPLAY-ONLY - Senza interazione bottoni
	/// </summary>
	public class ClusterDriveModeBehaviour : BaseMonoBehaviour<IClusterDriveModeFeatureInternal>
	{
		#region Serialized Fields - UI COMPONENTS (Display Only)

		[Header("Mode Indicator Images - ASSIGN IN PREFAB")]
		[Tooltip("Indicatore visuale ECO - Solo display")]
		[SerializeField] private Image _ecoModeIndicator;

		[Tooltip("Indicatore visuale COMFORT - Solo display")]
		[SerializeField] private Image _comfortModeIndicator;

		[Tooltip("Indicatore visuale SPORT - Solo display")]
		[SerializeField] private Image _sportModeIndicator;

		[Header("Mode Text Labels - ASSIGN IN PREFAB")]
		[Tooltip("Testo ECO - Solo display")]
		[SerializeField] private TextMeshProUGUI _ecoModeText;

		[Tooltip("Testo COMFORT - Solo display")]
		[SerializeField] private TextMeshProUGUI _comfortModeText;

		[Tooltip("Testo SPORT - Solo display")]
		[SerializeField] private TextMeshProUGUI _sportModeText;

		[Header("Vehicle Info Display - ASSIGN IN PREFAB")]
		[Tooltip("Display velocità")]
		[SerializeField] private TextMeshProUGUI _speedText;

		[Tooltip("Display RPM")]
		[SerializeField] private TextMeshProUGUI _rpmText;

		[Tooltip("Display marcia")]
		[SerializeField] private TextMeshProUGUI _gearText;

		[Tooltip("Display modalità corrente")]
		[SerializeField] private TextMeshProUGUI _currentModeText;

		#endregion

		#region Private Fields

		// Servizi (ottenuti dal Client via feature)
		private IBroadcaster _broadcaster;
		private IVehicleDataService _vehicleDataService;

		// Stato corrente
		private DriveMode _currentDriveMode = DriveMode.Comfort;

		// Animazioni
		private Coroutine _modeTransitionCoroutine;

		#endregion

		#region BaseMonoBehaviour Override

		/// <summary>
		/// Inizializzazione
		/// </summary>
		protected override void ManagedAwake()
		{
			Debug.Log("[CLUSTER DRIVE MODE] 🎛️ ClusterDriveModeBehaviour inizializzato (Display Only)");

			// Ottieni servizi dal Client via feature
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();
			_vehicleDataService = client.Services.Get<IVehicleDataService>();

			// Setup iniziale (senza bottoni)
			ValidateUIComponents();
			SubscribeToEvents();
			SetupInitialUI();
		}

		/// <summary>
		/// Avvio - CORRETTO per inizializzazione modalità
		/// </summary>
		protected override void ManagedStart()
		{
			Debug.Log("[CLUSTER DRIVE MODE] ▶️ Cluster Drive Mode UI avviata (Display Only)");

			// Forza aggiornamento iniziale con modalità corrente del servizio
			var initialMode = _vehicleDataService.CurrentDriveMode;
			Debug.Log($"[CLUSTER DRIVE MODE] 🔧 Modalità iniziale dal servizio: {initialMode}");

			// Reset _currentDriveMode per forzare l'aggiornamento
			_currentDriveMode = (DriveMode)(-1);

			// Aggiorna UI con modalità corrente
			UpdateModeDisplay(initialMode);

			// Applica immediatamente il tema per la modalità iniziale
			ApplyModeTheme(initialMode);
		}

		/// <summary>
		/// Update continuo
		/// </summary>
		protected override void ManagedUpdate()
		{
			// Update continuo delle informazioni veicolo
			UpdateVehicleInfo();
		}

		/// <summary>
		/// Cleanup
		/// </summary>
		protected override void ManagedOnDestroy()
		{
			Debug.Log("[CLUSTER DRIVE MODE] 🗑️ Cluster Drive Mode UI distrutta");

			// Cleanup eventi
			UnsubscribeFromEvents();

			// Stop animazioni
			if (_modeTransitionCoroutine != null)
			{
				StopCoroutine(_modeTransitionCoroutine);
			}
		}

		#endregion

		#region UI Setup Methods

		/// <summary>
		/// Valida che tutti i SerializeField siano assegnati nel prefab
		/// VERSIONE SENZA BOTTONI
		/// </summary>
		private void ValidateUIComponents()
		{
			int missingComponents = 0;

			// Validazione indicatori (solo immagini, no bottoni)
			if (_ecoModeIndicator == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _ecoModeIndicator non assegnato nel prefab!");
				missingComponents++;
			}
			if (_comfortModeIndicator == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _comfortModeIndicator non assegnato nel prefab!");
				missingComponents++;
			}
			if (_sportModeIndicator == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _sportModeIndicator non assegnato nel prefab!");
				missingComponents++;
			}

			// Validazione testi modalità
			if (_ecoModeText == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _ecoModeText non assegnato nel prefab!");
				missingComponents++;
			}
			if (_comfortModeText == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _comfortModeText non assegnato nel prefab!");
				missingComponents++;
			}
			if (_sportModeText == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _sportModeText non assegnato nel prefab!");
				missingComponents++;
			}

			// Validazione info veicolo
			if (_speedText == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _speedText non assegnato nel prefab!");
				missingComponents++;
			}
			if (_rpmText == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _rpmText non assegnato nel prefab!");
				missingComponents++;
			}
			if (_gearText == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _gearText non assegnato nel prefab!");
				missingComponents++;
			}
			if (_currentModeText == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _currentModeText non assegnato nel prefab!");
				missingComponents++;
			}

			// Risultato validazione
			if (missingComponents == 0)
			{
				Debug.Log("[CLUSTER DRIVE MODE] ✅ Tutti i SerializeField assegnati correttamente (Display Only)");
			}
			else
			{
				Debug.LogError($"[CLUSTER DRIVE MODE] ❌ {missingComponents} componenti mancanti! " +
							   "Configura il prefab ClusterDriveModePrefab");
			}
		}

		/// <summary>
		/// Setup UI iniziale (senza bottoni)
		/// </summary>
		private void SetupInitialUI()
		{
			// Configura colori iniziali
			SetInitialColors();

			// Configura testi modalità
			SetupModeTexts();

			Debug.Log("[CLUSTER DRIVE MODE] ✅ UI iniziale configurata (Display Only)");
		}

		/// <summary>
		/// Configura colori iniziali degli indicatori
		/// </summary>
		private void SetInitialColors()
		{
			Debug.Log("[CLUSTER DRIVE MODE] 🎨 Configurazione colori iniziali...");

			// Tutti gli indicatori iniziano inattivi
			if (_ecoModeIndicator != null)
			{
				_ecoModeIndicator.color = ClusterDriveModeData.INACTIVE_COLOR;
			}

			if (_comfortModeIndicator != null)
			{
				_comfortModeIndicator.color = ClusterDriveModeData.INACTIVE_COLOR;
			}

			if (_sportModeIndicator != null)
			{
				_sportModeIndicator.color = ClusterDriveModeData.INACTIVE_COLOR;
			}

			Debug.Log("[CLUSTER DRIVE MODE] ✅ Colori iniziali configurati");
		}

		/// <summary>
		/// Configura testi delle modalità
		/// </summary>
		private void SetupModeTexts()
		{
			// Configura testi se non già impostati nel prefab
			if (_ecoModeText != null && string.IsNullOrEmpty(_ecoModeText.text))
				_ecoModeText.text = ClusterDriveModeData.ECO_MODE_TEXT;

			if (_comfortModeText != null && string.IsNullOrEmpty(_comfortModeText.text))
				_comfortModeText.text = ClusterDriveModeData.COMFORT_MODE_TEXT;

			if (_sportModeText != null && string.IsNullOrEmpty(_sportModeText.text))
				_sportModeText.text = ClusterDriveModeData.SPORT_MODE_TEXT;

			// Colore testi
			Color textColor = ClusterDriveModeData.TEXT_COLOR;
			if (_ecoModeText != null) _ecoModeText.color = textColor;
			if (_comfortModeText != null) _comfortModeText.color = textColor;
			if (_sportModeText != null) _sportModeText.color = textColor;
		}

		#endregion

		#region Mode Display Methods

		/// <summary>
		/// Aggiorna display modalità con animazione
		/// CORRETTO per gestire correttamente l'inizializzazione
		/// </summary>
		private void UpdateModeDisplay(DriveMode activeMode)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] 🔄 Aggiornamento display: {_currentDriveMode} → {activeMode}");

			var previousMode = _currentDriveMode;
			_currentDriveMode = activeMode;

			// Stop animazione precedente
			if (_modeTransitionCoroutine != null)
			{
				StopCoroutine(_modeTransitionCoroutine);
			}

			// Avvia animazione transizione
			_modeTransitionCoroutine = StartCoroutine(AnimateModeTransition(activeMode));

			Debug.Log($"[CLUSTER DRIVE MODE] ✅ Display aggiornato da {previousMode} a {activeMode}");
		}

		/// <summary>
		/// Animazione transizione modalità con easing
		/// </summary>
		private IEnumerator AnimateModeTransition(DriveMode activeMode)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] 🎬 INIZIO animazione transizione verso: {activeMode}");

			float duration = ClusterDriveModeData.ANIMATION_DURATION;
			float elapsed = 0f;

			// Determina colori target
			Color ecoTarget = activeMode == DriveMode.Eco ? ClusterDriveModeData.ECO_COLOR : ClusterDriveModeData.INACTIVE_COLOR;
			Color comfortTarget = activeMode == DriveMode.Comfort ? ClusterDriveModeData.COMFORT_COLOR : ClusterDriveModeData.INACTIVE_COLOR;
			Color sportTarget = activeMode == DriveMode.Sport ? ClusterDriveModeData.SPORT_COLOR : ClusterDriveModeData.INACTIVE_COLOR;

			// Colori iniziali
			Color ecoStart = _ecoModeIndicator?.color ?? ClusterDriveModeData.INACTIVE_COLOR;
			Color comfortStart = _comfortModeIndicator?.color ?? ClusterDriveModeData.INACTIVE_COLOR;
			Color sportStart = _sportModeIndicator?.color ?? ClusterDriveModeData.INACTIVE_COLOR;

			// Animazione
			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

				// Aggiorna colori con interpolazione smooth
				if (_ecoModeIndicator != null)
					_ecoModeIndicator.color = Color.Lerp(ecoStart, ecoTarget, t);

				if (_comfortModeIndicator != null)
					_comfortModeIndicator.color = Color.Lerp(comfortStart, comfortTarget, t);

				if (_sportModeIndicator != null)
					_sportModeIndicator.color = Color.Lerp(sportStart, sportTarget, t);

				yield return null;
			}

			// Assicura colori finali
			if (_ecoModeIndicator != null) _ecoModeIndicator.color = ecoTarget;
			if (_comfortModeIndicator != null) _comfortModeIndicator.color = comfortTarget;
			if (_sportModeIndicator != null) _sportModeIndicator.color = sportTarget;

			Debug.Log($"[CLUSTER DRIVE MODE] ✅ Transizione completata per modalità: {activeMode}");

			// Aggiorna tema generale
			ApplyModeTheme(activeMode);

			_modeTransitionCoroutine = null;
		}

		#endregion

		#region Event System Methods

		/// <summary>
		/// Sottoscrizione agli eventi del sistema
		/// </summary>
		private void SubscribeToEvents()
		{
			Debug.Log("[CLUSTER DRIVE MODE] 📡 Sottoscrizione eventi...");

			// Eventi Drive Mode esistenti (dai tuoi stati FSM)
			_broadcaster.Add<DriveModeChangedEvent>(OnDriveModeChanged);
			_broadcaster.Add<ApplyThemeEvent>(OnThemeChanged);

			// Eventi display configuration esistenti (dai tuoi stati FSM)
			_broadcaster.Add<DisplayConfigEvent>(OnDisplayConfigChanged);

			// Eventi metriche specializzate (dai tuoi stati FSM)
			_broadcaster.Add<EcoMetricsUpdateEvent>(OnEcoMetricsUpdate);
			_broadcaster.Add<ComfortMetricsUpdateEvent>(OnComfortMetricsUpdate);
			_broadcaster.Add<SportMetricsUpdateEvent>(OnSportMetricsUpdate);

			Debug.Log("[CLUSTER DRIVE MODE] ✅ Eventi sottoscritti");
		}

		/// <summary>
		/// Rimozione sottoscrizioni eventi
		/// </summary>
		private void UnsubscribeFromEvents()
		{
			Debug.Log("[CLUSTER DRIVE MODE] 📡 Rimozione sottoscrizioni eventi...");

			// Rimuovi tutti gli eventi sottoscritti
			_broadcaster.Remove<DriveModeChangedEvent>(OnDriveModeChanged);
			_broadcaster.Remove<ApplyThemeEvent>(OnThemeChanged);
			_broadcaster.Remove<DisplayConfigEvent>(OnDisplayConfigChanged);
			_broadcaster.Remove<EcoMetricsUpdateEvent>(OnEcoMetricsUpdate);
			_broadcaster.Remove<ComfortMetricsUpdateEvent>(OnComfortMetricsUpdate);
			_broadcaster.Remove<SportMetricsUpdateEvent>(OnSportMetricsUpdate);

			Debug.Log("[CLUSTER DRIVE MODE] ✅ Sottoscrizioni rimosse");
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Gestisce cambio modalità di guida
		/// Aggiorna UI quando gli stati FSM cambiano modalità
		/// </summary>
		private void OnDriveModeChanged(DriveModeChangedEvent e)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] 📢 Modalità cambiata via FSM: {e.NewMode}");

			// Aggiorna display modalità
			UpdateModeDisplay(e.NewMode);
		}

		/// <summary>
		/// Gestisce cambio tema
		/// Applica i colori quando gli stati FSM cambiano tema
		/// </summary>
		private void OnThemeChanged(ApplyThemeEvent e)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] 🎨 Tema cambiato: {e.ThemeName}");

			// Applica tema personalizzato se fornito
			ApplyThemeColors(e);
		}

		/// <summary>
		/// Gestisce configurazione display
		/// Adatta UI alle impostazioni degli stati FSM
		/// </summary>
		private void OnDisplayConfigChanged(DisplayConfigEvent e)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] ⚙️ Display config aggiornata");

			// Applica configurazione display
			ApplyDisplayConfiguration(e);
		}

		/// <summary>
		/// Gestisce metriche ECO - Senza log continui
		/// </summary>
		private void OnEcoMetricsUpdate(EcoMetricsUpdateEvent e)
		{
			if (_currentDriveMode == DriveMode.Eco)
			{
				UpdateEcoSpecificDisplay(e);
			}
		}

		/// <summary>
		/// Gestisce metriche COMFORT - Senza log continui
		/// </summary>
		private void OnComfortMetricsUpdate(ComfortMetricsUpdateEvent e)
		{
			if (_currentDriveMode == DriveMode.Comfort)
			{
				UpdateComfortSpecificDisplay(e);
			}
		}

		/// <summary>
		/// Gestisce metriche SPORT - Senza log continui
		/// </summary>
		private void OnSportMetricsUpdate(SportMetricsUpdateEvent e)
		{
			if (_currentDriveMode == DriveMode.Sport)
			{
				UpdateSportSpecificDisplay(e);
			}
		}

		#endregion

		#region UI Update Methods

		/// <summary>
		/// Aggiorna informazioni veicolo in tempo reale
		/// </summary>
		private void UpdateVehicleInfo()
		{
			if (_vehicleDataService == null) return;

			// Aggiorna display velocità
			if (_speedText != null)
				_speedText.text = $"{_vehicleDataService.CurrentSpeed:F0} km/h";

			// Aggiorna display RPM
			if (_rpmText != null)
				_rpmText.text = $"{_vehicleDataService.CurrentRPM:F0} RPM";

			// Aggiorna display marcia
			if (_gearText != null)
			{
				string gearDisplay = _vehicleDataService.CurrentGear switch
				{
					-1 => "R",    // Retromarcia
					0 => "P",     // Parcheggio
					_ => _vehicleDataService.CurrentGear.ToString()
				};
				_gearText.text = gearDisplay;
			}

			// Aggiorna modalità corrente
			if (_currentModeText != null)
				_currentModeText.text = _currentDriveMode.ToString().ToUpper();
		}

		/// <summary>
		/// Applica tema per modalità corrente
		/// </summary>
		private void ApplyModeTheme(DriveMode mode)
		{
			Color primaryColor = mode switch
			{
				DriveMode.Eco => ClusterDriveModeData.ECO_COLOR,
				DriveMode.Comfort => ClusterDriveModeData.COMFORT_COLOR,
				DriveMode.Sport => ClusterDriveModeData.SPORT_COLOR,
				_ => ClusterDriveModeData.COMFORT_COLOR
			};

			// Applica colore primario ai testi delle info veicolo
			if (_speedText != null) _speedText.color = primaryColor;
			if (_rpmText != null) _rpmText.color = primaryColor;
			if (_gearText != null) _gearText.color = primaryColor;
			if (_currentModeText != null)
			{
				_currentModeText.color = primaryColor;
				_currentModeText.text = mode.ToString().ToUpper();
			}

			Debug.Log($"[CLUSTER DRIVE MODE] ✅ Tema applicato per modalità: {mode}");
		}

		/// <summary>
		/// Applica colori da evento tema personalizzato
		/// </summary>
		private void ApplyThemeColors(ApplyThemeEvent themeEvent)
		{
			// Applica tema personalizzato dagli stati FSM
			if (_currentModeText != null)
				_currentModeText.color = themeEvent.PrimaryColor;

			// Applica colore secondario se disponibile
			if (themeEvent.SecondaryColor != Color.clear)
			{
				if (_speedText != null) _speedText.color = themeEvent.SecondaryColor;
				if (_rpmText != null) _rpmText.color = themeEvent.SecondaryColor;
				if (_gearText != null) _gearText.color = themeEvent.SecondaryColor;
			}
		}

		/// <summary>
		/// Applica configurazione display dagli stati FSM
		/// </summary>
		private void ApplyDisplayConfiguration(DisplayConfigEvent config)
		{
			// Configura visibility e stili basati su configurazione modalità

			// Modalità ECO: Mostra info consumo
			if (_currentDriveMode == DriveMode.Eco && config.ShowConsumption)
			{
				ShowConsumptionInfo(true);
			}
			else if (!config.ShowConsumption)
			{
				ShowConsumptionInfo(false);
			}

			// Modalità SPORT: Mostra metriche avanzate
			if (_currentDriveMode == DriveMode.Sport && config.ShowSportMetrics)
			{
				ShowAdvancedSportMetrics(true);
			}
			else if (!config.ShowSportMetrics)
			{
				ShowAdvancedSportMetrics(false);
			}

			// Applica smoothing per modalità COMFORT
			if (_currentDriveMode == DriveMode.Comfort && config.SmoothingEnabled)
			{
				EnableSmoothDataDisplay(true);
			}
			else
			{
				EnableSmoothDataDisplay(false);
			}
		}

		#endregion

		#region Mode Specific Display Methods

		/// <summary>
		/// Aggiorna display specifico ECO - Senza log continui
		/// </summary>
		private void UpdateEcoSpecificDisplay(EcoMetricsUpdateEvent ecoMetrics)
		{
			if (_currentModeText != null)
			{
				_currentModeText.text = $"ECO - {ecoMetrics.EcoScore}% EFF";
			}
		}

		/// <summary>
		/// Aggiorna display specifico COMFORT - Senza log continui
		/// </summary>
		private void UpdateComfortSpecificDisplay(ComfortMetricsUpdateEvent comfortMetrics)
		{
			if (_currentModeText != null)
			{
				_currentModeText.text = $"COMFORT - {comfortMetrics.ComfortScore:F1}";
			}
		}

		/// <summary>
		/// Aggiorna display specifico SPORT - Senza log continui
		/// </summary>
		private void UpdateSportSpecificDisplay(SportMetricsUpdateEvent sportMetrics)
		{
			if (_currentModeText != null)
			{
				_currentModeText.text = $"SPORT - {sportMetrics.PerformanceScore:F0}%";
			}
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Mostra/nasconde informazioni consumo per modalità ECO
		/// </summary>
		private void ShowConsumptionInfo(bool show)
		{
			// TODO: Implementare quando aggiungeremo UI consumo specifica
		}

		/// <summary>
		/// Mostra/nasconde metriche avanzate per modalità SPORT
		/// </summary>
		private void ShowAdvancedSportMetrics(bool show)
		{
			// TODO: Implementare quando aggiungeremo UI sport metrics specifica
		}

		/// <summary>
		/// Abilita/disabilita smoothing dati per modalità COMFORT
		/// </summary>
		private void EnableSmoothDataDisplay(bool enable)
		{
			// TODO: Implementare smoothing dei dati quando necessario
		}

		#endregion
	}
}