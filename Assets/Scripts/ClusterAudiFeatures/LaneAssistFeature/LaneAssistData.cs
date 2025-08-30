using UnityEngine;

namespace ClusterAudiFeatures
{
	public static class LaneAssistData
	{
		#region Asset Paths

		public const string LANE_ASSIST_PREFAB_PATH = "LaneAssist/LaneAssistPrefab";

		#endregion

		#region Configuration - SEMPLIFICATA

		/// <summary>
		/// Velocità minima attivazione (km/h)
		/// </summary>
		public const float MIN_ACTIVATION_SPEED = 30f;

		/// <summary>
		/// Tempo tenuta A/D per lane departure (secondi)
		/// </summary>
		public const float LANE_DEPARTURE_TIME_THRESHOLD = 2.0f;

		/// <summary>
		/// Tempo auto-reset departure
		/// </summary>
		public const float AUTO_RESET_TIME = 1.0f;

		#endregion

		#region Audio

		public const string LANE_DEPARTURE_LEFT_AUDIO_PATH = "Audio/SFX/LaneAssist/LaneDepartureLeft";
		public const string LANE_DEPARTURE_RIGHT_AUDIO_PATH = "Audio/SFX/LaneAssist/LaneDepartureRight";
		public const float LANE_ASSIST_AUDIO_VOLUME = 0.8f;
		public const int LANE_ASSIST_AUDIO_PRIORITY = 4;

		#endregion

		#region UI Colors

		public static readonly Color NORMAL_LANE_COLOR = new Color(1f, 1f, 1f, 0.8f);
		public static readonly Color DEPARTURE_LANE_COLOR = new Color(1f, 0.8f, 0f, 1f);
		public const float CAR_ICON_SHIFT_DISTANCE = 20f;

		#endregion

		#region Controls

		public static readonly KeyCode LEFT_DEPARTURE_KEY = KeyCode.A;
		public static readonly KeyCode RIGHT_DEPARTURE_KEY = KeyCode.D;

		#endregion

		#region Helper Methods

		public static bool CanActivateLaneAssist(float currentSpeed)
		{
			return currentSpeed >= MIN_ACTIVATION_SPEED;
		}

		public static bool IsLaneDeparture(float holdTime)
		{
			return holdTime >= LANE_DEPARTURE_TIME_THRESHOLD;
		}

		#endregion

		#region Enums

		public enum LaneDepartureType
		{
			None,
			Left,
			Right
		}

		public enum LaneAssistState
		{
			Disabled,
			Active,
			Warning
		}

		#endregion
	}
}