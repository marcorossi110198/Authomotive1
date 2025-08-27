using ClusterAudi;
using UnityEngine;
using ClusterAudiFeatures;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Stato modalità ECO - Ottimizza per efficienza energetica
	/// Priorità: Consumo, Range, Guida eco-friendly
	/// </summary>
	public class EcoModeState : ClusterBaseState
	{
		private IVehicleDataService _vehicleDataService;
		private IBroadcaster _broadcaster;

		// Configurazioni specifiche ECO
		private readonly Color ECO_THEME_COLOR = new Color(0.2f, 0.8f, 0.2f, 1f); // Verde eco
		private readonly float ECO_MAX_RPM_DISPLAY = 4000f; // Limita visualizzazione RPM
		private readonly float ECO_EFFICIENCY_THRESHOLD = 0.8f; // Soglia efficienza

		public EcoModeState(ClusterStateContext context) : base(context)
		{
			_vehicleDataService = context.Client.Services.Get<IVehicleDataService>();
			_broadcaster = context.Client.Services.Get<IBroadcaster>();

			// Sottoscrizione evento UI Drive Mode (NUOVO)
			// _broadcaster.Add<ClusterDriveModeStateTransitionRequest>(OnDriveModeUITransitionRequest);

		}

		public override void StateOnEnter()
		{
			Debug.Log("[ECO MODE] 🌱 Attivazione modalità Eco");

			// 1. Configura il servizio dati per modalità ECO
			_vehicleDataService.SetDriveMode(DriveMode.Eco);

			// 2. Applica tema visivo ECO
			ApplyEcoTheme();

			// 3. Configura priorità informazioni mostrate
			ConfigureEcoDisplayPriorities();

			// 4. Avvia monitoraggio efficienza
			StartEfficiencyMonitoring();

			// 5. Broadcast evento cambio modalità
			_broadcaster.Broadcast(new DriveModeChangedEvent(DriveMode.Eco));
		}

		public override void StateOnExit()
		{
			Debug.Log("[ECO MODE] 🌱 Disattivazione modalità Eco");

			// Cleanup specifico ECO
			StopEfficiencyMonitoring();

			// Reset configurazioni se necessario
			ResetEcoSettings();

			// Rimuovi sottoscrizione evento (NUOVO)
			//_broadcaster.Remove<ClusterDriveModeStateTransitionRequest>(OnDriveModeUITransitionRequest);

		}

		public override void StateOnUpdate()
		{
			// Update continuo per modalità ECO
			UpdateEcoMetrics();
			UpdateEfficiencyFeedback();

			// Controllo transizioni (gestite da debug keys per ora)
			CheckModeTransitions();
		}

		/// <summary>
		/// Applica il tema visivo per la modalità ECO
		/// </summary>
		private void ApplyEcoTheme()
		{
			var themeEvent = new ApplyThemeEvent
			{
				PrimaryColor = ECO_THEME_COLOR,
				SecondaryColor = new Color(0.1f, 0.6f, 0.1f, 1f),
				AccentColor = Color.white,
				ThemeName = "EcoMode"
			};

			_broadcaster.Broadcast(themeEvent);
		}

		// ✅ AGGIUNTA: Handler per richieste dall'UI Drive Mode (NUOVO)
		//private void OnDriveModeUITransitionRequest(ClusterDriveModeStateTransitionRequest request)
		//{
		//	Debug.Log($"[ECO MODE] 🎛️ UI Drive Mode richiede transizione: {request.TargetState}");

		//	// Valida stato richiesto
		//	if (ClusterDriveModeData.IsValidState(request.TargetState))
		//	{
		//		_context.ClusterStateMachine.GoTo(request.TargetState);
		//	}
		//	else
		//	{
		//		Debug.LogWarning($"[ECO MODE] ⚠️ Stato non valido richiesto da UI: {request.TargetState}");
		//	}
		//}

		/// <summary>
		/// Configura le priorità di visualizzazione per modalità ECO
		/// </summary>
		private void ConfigureEcoDisplayPriorities()
		{
			var displayConfig = new DisplayConfigEvent
			{
				ShowConsumption = true,        // Priorità alta: consumo
				ShowRange = true,              // Priorità alta: autonomia
				ShowEfficiencyTips = true,    // Mostra consigli eco
				ShowSportMetrics = false,     // Nascondi metriche sportive
				MaxRPMDisplay = ECO_MAX_RPM_DISPLAY,
				SpeedUnitPreference = SpeedUnit.KmH // Preferisci km/h per precisione
			};

			_broadcaster.Broadcast(displayConfig);
		}

		/// <summary>
		/// Avvia il monitoraggio dell'efficienza di guida
		/// </summary>
		private void StartEfficiencyMonitoring()
		{
			Debug.Log("[ECO MODE] 📊 Monitoraggio efficienza attivato");
		}

		private void StopEfficiencyMonitoring()
		{
			Debug.Log("[ECO MODE] 📊 Monitoraggio efficienza disattivato");
		}

		/// <summary>
		/// Update delle metriche ECO-specifiche
		/// </summary>
		private void UpdateEcoMetrics()
		{
			if (_vehicleDataService == null) return;

			float currentConsumption = _vehicleDataService.GetCurrentConsumption();
			float efficiency = CalculateEfficiency(currentConsumption);

			// Broadcast metriche ECO
			var ecoMetrics = new EcoMetricsUpdateEvent
			{
				CurrentConsumption = currentConsumption,
				Efficiency = efficiency,
				EstimatedRange = _vehicleDataService.GetEstimatedRange(),
				EcoScore = CalculateEcoScore(efficiency)
			};

			_broadcaster.Broadcast(ecoMetrics);
		}

		/// <summary>
		/// Fornisce feedback visivo sull'efficienza di guida
		/// </summary>
		private void UpdateEfficiencyFeedback()
		{
			float efficiency = _vehicleDataService.GetDrivingEfficiency();

			if (efficiency > ECO_EFFICIENCY_THRESHOLD)
			{
				// Guida efficiente - feedback positivo
				_broadcaster.Broadcast(new EfficiencyFeedbackEvent
				{
					FeedbackType = EfficiencyFeedbackType.Excellent,
					Message = "Guida ottimale! 🌱",
					Color = Color.green
				});
			}
			else
			{
				// Guida migliorabile - suggerimenti
				_broadcaster.Broadcast(new EfficiencyFeedbackEvent
				{
					FeedbackType = EfficiencyFeedbackType.CanImprove,
					Message = "Riduci accelerazione per maggiore efficienza",
					Color = Color.yellow
				});
			}
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

		/// <summary>
		/// Calcola l'efficienza di guida basata sul consumo
		/// </summary>
		private float CalculateEfficiency(float consumption)
		{
			// Logica di calcolo efficienza specifica per ECO mode
			float baseConsumption = 15f; // L/100km base
			return Mathf.Clamp01(baseConsumption / Mathf.Max(consumption, 0.1f));
		}

		/// <summary>
		/// Calcola uno score ECO basato su vari fattori
		/// </summary>
		private int CalculateEcoScore(float efficiency)
		{
			return Mathf.RoundToInt(efficiency * 100f);
		}

		/// <summary>
		/// Reset delle impostazioni ECO specifiche
		/// </summary>
		private void ResetEcoSettings()
		{
			Debug.Log("[ECO MODE] Reset impostazioni ECO");
		}
	}

	#region Events Specifici ECO Mode

	/// <summary>
	/// Evento per aggiornamento metriche ECO
	/// </summary>
	public class EcoMetricsUpdateEvent
	{
		public float CurrentConsumption { get; set; }
		public float Efficiency { get; set; }
		public float EstimatedRange { get; set; }
		public int EcoScore { get; set; }
	}

	/// <summary>
	/// Evento per feedback efficienza
	/// </summary>
	public class EfficiencyFeedbackEvent
	{
		public EfficiencyFeedbackType FeedbackType { get; set; }
		public string Message { get; set; }
		public Color Color { get; set; }
	}

	public enum EfficiencyFeedbackType
	{
		Excellent,
		Good,
		CanImprove,
		Poor
	}

	/// <summary>
	/// Evento per configurazione display
	/// </summary>
	public class DisplayConfigEvent
	{
		// Proprietà base (già esistenti)
		public bool ShowConsumption { get; set; }
		public bool ShowRange { get; set; }
		public bool ShowEfficiencyTips { get; set; }
		public bool ShowSportMetrics { get; set; }
		public float MaxRPMDisplay { get; set; }
		public SpeedUnit SpeedUnitPreference { get; set; }

		// Proprietà aggiuntive per ComfortMode
		public bool SmoothingEnabled { get; set; } = false;
		public float AnimationSpeed { get; set; } = 1.0f;

		// Proprietà aggiuntive per SportMode
		public bool ShowAdvancedMetrics { get; set; } = false;
		public bool HighContrastMode { get; set; } = false;
	}

	public enum SpeedUnit
	{
		KmH,
		Mph
	}

	/// <summary>
	/// Evento cambio modalità guida
	/// </summary>
	public class DriveModeChangedEvent
	{
		public DriveMode NewMode { get; }

		public DriveModeChangedEvent(DriveMode newMode)
		{
			NewMode = newMode;
		}
	}

	/// <summary>
	/// Evento per applicazione tema
	/// </summary>
	public class ApplyThemeEvent
	{
		public Color PrimaryColor { get; set; }
		public Color SecondaryColor { get; set; }
		public Color AccentColor { get; set; }
		public string ThemeName { get; set; }
	}

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

	#endregion
}