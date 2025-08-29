using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Configurazioni e costanti per Lane Assist Feature
	/// IDENTICO al pattern ClusterDriveModeData.cs del tuo progetto
	/// </summary>
	public static class LaneAssistData
	{
		#region Asset Paths - IDENTICO Pattern Mercedes

		/// <summary>
		/// Path del prefab principale - IDENTICO al pattern Mercedes
		/// </summary>
		public const string LANE_ASSIST_PREFAB_PATH = "LaneAssist/LaneAssistPrefab";

		#endregion

		#region Lane Detection Configuration

		/// <summary>
		/// Velocità minima per attivazione Lane Assist (km/h)
		/// </summary>
		public const float MIN_ACTIVATION_SPEED = 30f;

		/// <summary>
		/// Tempo di tenuta tasti A/D per rilevamento lane departure (secondi)
		/// </summary>
		public const float LANE_DEPARTURE_TIME_THRESHOLD = 2.0f;

		/// <summary>
		/// Tempo per reset automatico lane departure
		/// </summary>
		public const float AUTO_RESET_TIME = 1.0f;

		#endregion

		#region Audio Configuration

		/// <summary>
		/// Path audio per lane departure - LEFT
		/// </summary>
		public const string LANE_DEPARTURE_LEFT_AUDIO_PATH = "Audio/SFX/LaneAssist/LaneDepartureLeft";

		/// <summary>
		/// Path audio per lane departure - RIGHT  
		/// </summary>
		public const string LANE_DEPARTURE_RIGHT_AUDIO_PATH = "Audio/SFX/LaneAssist/LaneDepartureRight";

		/// <summary>
		/// Volume audio lane assist
		/// </summary>
		public const float LANE_ASSIST_AUDIO_VOLUME = 0.8f;

		/// <summary>
		/// Priorità audio lane assist (alta per sicurezza)
		/// </summary>
		public const int LANE_ASSIST_AUDIO_PRIORITY = 4;

		/// <summary>
		/// 🆕 NUOVO: Intervallo ripetizione audio durante warning (secondi)
		/// </summary>
		public const float AUDIO_REPEAT_INTERVAL = 2.0f;

		#endregion

		#region UI Configuration

		/// <summary>
		/// Colori lane lines
		/// </summary>
		public static readonly Color NORMAL_LANE_COLOR = new Color(1f, 1f, 1f, 0.8f);  // Bianco normale
		public static readonly Color DEPARTURE_LANE_COLOR = new Color(1f, 0.8f, 0f, 1f); // Giallo warning

		/// <summary>
		/// Car icon shift distance per departure visual
		/// </summary>
		public const float CAR_ICON_SHIFT_DISTANCE = 20f;

		#endregion

		#region Debug Keys - IDENTICO Pattern Mercedes

		/// <summary>
		/// Tasti di controllo lane assist
		/// </summary>
		public static readonly KeyCode LEFT_DEPARTURE_KEY = KeyCode.A;
		public static readonly KeyCode RIGHT_DEPARTURE_KEY = KeyCode.D;

		/// <summary>
		/// Tasti debug
		/// </summary>
		public static readonly KeyCode DEBUG_TOGGLE_KEY = KeyCode.F6;

		#endregion

		#region Validation Helper - IDENTICO Pattern Mercedes

		/// <summary>
		/// Verifica se lane assist può essere attivo
		/// </summary>
		public static bool CanActivateLaneAssist(float currentSpeed)
		{
			return currentSpeed >= MIN_ACTIVATION_SPEED;
		}

		/// <summary>
		/// Verifica se è lane departure
		/// </summary>
		public static bool IsLaneDeparture(float holdTime)
		{
			return holdTime >= LANE_DEPARTURE_TIME_THRESHOLD;
		}

		#endregion

		#region Lane Departure Types

		/// <summary>
		/// Tipi di lane departure
		/// </summary>
		public enum LaneDepartureType
		{
			None,           // Nessuna departure
			Left,           // Departure a sinistra (A key)
			Right           // Departure a destra (D key)
		}

		/// <summary>
		/// Stati Lane Assist
		/// </summary>
		public enum LaneAssistState
		{
			Disabled,       // Disabilitato (velocità < 30)
			Active,         // Attivo e monitoraggio
			Warning,        // Warning departure in corso
			Unavailable     // Non disponibile per condizioni
		}

		#endregion
	}
}