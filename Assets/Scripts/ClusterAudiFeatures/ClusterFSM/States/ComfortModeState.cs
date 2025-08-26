using ClusterAudi;
using UnityEngine;
using ClusterAudiFeatures;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Stato modalità COMFORT - Bilancia performance e comfort
	/// Priorità: Esperienza di guida bilanciata, informazioni complete ma non invasive
	/// </summary>
	public class ComfortModeState : ClusterBaseState
	{
		private IVehicleDataService _vehicleDataService;
		private IBroadcaster _broadcaster;

		// Configurazioni specifiche COMFORT
		private readonly Color COMFORT_THEME_COLOR = new Color(0.2f, 0.5f, 0.8f, 1f); // Blu comfort
		private readonly float COMFORT_MAX_RPM_DISPLAY = 6000f; // RPM moderate
		private readonly float COMFORT_SMOOTHING_FACTOR = 0.7f; // Smooth delle transizioni

		public ComfortModeState(ClusterStateContext context) : base(context)
		{
			_vehicleDataService = context.Client.Services.Get<IVehicleDataService>();
			_broadcaster = context.Client.Services.Get<IBroadcaster>();

			// Sottoscrizione evento UI Drive Mode (NUOVO)
			// _broadcaster.Add<ClusterDriveModeStateTransitionRequest>(OnDriveModeUITransitionRequest);

		}

		public override void StateOnEnter()
		{
			Debug.Log("[COMFORT MODE] 🛣️ Attivazione modalità Comfort");

			// 1. Configura servizio dati per modalità COMFORT
			_vehicleDataService.SetDriveMode(DriveMode.Comfort);

			// 2. Applica tema visivo COMFORT
			ApplyComfortTheme();

			// 3. Configura display bilanciato
			ConfigureBalancedDisplay();

			// 4. Avvia smoothing delle transizioni
			StartSmoothTransitions();

			// 5. Broadcast cambio modalità
			_broadcaster.Broadcast(new DriveModeChangedEvent(DriveMode.Comfort));
		}

		public override void StateOnExit()
		{
			Debug.Log("[COMFORT MODE] 🛣️ Disattivazione modalità Comfort");

			// Cleanup smoothing
			StopSmoothTransitions();

			// Reset impostazioni comfort
			ResetComfortSettings();

			// Rimuovi sottoscrizione evento (NUOVO)
			// _broadcaster.Remove<ClusterDriveModeStateTransitionRequest>(OnDriveModeUITransitionRequest);

		}

		public override void StateOnUpdate()
		{
			// Update continuo modalità COMFORT
			UpdateComfortMetrics();
			UpdateSmoothTransitions();

			// Gestione transizioni stati
			CheckModeTransitions();
		}

		/// <summary>
		/// Applica tema visivo comfort - colori rilassanti e bilanciati
		/// </summary>
		private void ApplyComfortTheme()
		{
			var themeEvent = new ApplyThemeEvent
			{
				PrimaryColor = COMFORT_THEME_COLOR,
				SecondaryColor = new Color(0.3f, 0.6f, 0.9f, 1f), // Blu più chiaro
				AccentColor = new Color(1f, 1f, 1f, 0.9f), // Bianco soft
				ThemeName = "ComfortMode"
			};

			_broadcaster.Broadcast(themeEvent);
		}

		// Handler per richieste dall'UI Drive Mode (NUOVO)
		//private void OnDriveModeUITransitionRequest(ClusterDriveModeStateTransitionRequest request)
		//{
		//	Debug.Log($"[COMFORT MODE] 🎛️ UI Drive Mode richiede transizione: {request.TargetState}");

		//	// Valida stato richiesto
		//	if (ClusterDriveModeData.IsValidState(request.TargetState))
		//	{
		//		_context.ClusterStateMachine.GoTo(request.TargetState);
		//	}
		//	else
		//	{
		//		Debug.LogWarning($"[COMFORT MODE] ⚠️ Stato non valido richiesto da UI: {request.TargetState}");
		//	}
		//}

		/// <summary>
		/// Configura display bilanciato - mostra tutto ma senza sovraccarico
		/// </summary>
		private void ConfigureBalancedDisplay()
		{
			var displayConfig = new DisplayConfigEvent
			{
				ShowConsumption = true,        // Informazione presente ma non prioritaria
				ShowRange = true,              // Utile per pianificazione
				ShowEfficiencyTips = false,   // Non necessari in modalità comfort
				ShowSportMetrics = true,      // Metriche moderate disponibili
				MaxRPMDisplay = COMFORT_MAX_RPM_DISPLAY,
				SpeedUnitPreference = SpeedUnit.KmH,
				SmoothingEnabled = true,      // Comfort = transizioni smooth
				AnimationSpeed = 1.0f         // Velocità normale animazioni
			};

			_broadcaster.Broadcast(displayConfig);
		}

		/// <summary>
		/// Avvia sistema di smoothing per transizioni più confortevoli
		/// </summary>
		private void StartSmoothTransitions()
		{
			var smoothingConfig = new SmoothingConfigEvent
			{
				Enabled = true,
				SmoothingFactor = COMFORT_SMOOTHING_FACTOR,
				ApplyToSpeed = true,
				ApplyToRPM = true,
				ApplyToConsumption = true
			};

			_broadcaster.Broadcast(smoothingConfig);
		}

		private void StopSmoothTransitions()
		{
			var smoothingConfig = new SmoothingConfigEvent
			{
				Enabled = false
			};

			_broadcaster.Broadcast(smoothingConfig);
		}

		/// <summary>
		/// Update metriche specifiche modalità comfort
		/// </summary>
		private void UpdateComfortMetrics()
		{
			if (_vehicleDataService == null) return;

			// Metriche bilanciate - né troppo aggressive né troppo conservative
			var comfortMetrics = new ComfortMetricsUpdateEvent
			{
				CurrentSpeed = _vehicleDataService.CurrentSpeed,
				CurrentRPM = _vehicleDataService.CurrentRPM,
				CurrentConsumption = _vehicleDataService.GetCurrentConsumption(),
				ComfortScore = CalculateComfortScore(),
				RideQuality = AssessRideQuality(),
				EstimatedRange = _vehicleDataService.GetEstimatedRange()
			};

			_broadcaster.Broadcast(comfortMetrics);
		}

		/// <summary>
		/// Update smooth delle transizioni per esperienza confortevole
		/// </summary>
		private void UpdateSmoothTransitions()
		{
			// Implementazione smoothing in tempo reale
			ApplyDataSmoothing();
		}

		/// <summary>
		/// Applica smoothing ai dati per ridurre oscillazioni brusche
		/// </summary>
		private void ApplyDataSmoothing()
		{
			// Esempio di smoothing semplice
			// In implementazione reale, userebbe buffer circolari e filtri

			float rawSpeed = _vehicleDataService.GetRawSpeed();
			float smoothedSpeed = Mathf.Lerp(
				_vehicleDataService.CurrentSpeed,
				rawSpeed,
				COMFORT_SMOOTHING_FACTOR * Time.deltaTime
			);

			// Note: In implementazione reale applicheremmo il valore smoothed
			// _vehicleDataService.SetSmoothedSpeed(smoothedSpeed);
		}

		/// <summary>
		/// Calcola score di comfort basato su vari fattori
		/// </summary>
		private float CalculateComfortScore()
		{
			// Fattori che influenzano il comfort:
			float accelerationSmoothness = _vehicleDataService.GetAccelerationSmoothness();
			float speedStability = _vehicleDataService.GetSpeedStability();
			float gearUsageOptimality = _vehicleDataService.GetGearUsageOptimality();

			// Weighted average per comfort score
			return (accelerationSmoothness * 0.4f + speedStability * 0.4f + gearUsageOptimality * 0.2f);
		}

		/// <summary>
		/// Valuta la qualità complessiva della guida
		/// </summary>
		private RideQuality AssessRideQuality()
		{
			float comfortScore = CalculateComfortScore();

			if (comfortScore > 0.8f) return RideQuality.Excellent;
			if (comfortScore > 0.6f) return RideQuality.Good;
			if (comfortScore > 0.4f) return RideQuality.Moderate;
			return RideQuality.NeedsImprovement;
		}

		/// <summary>
		/// Gestisce le transizioni tra modalità - VERSION FIXED
		/// </summary>
		private void CheckModeTransitions()
		{
			// DEBUG: Log per verificare che Update venga chiamato
			if (Input.GetKeyDown(KeyCode.F5))
			{
				Debug.Log("[COMFORT MODE] 🔧 DEBUG: StateOnUpdate funziona!");
			}

			// F1: ECO Mode
			if (Input.GetKeyDown(KeyCode.F1))
			{
				Debug.Log("[COMFORT MODE] 🟢 F1 premuto - Transizione a Eco Mode");
				_context.ClusterStateMachine.GoTo("EcoModeState");
			}
			// F2: COMFORT Mode (già attivo, per test)
			else if (Input.GetKeyDown(KeyCode.F2))
			{
				Debug.Log("[COMFORT MODE] 🔵 F2 premuto - Già in Comfort Mode");
			}
			// F3: SPORT Mode  
			else if (Input.GetKeyDown(KeyCode.F3))
			{
				Debug.Log("[COMFORT MODE] 🔴 F3 premuto - Transizione a Sport Mode");
				_context.ClusterStateMachine.GoTo("SportModeState");
			}
			// F4: WELCOME Mode
			else if (Input.GetKeyDown(KeyCode.F4))
			{
				Debug.Log("[COMFORT MODE] 🎉 F4 premuto - Transizione a Welcome State");
				_context.ClusterStateMachine.GoTo("WelcomeState");
			}

			// ESC: Info debug
			else if (Input.GetKeyDown(KeyCode.Escape))
			{
				Debug.Log("[COMFORT MODE] 📍 ESC premuto - Info stato corrente");
				LogCurrentModeInfo();
			}
		}

		/// <summary>
		/// Log informazioni modalità corrente per debug
		/// </summary>
		private void LogCurrentModeInfo()
		{
			Debug.Log("=== COMFORT MODE INFO ===");
			Debug.Log($"Current State: {_context.ClusterStateMachine.GetCurrentState()?.GetType().Name}");
			Debug.Log($"Vehicle Mode: {_vehicleDataService.CurrentDriveMode}");
			Debug.Log($"Speed: {_vehicleDataService.CurrentSpeed:F1} km/h");
			Debug.Log($"RPM: {_vehicleDataService.CurrentRPM:F0}");
			Debug.Log("F1=Eco | F2=Comfort | F3=Sport | F4=Welcome");
		}

		/// <summary>
		/// Reset delle impostazioni comfort
		/// </summary>
		private void ResetComfortSettings()
		{
			Debug.Log("[COMFORT MODE] Reset impostazioni comfort");
		}
	}

	#region Events Specifici Comfort Mode

	/// <summary>
	/// Evento per metriche comfort
	/// </summary>
	public class ComfortMetricsUpdateEvent
	{
		public float CurrentSpeed { get; set; }
		public float CurrentRPM { get; set; }
		public float CurrentConsumption { get; set; }
		public float ComfortScore { get; set; }
		public RideQuality RideQuality { get; set; }
		public float EstimatedRange { get; set; }
	}

	/// <summary>
	/// Configurazione smoothing per comfort
	/// </summary>
	public class SmoothingConfigEvent
	{
		public bool Enabled { get; set; }
		public float SmoothingFactor { get; set; } = 0.5f;
		public bool ApplyToSpeed { get; set; } = true;
		public bool ApplyToRPM { get; set; } = true;
		public bool ApplyToConsumption { get; set; } = false;
	}

	/// <summary>
	/// Enum per qualità della guida
	/// </summary>
	public enum RideQuality
	{
		Excellent,      // Guida molto confortevole
		Good,           // Guida confortevole
		Moderate,       // Guida accettabile
		NeedsImprovement // Guida da migliorare per comfort
	}

	#endregion
}