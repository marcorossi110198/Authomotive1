using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Dati e costanti per AutomaticGearbox Feature
	/// IDENTICO al pattern SpeedometerData seguendo modello Mercedes
	/// </summary>
	public static class AutomaticGearboxData
	{
		#region Prefab Path

		/// <summary>
		/// Path del prefab principale per AutomaticGearbox
		/// </summary>
		public const string AutomaticGearbox_PREFAB_PATH = "AutomaticGearbox/AutomaticGearboxPrefab";

		#endregion

		#region RPM Ranges & Colors

		/// <summary>
		/// Range RPM per diverse colorazioni
		/// </summary>
		public const float IDLE_RPM = 800f;                    // RPM minimo motore acceso
		public const float LOW_RPM_THRESHOLD = 1500f;          // RPM bassi - Verde/Blu
		public const float MEDIUM_RPM_THRESHOLD = 3000f;       // RPM medi - Bianco
		public const float HIGH_RPM_THRESHOLD = 4500f;         // RPM alti - Giallo
		public const float DANGER_RPM_THRESHOLD = 5500f;       // RPM pericolosi - Arancione
		public const float RED_ZONE_RPM_THRESHOLD = 6500f;     // RED ZONE - Rosso

		/// <summary>
		/// RPM massimo visualizzabile
		/// </summary>
		public const float MAX_DISPLAY_RPM = 8000f;

		/// <summary>
		/// Colori per i diversi range di RPM
		/// </summary>
		public static readonly Color IDLE_RPM_COLOR = new Color(0.3f, 0.3f, 0.8f, 1f);        // Blu idle
		public static readonly Color LOW_RPM_COLOR = new Color(0.2f, 0.8f, 0.2f, 1f);         // Verde
		public static readonly Color MEDIUM_RPM_COLOR = new Color(0.9f, 0.9f, 0.9f, 1f);      // Bianco
		public static readonly Color HIGH_RPM_COLOR = new Color(0.9f, 0.9f, 0.2f, 1f);        // Giallo
		public static readonly Color DANGER_RPM_COLOR = new Color(0.9f, 0.6f, 0.1f, 1f);      // Arancione
		public static readonly Color RED_ZONE_COLOR = new Color(0.9f, 0.1f, 0.1f, 1f);        // Rosso

		/// <summary>
		/// Colore di default per testo RPM
		/// </summary>
		public static readonly Color DEFAULT_RPM_COLOR = Color.white;

		#endregion

		#region Red Zone Settings

		/// <summary>
		/// Configurazioni red zone
		/// </summary>
		public const float RED_ZONE_WARNING_THRESHOLD = 6000f;  // Warning pre-red zone
		public const float RED_ZONE_FLASH_SPEED = 5f;           // Velocità flash in red zone
		public const float RED_ZONE_CRITICAL_THRESHOLD = 7000f; // Soglia critica

		/// <summary>
		/// Effetti red zone
		/// </summary>
		public static readonly Color RED_ZONE_FLASH_COLOR = new Color(1f, 0f, 0f, 0.8f);
		public static readonly Color RED_ZONE_WARNING_COLOR = new Color(1f, 0.5f, 0f, 1f);

		#endregion

		#region Animation Settings

		/// <summary>
		/// Impostazioni animazioni AutomaticGearbox
		/// </summary>
		public const float RPM_ANIMATION_SMOOTHING = 0.1f;      // Smoothing RPM changes
		public const float COLOR_TRANSITION_DURATION = 0.2f;    // Durata cambio colore
		public const float UPDATE_FREQUENCY = 60f;              // Update frequency Hz

		/// <summary>
		/// Velocità di risposta diverse per modalità guida
		/// </summary>
		public const float ECO_RESPONSE_DAMPING = 0.3f;    // Più smooth per eco
		public const float COMFORT_RESPONSE_DAMPING = 0.2f; // Bilanciato
		public const float SPORT_RESPONSE_DAMPING = 0.05f;  // Risposta immediata

		#endregion

		#region Display Settings

		/// <summary>
		/// Impostazioni display AutomaticGearbox
		/// </summary>
		public const int RPM_DECIMAL_PLACES = 0;                // Decimali da mostrare
		public const string RPM_FORMAT = "F0";                  // Format string RPM
		public const string RPM_UNIT_LABEL = "RPM";             // Label unità RPM

		/// <summary>
		/// Dimensioni e posizioni UI
		/// </summary>
		public const float RPM_TEXT_SIZE_LARGE = 48f;     // Font size numero RPM
		public const float UNIT_TEXT_SIZE = 24f;          // Font size unità
		public const float PROGRESS_BAR_HEIGHT = 8f;      // Altezza progress bar

		#endregion

		#region Gear Display Settings

		/// <summary>
		/// Configurazioni display marce
		/// </summary>
		public const string GEAR_PARK = "P";
		public const string GEAR_REVERSE = "R";
		public const string GEAR_NEUTRAL = "N";

		public static readonly Color GEAR_ACTIVE_COLOR = Color.white;
		public static readonly Color GEAR_INACTIVE_COLOR = new Color(0.5f, 0.5f, 0.5f, 1f);

		#endregion

		#region Mode-Specific Settings

		/// <summary>
		/// Configurazioni specifiche per modalità guida
		/// </summary>

		// ECO Mode - Focus sull'efficienza
		public static readonly AutomaticGearboxConfig ECO_CONFIG = new AutomaticGearboxConfig
		{
			MaxDisplayRPM = 5000f,             // Limite ridotto per eco
			ShowRedZone = false,               // Nascondi red zone per scoraggiare
			ShowShiftIndicator = true,         // Mostra indicazioni cambio
			ShowGearDisplay = true,            // Mostra marcia corrente
			ResponseDamping = ECO_RESPONSE_DAMPING,
			RedZoneFlashEnabled = false        // No flash in eco mode
		};

		// COMFORT Mode - Bilanciato
		public static readonly AutomaticGearboxConfig COMFORT_CONFIG = new AutomaticGearboxConfig
		{
			MaxDisplayRPM = MAX_DISPLAY_RPM,   // Range completo
			ShowRedZone = true,                // Red zone visibile
			ShowShiftIndicator = false,        // No indicazioni cambio automatico
			ShowGearDisplay = true,            // Mostra marcia corrente
			ResponseDamping = COMFORT_RESPONSE_DAMPING,
			RedZoneFlashEnabled = false        // No flash invasivo
		};

		// SPORT Mode - Performance focus
		public static readonly AutomaticGearboxConfig SPORT_CONFIG = new AutomaticGearboxConfig
		{
			MaxDisplayRPM = MAX_DISPLAY_RPM,   // Range completo
			ShowRedZone = true,                // Red zone ben visibile
			ShowShiftIndicator = true,         // Indicazioni performance
			ShowGearDisplay = true,            // Marcia sempre visibile
			ResponseDamping = SPORT_RESPONSE_DAMPING,  // Risposta immediata
			RedZoneFlashEnabled = true         // Flash aggressivo in red zone
		};

		#endregion

		#region Utility Methods

		/// <summary>
		/// Ottiene il colore appropriato per i RPM
		/// </summary>
		public static Color GetRPMColor(float rpm)
		{
			if (rpm >= RED_ZONE_RPM_THRESHOLD)
				return RED_ZONE_COLOR;
			else if (rpm >= DANGER_RPM_THRESHOLD)
				return DANGER_RPM_COLOR;
			else if (rpm >= HIGH_RPM_THRESHOLD)
				return HIGH_RPM_COLOR;
			else if (rpm >= MEDIUM_RPM_THRESHOLD)
				return MEDIUM_RPM_COLOR;
			else if (rpm >= LOW_RPM_THRESHOLD)
				return LOW_RPM_COLOR;
			else if (rpm >= IDLE_RPM)
				return IDLE_RPM_COLOR;
			else
				return DEFAULT_RPM_COLOR;
		}

		/// <summary>
		/// Ottiene configurazione per modalità guida
		/// </summary>
		public static AutomaticGearboxConfig GetConfigForDriveMode(ClusterAudi.DriveMode mode)
		{
			return mode switch
			{
				ClusterAudi.DriveMode.Eco => ECO_CONFIG,
				ClusterAudi.DriveMode.Comfort => COMFORT_CONFIG,
				ClusterAudi.DriveMode.Sport => SPORT_CONFIG,
				_ => COMFORT_CONFIG
			};
		}

		/// <summary>
		/// Formatta RPM per display
		/// </summary>
		public static string FormatRPM(float rpm)
		{
			return rpm.ToString(RPM_FORMAT);
		}

		/// <summary>
		/// Converte marcia in display string
		/// </summary>
		public static string FormatGear(int gear)
		{
			return gear switch
			{
				-1 => GEAR_REVERSE,
				0 => GEAR_PARK,
				_ => gear.ToString()
			};
		}

		/// <summary>
		/// Verifica se RPM è in red zone
		/// </summary>
		public static bool IsInRedZone(float rpm)
		{
			return rpm >= RED_ZONE_RPM_THRESHOLD;
		}

		/// <summary>
		/// Verifica se RPM è in zona warning (pre-red zone)
		/// </summary>
		public static bool IsInWarningZone(float rpm)
		{
			return rpm >= RED_ZONE_WARNING_THRESHOLD && rpm < RED_ZONE_RPM_THRESHOLD;
		}

		#endregion


		#region Development Notes

		/*
         * AutomaticGearbox DATA - CONFIGURAZIONE COMPLETA
         * 
         * Pattern seguito: IDENTICO a SpeedometerData
         * 
         * RANGE & COLORS:
         * - 6 fasce RPM con colori dedicati (Idle -> Red Zone)
         * - Red zone con warning progressivo
         * 
         * ANIMATIONS:
         * - Smoothing configurabile per modalità
         * - Flash effect per red zone
         * 
         * MODE CONFIGURATIONS:
         * - ECO: Limita RPM, nasconde red zone
         * - COMFORT: Bilanciato, red zone visibile
         * - SPORT: Range completo, effetti aggressivi
         * 
         * AUTOMATIC TRANSMISSION:
         * - Integration con VehicleDataService per cambio automatico
         * - Display marce dinamico
         * - Shift indicators
         */

		#endregion
	}

	/// <summary>
	/// Configurazione specifica per AutomaticGearbox in base alla modalità guida
	/// </summary>
	[System.Serializable]
	public class AutomaticGearboxConfig
	{
		public float MaxDisplayRPM;
		public bool ShowRedZone;
		public bool ShowShiftIndicator;
		public bool ShowGearDisplay;
		public float ResponseDamping;
		public bool RedZoneFlashEnabled;
	}
}