using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Dati e costanti per SeatBelt Feature
	/// IDENTICO al pattern SpeedometerData seguendo modello Mercedes
	/// Centralizza TUTTE le configurazioni per cinture di sicurezza
	/// </summary>
	public static class SeatBeltData
	{
		#region Prefab Path

		/// <summary>
		/// Path del prefab principale per SeatBelt UI
		/// </summary>
		public const string SEATBELT_PREFAB_PATH = "SeatBelt/SeatBeltPrefab";

		#endregion

		#region Speed Thresholds per Drive Mode

		/// <summary>
		/// Soglie velocità per warning cinture - Configurabili per modalità guida
		/// </summary>
		public const float ECO_MODE_SPEED_THRESHOLD = 20f;      // km/h - Modalità Eco (più conservativa)
		public const float COMFORT_MODE_SPEED_THRESHOLD = 20f;   // km/h - Modalità Comfort (bilanciata)
		public const float SPORT_MODE_SPEED_THRESHOLD = 20f;     // km/h - Modalità Sport (più permissiva)

		/// <summary>
		/// Soglia di default (se modalità non riconosciuta)
		/// </summary>
		public const float DEFAULT_SPEED_THRESHOLD = 20f; // km/h

		#endregion

		#region SeatBelt States & Icons

		/// <summary>
		/// Enum per stato delle singole cinture
		/// </summary>
		public enum SeatBeltStatus
		{
			Fastened,    // Allacciata (verde/spenta)
			Unfastened,  // Slacciata (rosso)
			Warning,     // Warning attivo (lampeggiante)
			Unknown      // Stato sconosciuto (grigio)
		}

		/// <summary>
		/// Enum per posizioni cinture nel veicolo
		/// </summary>
		public enum SeatBeltPosition
		{
			Driver = 0,      // Guidatore (posizione 0)
			Passenger = 1,   // Passeggero anteriore (posizione 1)
			RearLeft = 2,    // Posteriore sinistro (posizione 2)
			RearRight = 3    // Posteriore destro (posizione 3)
		}

		/// <summary>
		/// Numero totale di cinture monitorate
		/// </summary>
		public const int TOTAL_SEATBELTS = 4;

		#endregion

		#region Warning Colors & Visual

		/// <summary>
		/// Colori per stati delle cinture
		/// </summary>
		public static readonly Color SEATBELT_FASTENED_COLOR = new Color(0.2f, 0.8f, 0.2f, 1f);   // Verde
		public static readonly Color SEATBELT_UNFASTENED_COLOR = new Color(0.9f, 0.1f, 0.1f, 1f); // Rosso
		public static readonly Color SEATBELT_WARNING_COLOR = new Color(1f, 0.5f, 0f, 1f);        // Arancione
		public static readonly Color SEATBELT_UNKNOWN_COLOR = new Color(0.5f, 0.5f, 0.5f, 1f);    // Grigio
		public static readonly Color SEATBELT_DISABLED_COLOR = new Color(0.3f, 0.3f, 0.3f, 0.5f); // Grigio trasparente

		/// <summary>
		/// Colore testo warning principale
		/// </summary>
		public static readonly Color WARNING_TEXT_COLOR = new Color(1f, 0.2f, 0.2f, 1f); // Rosso forte

		#endregion

		#region Audio Configuration

		/// <summary>
		/// Path audio files per warning cinture
		/// </summary>
		public const string SOFT_BEEP_AUDIO_PATH = AudioData.SEATBELT_SOFT_BEEP_PATH;
		public const string URGENT_BEEP_AUDIO_PATH = AudioData.SEATBELT_URGENT_BEEP_PATH;
		public const string CONTINUOUS_BEEP_AUDIO_PATH = AudioData.SEATBELT_CONTINUOUS_BEEP_PATH;

		/// <summary>
		/// Timing per escalation audio warning
		/// </summary>
		public const float SOFT_BEEP_INTERVAL = 3f;        // Ogni 3 secondi
		public const float URGENT_BEEP_INTERVAL = 1.5f;    // Ogni 1.5 secondi
		public const float CONTINUOUS_BEEP_INTERVAL = 0.5f; // Ogni 0.5 secondi

		/// <summary>
		/// Soglie tempo per escalation warning
		/// </summary>
		public const float SOFT_TO_URGENT_TIME = 10f;      // Dopo 10s -> urgent
		public const float URGENT_TO_CONTINUOUS_TIME = 20f; // Dopo 20s -> continuous

		#endregion

		#region Warning Animation Settings

		/// <summary>
		/// Impostazioni animazioni warning
		/// </summary>
		public const float WARNING_FLASH_DURATION = 0.3f;    // Durata singolo flash
		public const float WARNING_FLASH_INTERVAL = 0.6f;    // Intervallo tra flash
		public const float WARNING_ICON_SCALE_MIN = 0.9f;    // Scale minimo icone
		public const float WARNING_ICON_SCALE_MAX = 1.1f;    // Scale massimo icone
		public const float WARNING_PANEL_FADE_DURATION = 0.5f; // Durata fade panel

		#endregion

		#region UI Layout Configuration

		/// <summary>
		/// Configurazione layout UI per diverse modalità
		/// </summary>
		public const float UI_ICON_SIZE_SMALL = 32f;   // Icone piccole
		public const float UI_ICON_SIZE_MEDIUM = 48f;  // Icone medie  
		public const float UI_ICON_SIZE_LARGE = 64f;   // Icone grandi

		/// <summary>
		/// Posizioni UI per icone cinture (normalizzate 0-1)
		/// Layout automotive standard: Driver | Passenger
		///                            Rear-L | Rear-R
		/// </summary>
		public static readonly Vector2 DRIVER_ICON_POSITION = new Vector2(0.2f, 0.3f);
		public static readonly Vector2 PASSENGER_ICON_POSITION = new Vector2(0.8f, 0.3f);
		public static readonly Vector2 REAR_LEFT_ICON_POSITION = new Vector2(0.2f, 0.15f);
		public static readonly Vector2 REAR_RIGHT_ICON_POSITION = new Vector2(0.8f, 0.15f);

		#endregion

		#region Mode-Specific Configurations

		/// <summary>
		/// Configurazioni specifiche per modalità guida
		/// Pattern IDENTICO a SpeedometerData
		/// </summary>

		// ECO Mode - Focus su sicurezza, soglia velocità bassa
		public static readonly SeatBeltConfig ECO_CONFIG = new SeatBeltConfig
		{
			SpeedThreshold = ECO_MODE_SPEED_THRESHOLD,
			EnableAudioWarning = true,
			EnableVisualWarning = true,
			AudioEscalationEnabled = true,
			WarningPriority = WarningPriority.High,
			FlashingEnabled = true,
			ShowWarningText = true,
			AutoDismissWarning = false  // Più insistente in Eco
		};

		// COMFORT Mode - Bilanciato
		public static readonly SeatBeltConfig COMFORT_CONFIG = new SeatBeltConfig
		{
			SpeedThreshold = COMFORT_MODE_SPEED_THRESHOLD,
			EnableAudioWarning = true,
			EnableVisualWarning = true,
			AudioEscalationEnabled = true,
			WarningPriority = WarningPriority.Medium,
			FlashingEnabled = true,
			ShowWarningText = true,
			AutoDismissWarning = false
		};

		// SPORT Mode - Meno invasivo, soglia più alta
		public static readonly SeatBeltConfig SPORT_CONFIG = new SeatBeltConfig
		{
			SpeedThreshold = SPORT_MODE_SPEED_THRESHOLD,
			EnableAudioWarning = true,
			EnableVisualWarning = true,
			AudioEscalationEnabled = false, // Meno escalation in Sport
			WarningPriority = WarningPriority.Low,
			FlashingEnabled = false,        // Meno distrazioni
			ShowWarningText = true,
			AutoDismissWarning = true       // Si spegne da solo
		};

		#endregion

		#region Utility Methods

		/// <summary>
		/// Ottiene la soglia velocità per la modalità guida corrente
		/// </summary>
		public static float GetSpeedThresholdForMode(ClusterAudi.DriveMode mode)
		{
			return mode switch
			{
				ClusterAudi.DriveMode.Eco => ECO_MODE_SPEED_THRESHOLD,
				ClusterAudi.DriveMode.Comfort => COMFORT_MODE_SPEED_THRESHOLD,
				ClusterAudi.DriveMode.Sport => SPORT_MODE_SPEED_THRESHOLD,
				_ => DEFAULT_SPEED_THRESHOLD
			};
		}

		/// <summary>
		/// Ottiene la configurazione completa per modalità guida
		/// </summary>
		public static SeatBeltConfig GetConfigForDriveMode(ClusterAudi.DriveMode mode)
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
		/// Ottiene il colore per lo stato della cintura
		/// </summary>
		public static Color GetColorForStatus(SeatBeltStatus status)
		{
			return status switch
			{
				SeatBeltStatus.Fastened => SEATBELT_FASTENED_COLOR,
				SeatBeltStatus.Unfastened => SEATBELT_UNFASTENED_COLOR,
				SeatBeltStatus.Warning => SEATBELT_WARNING_COLOR,
				SeatBeltStatus.Unknown => SEATBELT_UNKNOWN_COLOR,
				_ => SEATBELT_DISABLED_COLOR
			};
		}

		/// <summary>
		/// Ottiene la posizione UI per una specifica cintura
		/// </summary>
		public static Vector2 GetUIPositionForSeatBelt(SeatBeltPosition position)
		{
			return position switch
			{
				SeatBeltPosition.Driver => DRIVER_ICON_POSITION,
				SeatBeltPosition.Passenger => PASSENGER_ICON_POSITION,
				SeatBeltPosition.RearLeft => REAR_LEFT_ICON_POSITION,
				SeatBeltPosition.RearRight => REAR_RIGHT_ICON_POSITION,
				_ => DRIVER_ICON_POSITION
			};
		}

		/// <summary>
		/// Calcola il livello di escalation audio basato sul tempo
		/// </summary>
		public static AudioEscalationLevel GetAudioEscalationLevel(float warningTime)
		{
			if (warningTime < SOFT_TO_URGENT_TIME)
				return AudioEscalationLevel.Soft;
			else if (warningTime < URGENT_TO_CONTINUOUS_TIME)
				return AudioEscalationLevel.Urgent;
			else
				return AudioEscalationLevel.Continuous;
		}

		#endregion

		#region Development Notes

		/*
         * SEATBELT DATA - CONFIGURAZIONE COMPLETA
         * 
         * Questo file centralizza TUTTE le configurazioni per SeatBeltFeature:
         * 
         * SPEED THRESHOLDS:
         * - Diverse soglie per Eco/Comfort/Sport
         * - Configurabili e modificabili facilmente
         * 
         * VISUAL CONFIGURATION:
         * - Colori stati cinture
         * - Posizioni UI automotive standard
         * - Animazioni warning
         * 
         * AUDIO ESCALATION:
         * - 3 livelli audio (soft ? urgent ? continuous)
         * - Timing configurabile per escalation
         * 
         * MODE CONFIGURATIONS:
         * - Comportamenti diversi per modalità guida
         * - Pattern identico a SpeedometerData
         * 
         * Seguendo esattamente il pattern Mercedes per consistenza architetturale.
         */

		#endregion
	}

	#region Configuration Classes

	/// <summary>
	/// Configurazione completa SeatBelt per modalità guida specifica
	/// </summary>
	[System.Serializable]
	public class SeatBeltConfig
	{
		public float SpeedThreshold;
		public bool EnableAudioWarning;
		public bool EnableVisualWarning;
		public bool AudioEscalationEnabled;
		public WarningPriority WarningPriority;
		public bool FlashingEnabled;
		public bool ShowWarningText;
		public bool AutoDismissWarning;
	}

	/// <summary>
	/// Priorità warning per sistema audio
	/// </summary>
	public enum WarningPriority
	{
		Low = 1,
		Medium = 2,
		High = 3,
		Critical = 4
	}

	/// <summary>
	/// Livelli escalation audio
	/// </summary>
	public enum AudioEscalationLevel
	{
		None,        // Nessun audio
		Soft,        // Beep delicato ogni 3s
		Urgent,      // Beep più frequente ogni 1.5s
		Continuous   // Beep continuo ogni 0.5s
	}

	#endregion
}