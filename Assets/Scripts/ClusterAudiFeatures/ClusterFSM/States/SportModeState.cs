using ClusterAudi;
using UnityEngine;
using ClusterAudiFeatures;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Stato modalità SPORT - Massimizza le performance e il feedback dinamico
	/// Priorità: Performance metrics, responsività, feedback immediato, dati avanzati
	/// </summary>
	public class SportModeState : ClusterBaseState
	{
		private IVehicleDataService _vehicleDataService;
		private IBroadcaster _broadcaster;

		// Configurazioni specifiche SPORT
		private readonly Color SPORT_THEME_COLOR = new Color(0.9f, 0.1f, 0.1f, 1f); // Rosso sport
		private readonly float SPORT_MAX_RPM_DISPLAY = 8000f; // RPM complete
		private readonly float SPORT_UPDATE_FREQUENCY = 60f; // Update più frequenti
		private readonly float SPORT_RESPONSE_FACTOR = 0.95f; // Risposta immediata

		// Performance tracking
		private float _sessionMaxSpeed = 0f;
		private float _sessionMaxRPM = 0f;
		private float _lapTimer = 0f;
		private bool _isLapTimerActive = false;

		public SportModeState(ClusterStateContext context) : base(context)
		{
			_vehicleDataService = context.Client.Services.Get<IVehicleDataService>();
			_broadcaster = context.Client.Services.Get<IBroadcaster>();

			// Sottoscrizione evento UI Drive Mode (NUOVO)
			// _broadcaster.Add<ClusterDriveModeStateTransitionRequest>(OnDriveModeUITransitionRequest);

		}

		public override void StateOnEnter()
		{
			Debug.Log("[SPORT MODE] 🏁 Attivazione modalità Sport - Performance Mode ON!");

			// 1. Configura servizio dati per modalità SPORT
			_vehicleDataService.SetDriveMode(DriveMode.Sport);

			// 2. Applica tema visivo aggressivo SPORT
			ApplySportTheme();

			// 3. Configura display performance-oriented
			ConfigurePerformanceDisplay();

			// 4. Attiva modalità high-frequency update
			StartHighFrequencyUpdates();

			// 5. Inizializza performance tracking
			InitializePerformanceTracking();

			// 6. Broadcast cambio modalità
			_broadcaster.Broadcast(new DriveModeChangedEvent(DriveMode.Sport));
		}

		public override void StateOnExit()
		{
			Debug.Log("[SPORT MODE] 🏁 Disattivazione modalità Sport");

			// Cleanup performance tracking
			StopPerformanceTracking();

			// Reset high-frequency updates
			StopHighFrequencyUpdates();

			// Reset impostazioni sport
			ResetSportSettings();

			// Rimuovi sottoscrizione evento (NUOVO)
			// _broadcaster.Remove<ClusterDriveModeStateTransitionRequest>(OnDriveModeUITransitionRequest);

		}

		public override void StateOnUpdate()
		{
			// Update ad alta frequenza per modalità SPORT
			UpdateSportMetrics();
			UpdatePerformanceTracking();
			UpdateLapTimer();

			// Controlli performance real-time
			CheckPerformanceThresholds();

			// Gestione transizioni
			CheckModeTransitions();
		}

		/// <summary>
		/// Applica tema visivo aggressivo sport - rossi dinamici e contrasti forti
		/// </summary>
		private void ApplySportTheme()
		{
			var themeEvent = new ApplyThemeEvent
			{
				PrimaryColor = SPORT_THEME_COLOR,
				SecondaryColor = new Color(0.7f, 0.0f, 0.0f, 1f), // Rosso scuro
				AccentColor = new Color(1f, 0.8f, 0f, 1f), // Giallo/arancio per highlights
				ThemeName = "SportMode"
			};

			_broadcaster.Broadcast(themeEvent);
		}

		// Handler per richieste dall'UI Drive Mode (NUOVO)
		//private void OnDriveModeUITransitionRequest(ClusterDriveModeStateTransitionRequest request)
		//{
		//	Debug.Log($"[SPORT MODE] 🎛️ UI Drive Mode richiede transizione: {request.TargetState}");

		//	// Valida stato richiesto
		//	if (ClusterDriveModeData.IsValidState(request.TargetState))
		//	{
		//		_context.ClusterStateMachine.GoTo(request.TargetState);
		//	}
		//	else
		//	{
		//		Debug.LogWarning($"[SPORT MODE] ⚠️ Stato non valido richiesto da UI: {request.TargetState}");
		//	}
		//}

		/// <summary>
		/// Configura display per massime performance - tutti i dati visibili
		/// </summary>
		private void ConfigurePerformanceDisplay()
		{
			var displayConfig = new DisplayConfigEvent
			{
				ShowConsumption = false,       // Non prioritario in sport mode
				ShowRange = false,             // Non prioritario in sport mode
				ShowEfficiencyTips = false,   // Controproducente in sport mode
				ShowSportMetrics = true,      // PRIORITÀ MASSIMA
				MaxRPMDisplay = SPORT_MAX_RPM_DISPLAY,
				SpeedUnitPreference = SpeedUnit.KmH,
				SmoothingEnabled = false,     // Risposta immediata, no smoothing
				AnimationSpeed = 2.0f,        // Animazioni più rapide
				ShowAdvancedMetrics = true,   // G-forces, lap times, etc.
				HighContrastMode = true       // Migliore leggibilità ad alta velocità
			};

			_broadcaster.Broadcast(displayConfig);
		}

		/// <summary>
		/// Attiva aggiornamenti ad alta frequenza per responsività
		/// </summary>
		private void StartHighFrequencyUpdates()
		{
			var frequencyConfig = new UpdateFrequencyConfigEvent
			{
				UpdateFrequency = SPORT_UPDATE_FREQUENCY,
				HighPrecisionMode = true,
				ResponseFactor = SPORT_RESPONSE_FACTOR
			};

			_broadcaster.Broadcast(frequencyConfig);
		}

		private void StopHighFrequencyUpdates()
		{
			var frequencyConfig = new UpdateFrequencyConfigEvent
			{
				UpdateFrequency = 30f, // Back to normal
				HighPrecisionMode = false,
				ResponseFactor = 0.5f
			};

			_broadcaster.Broadcast(frequencyConfig);
		}

		/// <summary>
		/// Inizializza il tracking delle performance per la sessione sport
		/// </summary>
		private void InitializePerformanceTracking()
		{
			_sessionMaxSpeed = 0f;
			_sessionMaxRPM = 0f;
			_lapTimer = 0f;
			_isLapTimerActive = false;

			Debug.Log("[SPORT MODE] 📊 Performance tracking inizializzato");
		}

		/// <summary>
		/// Update metriche sport - focus su performance e dinamica
		/// </summary>
		private void UpdateSportMetrics()
		{
			if (_vehicleDataService == null) return;

			float currentSpeed = _vehicleDataService.CurrentSpeed;
			float currentRPM = _vehicleDataService.CurrentRPM;

			// Track session records
			if (currentSpeed > _sessionMaxSpeed)
				_sessionMaxSpeed = currentSpeed;

			if (currentRPM > _sessionMaxRPM)
				_sessionMaxRPM = currentRPM;

			var sportMetrics = new SportMetricsUpdateEvent
			{
				CurrentSpeed = currentSpeed,
				CurrentRPM = currentRPM,
				SessionMaxSpeed = _sessionMaxSpeed,
				SessionMaxRPM = _sessionMaxRPM,
				PowerOutput = CalculatePowerOutput(currentRPM, currentSpeed),
				TorqueOutput = CalculateTorqueOutput(currentRPM),
				GForce = CalculateGForce(),
				LapTime = _lapTimer,
				IsLapActive = _isLapTimerActive,
				PerformanceScore = CalculatePerformanceScore(),
				ThrottlePosition = _vehicleDataService.GetThrottlePosition(),
				BrakeForce = _vehicleDataService.GetBrakeForce(),
				GearPosition = _vehicleDataService.CurrentGear
			};

			_broadcaster.Broadcast(sportMetrics);
		}

		/// <summary>
		/// Update del performance tracking in tempo reale
		/// </summary>
		private void UpdatePerformanceTracking()
		{
			// Track acceleration performance
			float currentAcceleration = _vehicleDataService.GetAcceleration();

			if (Mathf.Abs(currentAcceleration) > 0.5f) // Significant acceleration/deceleration
			{
				var accelerationEvent = new AccelerationEventData
				{
					Timestamp = Time.time,
					Acceleration = currentAcceleration,
					Speed = _vehicleDataService.CurrentSpeed,
					RPM = _vehicleDataService.CurrentRPM
				};

				_broadcaster.Broadcast(new PerformanceEventDetected(accelerationEvent));
			}
		}

		/// <summary>
		/// Update del lap timer se attivo
		/// </summary>
		private void UpdateLapTimer()
		{
			if (_isLapTimerActive)
			{
				_lapTimer += Time.deltaTime;

				// Broadcast update timer ogni secondo
				if (Mathf.FloorToInt(_lapTimer) != Mathf.FloorToInt(_lapTimer - Time.deltaTime))
				{
					_broadcaster.Broadcast(new LapTimerUpdateEvent
					{
						CurrentLapTime = _lapTimer,
						IsActive = _isLapTimerActive
					});
				}
			}
		}

		/// <summary>
		/// Controlla soglie performance per feedback immediato
		/// </summary>
		private void CheckPerformanceThresholds()
		{
			float currentRPM = _vehicleDataService.CurrentRPM;
			float maxRPM = _vehicleDataService.GetMaxRPM();

			// Red line warning
			if (currentRPM > maxRPM * 0.9f)
			{
				_broadcaster.Broadcast(new PerformanceWarningEvent
				{
					WarningType = PerformanceWarningType.RedLineApproaching,
					Severity = WarningsSeverity.High,
					Message = "RED LINE!"
				});
			}

			// Optimal shift point
			else if (currentRPM > maxRPM * 0.75f)
			{
				_broadcaster.Broadcast(new PerformanceWarningEvent
				{
					WarningType = PerformanceWarningType.OptimalShiftPoint,
					Severity = WarningsSeverity.Info,
					Message = "SHIFT UP"
				});
			}
		}

		/// <summary>
		/// Gestisce transizioni tra modalità - FIXED
		/// </summary>
		private void CheckModeTransitions()
		{
			// DEBUG: Log per verificare che Update venga chiamato
			if (Input.GetKeyDown(KeyCode.F5))
			{
				Debug.Log("[SPORT MODE] 🔧 DEBUG: StateOnUpdate funziona!");
			}

			// F1: ECO Mode
			if (Input.GetKeyDown(KeyCode.F1))
			{
				Debug.Log("[SPORT MODE] 🟢 F1 premuto - Transizione a Eco Mode");
				_context.ClusterStateMachine.GoTo("EcoModeState");
			}
			// F2: COMFORT Mode
			else if (Input.GetKeyDown(KeyCode.F2))
			{
				Debug.Log("[SPORT MODE] 🔵 F2 premuto - Transizione a Comfort Mode");
				_context.ClusterStateMachine.GoTo("ComfortModeState");
			}
			// F3: SPORT Mode (già attivo)
			else if (Input.GetKeyDown(KeyCode.F3))
			{
				Debug.Log("[SPORT MODE] 🔴 F3 premuto - Già in Sport Mode");
			}
			// F4: WELCOME Mode
			else if (Input.GetKeyDown(KeyCode.F4))
			{
				Debug.Log("[SPORT MODE] 🎉 F4 premuto - Transizione a Welcome State");
				_context.ClusterStateMachine.GoTo("WelcomeState");
			}

			// ESC: Info debug
			else if (Input.GetKeyDown(KeyCode.Escape))
			{
				Debug.Log("[SPORT MODE] 📍 ESC premuto - Info stato corrente");
				LogCurrentModeInfo();
			}

			// Sport-specific controls (mantieni quelli esistenti)
			if (Input.GetKeyDown(KeyCode.L)) // Lap timer toggle
			{
				ToggleLapTimer();
			}

			if (Input.GetKeyDown(KeyCode.R)) // Reset session records
			{
				ResetSessionRecords();
			}
		}

		private void LogCurrentModeInfo()
		{
			Debug.Log("=== SPORT MODE INFO ===");
			Debug.Log($"Current State: {_context.ClusterStateMachine.GetCurrentState()?.GetType().Name}");
			Debug.Log($"Vehicle Mode: {_vehicleDataService.CurrentDriveMode}");
			Debug.Log("F1=Eco | F2=Comfort | F3=Sport | F4=Welcome | L=LapTimer | R=Reset");
		}

		#region Performance Calculations

		private float CalculatePowerOutput(float rpm, float speed)
		{
			// Formula semplificata per calcolo potenza
			// In realtà molto più complessa e dipendente dal veicolo specifico
			return (rpm * speed) / 1000f; // kW approssimati
		}

		private float CalculateTorqueOutput(float rpm)
		{
			// Curva di coppia semplificata
			float optimalRPM = 4000f;
			float maxTorque = 400f; // Nm

			if (rpm < optimalRPM)
				return (rpm / optimalRPM) * maxTorque;
			else
				return maxTorque * (1f - ((rpm - optimalRPM) / (8000f - optimalRPM)) * 0.3f);
		}

		private float CalculateGForce()
		{
			// Calcolo G-force basato sull'accelerazione
			float acceleration = _vehicleDataService.GetAcceleration();
			return acceleration / 9.81f; // Converti in G
		}

		private float CalculatePerformanceScore()
		{
			// Score basato su utilizzo ottimale delle performance
			float rpmEfficiency = _vehicleDataService.GetRPMEfficiency();
			float accelerationControl = _vehicleDataService.GetAccelerationControl();
			float corneringPerformance = _vehicleDataService.GetCorneringPerformance();

			return (rpmEfficiency * 0.4f + accelerationControl * 0.35f + corneringPerformance * 0.25f) * 100f;
		}

		#endregion

		#region Sport Mode Controls

		private void ToggleLapTimer()
		{
			_isLapTimerActive = !_isLapTimerActive;

			if (_isLapTimerActive)
			{
				_lapTimer = 0f;
				Debug.Log("[SPORT MODE] 🏁 Lap timer started!");
			}
			else
			{
				Debug.Log($"[SPORT MODE] 🏁 Lap completed: {_lapTimer:F2}s");

				_broadcaster.Broadcast(new LapCompletedEvent
				{
					LapTime = _lapTimer,
					MaxSpeed = _sessionMaxSpeed,
					MaxRPM = _sessionMaxRPM
				});
			}
		}

		private void ResetSessionRecords()
		{
			_sessionMaxSpeed = 0f;
			_sessionMaxRPM = 0f;
			Debug.Log("[SPORT MODE] 📊 Session records reset");

			_broadcaster.Broadcast(new SessionRecordsResetEvent());
		}

		#endregion

		private void StopPerformanceTracking()
		{
			_isLapTimerActive = false;
			Debug.Log("[SPORT MODE] 📊 Performance tracking stopped");
		}

		private void ResetSportSettings()
		{
			Debug.Log("[SPORT MODE] Reset impostazioni sport");
		}
	}

	#region Sport Mode Specific Events

	public class SportMetricsUpdateEvent
	{
		public float CurrentSpeed { get; set; }
		public float CurrentRPM { get; set; }
		public float SessionMaxSpeed { get; set; }
		public float SessionMaxRPM { get; set; }
		public float PowerOutput { get; set; }
		public float TorqueOutput { get; set; }
		public float GForce { get; set; }
		public float LapTime { get; set; }
		public bool IsLapActive { get; set; }
		public float PerformanceScore { get; set; }
		public float ThrottlePosition { get; set; }
		public float BrakeForce { get; set; }
		public int GearPosition { get; set; }
	}

	public class UpdateFrequencyConfigEvent
	{
		public float UpdateFrequency { get; set; }
		public bool HighPrecisionMode { get; set; }
		public float ResponseFactor { get; set; }
	}

	public class PerformanceEventDetected
	{
		public AccelerationEventData EventData { get; }
		public PerformanceEventDetected(AccelerationEventData data) => EventData = data;
	}

	public class AccelerationEventData
	{
		public float Timestamp { get; set; }
		public float Acceleration { get; set; }
		public float Speed { get; set; }
		public float RPM { get; set; }
	}

	public class LapTimerUpdateEvent
	{
		public float CurrentLapTime { get; set; }
		public bool IsActive { get; set; }
	}

	public class LapCompletedEvent
	{
		public float LapTime { get; set; }
		public float MaxSpeed { get; set; }
		public float MaxRPM { get; set; }
	}

	public class PerformanceWarningEvent
	{
		public PerformanceWarningType WarningType { get; set; }
		public WarningsSeverity Severity { get; set; }
		public string Message { get; set; }
	}

	public enum PerformanceWarningType
	{
		RedLineApproaching,
		OptimalShiftPoint,
		OverheatingRisk,
		TractionLoss
	}

	public enum WarningsSeverity
	{
		Info,
		Warning,
		High,
		Critical
	}

	public class SessionRecordsResetEvent { }

	#endregion
}