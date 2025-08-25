using ClusterAudi;
using UnityEngine;

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
		/// Controlla se ci sono richieste di cambio modalità
		/// </summary>
		private void CheckModeTransitions()
		{
			// Debug keys per testing
			if (Input.GetKeyDown(KeyCode.F1)) // Debug key per Eco
			{
				_context.ClusterStateMachine.GoTo("EcoModeState");
				Debug.Log("[SPORT MODE] Richiesta transizione a Eco Mode (placeholder)");
			}
			else if (Input.GetKeyDown(KeyCode.F2)) // Debug key per Comfort
			{
				_context.ClusterStateMachine.GoTo("ComfortModeState");
				Debug.Log("[SPORT MODE] Richiesta transizione a Comfort Mode (placeholder)");
			}
			else if (Input.GetKeyDown(KeyCode.F4)) // Debug key per Welcome
			{
				_context.ClusterStateMachine.GoTo("WelcomeState");
				Debug.Log("[SPORT MODE] Richiesta transizione a Welcome State (placeholder)");
			}
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

	#endregion
}