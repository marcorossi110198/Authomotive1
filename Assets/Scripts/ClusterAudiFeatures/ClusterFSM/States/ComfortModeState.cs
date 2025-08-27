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
		/// Gestisce le transizioni tra modalità + controllo velocità realistico
		/// NUOVO: Sistema W/Spazio per accelerazione/frenata progressive
		/// </summary>
		private void CheckModeTransitions()
		{
			// ===== CONTROLLO VELOCITÀ REALISTICO =====
			HandleRealisticSpeedControl();

			// ===== CAMBIO MODALITÀ (invariato) =====

			// F1: ECO Mode
			if (Input.GetKeyDown(KeyCode.F1))
			{
				Debug.Log("[ECO MODE] 🟢 F1 premuto - Transizione a Eco Mode"); // Aggiorna per ogni stato
				_context.ClusterStateMachine.GoTo("EcoModeState");
			}
			// F2: COMFORT Mode
			else if (Input.GetKeyDown(KeyCode.F2))
			{
				Debug.Log("[ECO MODE] 🔵 F2 premuto - Transizione a Comfort Mode"); // Aggiorna per ogni stato
				_context.ClusterStateMachine.GoTo("ComfortModeState");
			}
			// F3: SPORT Mode
			else if (Input.GetKeyDown(KeyCode.F3))
			{
				Debug.Log("[ECO MODE] 🔴 F3 premuto - Transizione a Sport Mode"); // Aggiorna per ogni stato
				_context.ClusterStateMachine.GoTo("SportModeState");
			}
			// F4: WELCOME Mode
			else if (Input.GetKeyDown(KeyCode.F4))
			{
				Debug.Log("[ECO MODE] 🎉 F4 premuto - Transizione a Welcome State"); // Aggiorna per ogni stato
				_context.ClusterStateMachine.GoTo("WelcomeState");
			}

			// ===== DEBUG CONTROLS =====

			// ESC: Info debug
			else if (Input.GetKeyDown(KeyCode.Escape))
			{
				Debug.Log("[ECO MODE] 📍 ESC premuto - Info stato corrente"); // Aggiorna per ogni stato
				LogCurrentModeInfo();
			}

			// F5: Debug
			else if (Input.GetKeyDown(KeyCode.F5))
			{
				Debug.Log("[ECO MODE] 🔧 DEBUG: StateOnUpdate funziona!"); // Aggiorna per ogni stato
			}

			// R: Random RPM (mantieni per completezza)
			else if (Input.GetKeyDown(KeyCode.R))
			{
				_vehicleDataService.SetRPM(Random.Range(800f, 6000f));
				Debug.Log("[RPM TEST] ⚙️ RPM casuali impostati");
			}
		}

		/// <summary>
		/// 🆕 NUOVO: Gestisce controllo velocità realistico con W/Spazio
		/// </summary>
		private void HandleRealisticSpeedControl()
		{
			float currentSpeed = _vehicleDataService.CurrentSpeed;
			float deltaTime = Time.deltaTime;

			// Parametri di accelerazione/decelerazione (personalizzabili per modalità)
			float accelerationRate = GetAccelerationRateForMode();    // km/h per secondo accelerando
			float brakingRate = GetBrakingRateForMode();             // km/h per secondo frenando
			float naturalDecelerationRate = GetNaturalDecelerationRateForMode(); // km/h per secondo rilascio

			float newSpeed = currentSpeed;

			// W = ACCELERAZIONE (tenuto premuto)
			if (Input.GetKey(KeyCode.W))
			{
				newSpeed += accelerationRate * deltaTime;
				newSpeed = Mathf.Clamp(newSpeed, 0f, 200f); // Max 200 km/h

				// Log solo quando inizia accelerazione
				if (Input.GetKeyDown(KeyCode.W))
				{
					Debug.Log("[SPEED CONTROL] 🚀 Accelerazione iniziata");
				}
			}
			// SPAZIO = FRENATA (tenuto premuto) 
			else if (Input.GetKey(KeyCode.Space))
			{
				newSpeed -= brakingRate * deltaTime;
				newSpeed = Mathf.Max(newSpeed, 0f); // Non andare sotto 0

				// Log solo quando inizia frenata
				if (Input.GetKeyDown(KeyCode.Space))
				{
					Debug.Log("[SPEED CONTROL] 🛑 Frenata iniziata");
				}
			}
			// NESSUN TASTO = DECELERAZIONE NATURALE
			else if (currentSpeed > 0f)
			{
				newSpeed -= naturalDecelerationRate * deltaTime;
				newSpeed = Mathf.Max(newSpeed, 0f); // Non andare sotto 0

				// Log decelerazione naturale ogni tanto (non spam)
				if (Time.frameCount % 120 == 0 && currentSpeed > 5f) // Ogni 2 secondi a 60fps
				{
					Debug.Log($"[SPEED CONTROL] 📉 Decelerazione naturale: {currentSpeed:F1} → {newSpeed:F1} km/h");
				}
			}

			// Aggiorna velocità solo se è cambiata significativamente
			if (Mathf.Abs(newSpeed - currentSpeed) > 0.1f)
			{
				_vehicleDataService.SetSpeed(newSpeed);
			}
		}

		/// <summary>
		/// 🆕 NUOVO: Ottiene tasso accelerazione basato su modalità guida
		/// </summary>
		private float GetAccelerationRateForMode()
		{
			return _vehicleDataService.CurrentDriveMode switch
			{
				DriveMode.Eco => 25f,      // km/h per secondo (lento, efficiente)
				DriveMode.Comfort => 35f,  // km/h per secondo (bilanciato)
				DriveMode.Sport => 50f,    // km/h per secondo (rapido, aggressivo)
				_ => 35f                   // Default comfort
			};
		}

		/// <summary>
		/// 🆕 NUOVO: Ottiene tasso frenata basato su modalità guida  
		/// </summary>
		private float GetBrakingRateForMode()
		{
			return _vehicleDataService.CurrentDriveMode switch
			{
				DriveMode.Eco => 40f,      // km/h per secondo (frenata graduale)
				DriveMode.Comfort => 60f,  // km/h per secondo (frenata normale)
				DriveMode.Sport => 80f,    // km/h per secondo (frenata sportiva)
				_ => 60f                   // Default comfort
			};
		}

		/// <summary>
		/// 🆕 NUOVO: Ottiene tasso decelerazione naturale basato su modalità
		/// </summary>
		private float GetNaturalDecelerationRateForMode()
		{
			return _vehicleDataService.CurrentDriveMode switch
			{
				DriveMode.Eco => 8f,       // km/h per secondo (coast molto lungo)
				DriveMode.Comfort => 12f,  // km/h per secondo (coast normale)  
				DriveMode.Sport => 15f,    // km/h per secondo (engine brake sportivo)
				_ => 12f                   // Default comfort
			};
		}

		/// <summary>
		/// Log informazioni modalità corrente per debug - AGGIORNATO con nuovi controlli
		/// </summary>
		private void LogCurrentModeInfo()
		{
			Debug.Log("=== ECO MODE INFO ==="); // Cambia per ogni stato
			Debug.Log($"Current State: {_context.ClusterStateMachine.GetCurrentState()?.GetType().Name}");
			Debug.Log($"Vehicle Mode: {_vehicleDataService.CurrentDriveMode}");
			Debug.Log($"Speed: {_vehicleDataService.CurrentSpeed:F1} km/h");
			Debug.Log($"RPM: {_vehicleDataService.CurrentRPM:F0}");
			Debug.Log($"Gear: {_vehicleDataService.CurrentGear}");

			// Mostra parametri accelerazione per modalità corrente
			Debug.Log($"=== PARAMETRI ACCELERAZIONE {_vehicleDataService.CurrentDriveMode.ToString().ToUpper()} ===");
			Debug.Log($"Accelerazione: {GetAccelerationRateForMode()} km/h/s");
			Debug.Log($"Frenata: {GetBrakingRateForMode()} km/h/s");
			Debug.Log($"Decelerazione: {GetNaturalDecelerationRateForMode()} km/h/s");

			Debug.Log("=== CONTROLLI ===");
			Debug.Log("F1=Eco | F2=Comfort | F3=Sport | F4=Welcome");
			Debug.Log("W=Accelera (hold) | SPAZIO=Frena (hold) | Rilascia=Coast");
			Debug.Log("R=Random RPM | ESC=Info");
		}

		// ===== 🆕 CLASSE DATI PARAMETRI ACCELERAZIONE =====

		/// <summary>
		/// 🆕 NUOVO: Parametri accelerazione per diverse modalità guida
		/// </summary>
		public class AccelerationParameters
		{
			public float BaseAcceleration = 35f;      // Accelerazione base km/h/s
			public float MaxAcceleration = 55f;       // Accelerazione massima km/h/s
			public float MaxSpeed = 180f;             // Velocità massima km/h
			public float BrakingPower = 70f;          // Potenza frenata km/h/s
			public float NaturalDeceleration = 10f;   // Decelerazione naturale km/h/s
			public float AccelerationCurve = 0.85f;   // Fattore curva accelerazione
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