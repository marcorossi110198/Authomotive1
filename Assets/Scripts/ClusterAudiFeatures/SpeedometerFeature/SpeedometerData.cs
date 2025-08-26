using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Dati e costanti per Speedometer Feature
	/// IDENTICO al pattern ClusterDriveModeData seguendo modello Mercedes
	/// </summary>
	public static class SpeedometerData
	{
		#region Prefab Path

		/// <summary>
		/// Path del prefab principale per Speedometer
		/// </summary>
		public const string SPEEDOMETER_PREFAB_PATH = "Speedometer/SpeedometerPrefab";

		#endregion

		#region Speed Ranges & Colors

		/// <summary>
		/// Range velocità per diverse colorazioni
		/// </summary>
		public const float LOW_SPEED_THRESHOLD = 30f;      // km/h - Verde/Blu
		public const float MEDIUM_SPEED_THRESHOLD = 90f;   // km/h - Giallo
		public const float HIGH_SPEED_THRESHOLD = 130f;    // km/h - Arancione
		public const float DANGER_SPEED_THRESHOLD = 180f;  // km/h - Rosso

		/// <summary>
		/// Velocità massima visualizzabile
		/// </summary>
		public const float MAX_DISPLAY_SPEED = 220f; // km/h

		/// <summary>
		/// Colori per i diversi range di velocità
		/// </summary>
		public static readonly Color LOW_SPEED_COLOR = new Color(0.2f, 0.8f, 0.2f, 1f);    // Verde
		public static readonly Color MEDIUM_SPEED_COLOR = new Color(0.9f, 0.9f, 0.2f, 1f); // Giallo
		public static readonly Color HIGH_SPEED_COLOR = new Color(0.9f, 0.6f, 0.1f, 1f);   // Arancione
		public static readonly Color DANGER_SPEED_COLOR = new Color(0.9f, 0.1f, 0.1f, 1f); // Rosso

		/// <summary>
		/// Colore di default per testo velocità
		/// </summary>
		public static readonly Color DEFAULT_SPEED_COLOR = Color.white;

		#endregion

		#region Animation Settings

		/// <summary>
		/// Impostazioni animazioni speedometer
		/// </summary>
		public const float SPEED_ANIMATION_SMOOTHING = 0.1f;     // Smoothing speed changes
		public const float COLOR_TRANSITION_DURATION = 0.3f;    // Durata cambio colore
		public const float UPDATE_FREQUENCY = 60f;              // Update frequency Hz

		/// <summary>
		/// Velocità di risposta diverse per modalità guida
		/// </summary>
		public const float ECO_RESPONSE_DAMPING = 0.3f;    // Più smooth per eco
		public const float COMFORT_RESPONSE_DAMPING = 0.2f; // Bilanciato
		public const float SPORT_RESPONSE_DAMPING = 0.05f;  // Risposta immediata

		#endregion

		#region Unit Conversion

		/// <summary>
		/// Fattori di conversione unità velocità
		/// </summary>
		public const float KMH_TO_MPH = 0.621371f;
		public const float MPH_TO_KMH = 1.609344f;

		/// <summary>
		/// Enum per unità di misura velocità
		/// </summary>
		public enum SpeedUnit
		{
			KilometersPerHour,  // km/h
			MilesPerHour        // mph
		}

		#endregion

		#region Display Settings

		/// <summary>
		/// Impostazioni display speedometer
		/// </summary>
		public const int SPEED_DECIMAL_PLACES = 0;              // Decimali da mostrare
		public const string SPEED_FORMAT_KMH = "F0";            // Format string km/h
		public const string SPEED_FORMAT_MPH = "F0";            // Format string mph
		public const string SPEED_UNIT_LABEL_KMH = "km/h";      // Label unità km/h
		public const string SPEED_UNIT_LABEL_MPH = "mph";       // Label unità mph

		/// <summary>
		/// Dimensioni e posizioni UI
		/// </summary>
		public const float SPEED_TEXT_SIZE_LARGE = 48f;   // Font size numero velocità
		public const float UNIT_TEXT_SIZE = 24f;          // Font size unità
		public const float PROGRESS_BAR_HEIGHT = 8f;      // Altezza progress bar

		#endregion

		#region Mode-Specific Settings

		/// <summary>
		/// Configurazioni specifiche per modalità guida
		/// </summary>

		// ECO Mode - Focus sull'efficienza
		public static readonly SpeedometerConfig ECO_CONFIG = new SpeedometerConfig
		{
			MaxDisplaySpeed = 120f,        // Limite ridotto per eco
			ShowEfficiencyBar = true,      // Mostra barra efficienza
			ColorByEfficiency = true,      // Colore basato su efficienza
			ResponseDamping = ECO_RESPONSE_DAMPING,
			PreferredUnit = SpeedUnit.KilometersPerHour
		};

		// COMFORT Mode - Bilanciato
		public static readonly SpeedometerConfig COMFORT_CONFIG = new SpeedometerConfig
		{
			MaxDisplaySpeed = MAX_DISPLAY_SPEED,  // Range completo
			ShowEfficiencyBar = false,            // No barra efficienza
			ColorByEfficiency = false,            // Colore basato su velocità
			ResponseDamping = COMFORT_RESPONSE_DAMPING,
			PreferredUnit = SpeedUnit.KilometersPerHour
		};

		// SPORT Mode - Performance focus
		public static readonly SpeedometerConfig SPORT_CONFIG = new SpeedometerConfig
		{
			MaxDisplaySpeed = MAX_DISPLAY_SPEED + 50f, // Range esteso
			ShowEfficiencyBar = false,                 // No efficienza
			ColorByEfficiency = false,                 // Colore aggressivo
			ResponseDamping = SPORT_RESPONSE_DAMPING,  // Risposta immediata
			PreferredUnit = SpeedUnit.KilometersPerHour
		};

		#endregion

		#region Utility Methods

		/// <summary>
		/// Converte velocità da km/h a mph
		/// </summary>
		public static float ConvertKmhToMph(float kmh)
		{
			return kmh * KMH_TO_MPH;
		}

		/// <summary>
		/// Converte velocità da mph a km/h
		/// </summary>
		public static float ConvertMphToKmh(float mph)
		{
			return mph * MPH_TO_KMH;
		}

		/// <summary>
		/// Ottiene il colore appropriato per la velocità
		/// </summary>
		public static Color GetSpeedColor(float speed)
		{
			if (speed >= DANGER_SPEED_THRESHOLD)
				return DANGER_SPEED_COLOR;
			else if (speed >= HIGH_SPEED_THRESHOLD)
				return HIGH_SPEED_COLOR;
			else if (speed >= MEDIUM_SPEED_THRESHOLD)
				return MEDIUM_SPEED_COLOR;
			else if (speed >= LOW_SPEED_THRESHOLD)
				return LOW_SPEED_COLOR;
			else
				return DEFAULT_SPEED_COLOR;
		}

		/// <summary>
		/// Ottiene configurazione per modalità guida
		/// </summary>
		public static SpeedometerConfig GetConfigForDriveMode(ClusterAudi.DriveMode mode)
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
		/// Formatta velocità per display
		/// </summary>
		public static string FormatSpeed(float speed, SpeedUnit unit)
		{
			float displaySpeed = unit == SpeedUnit.MilesPerHour ? ConvertKmhToMph(speed) : speed;
			string format = unit == SpeedUnit.MilesPerHour ? SPEED_FORMAT_MPH : SPEED_FORMAT_KMH;

			return displaySpeed.ToString(format);
		}

		/// <summary>
		/// Ottiene label unità
		/// </summary>
		public static string GetUnitLabel(SpeedUnit unit)
		{
			return unit == SpeedUnit.MilesPerHour ? SPEED_UNIT_LABEL_MPH : SPEED_UNIT_LABEL_KMH;
		}

		#endregion

		#region Development Notes

		/*
         * SPEEDOMETER DATA - CONFIGURAZIONE COMPLETA
         * 
         * Questo file centralizza TUTTE le configurazioni per SpeedometerFeature:
         * 
         * RANGE & COLORS:
         * - 4 fasce di velocità con colori dedicati
         * - Personalizzabili per diverse modalità guida
         * 
         * ANIMATIONS:
         * - Smoothing configurabile per modalità
         * - Transizioni colore fluide
         * 
         * CONVERSIONI:
         * - Support completo km/h ↔ mph
         * - Utility methods per conversioni
         * 
         * MODE CONFIGURATIONS:
         * - Configurazioni dedicate per Eco/Comfort/Sport
         * - Parametri diversi per ogni modalità
         * 
         * Seguendo esattamente il pattern Mercedes per consistenza architetturale.
         */

		#endregion
	}

	/// <summary>
	/// Configurazione specifica per speedometer in base alla modalità guida
	/// </summary>
	[System.Serializable]
	public class SpeedometerConfig
	{
		public float MaxDisplaySpeed;
		public bool ShowEfficiencyBar;
		public bool ColorByEfficiency;
		public float ResponseDamping;
		public SpeedometerData.SpeedUnit PreferredUnit;
	}
}