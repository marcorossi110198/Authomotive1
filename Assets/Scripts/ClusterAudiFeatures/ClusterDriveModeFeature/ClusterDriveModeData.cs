using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Costanti per Cluster Drive Mode Feature
	/// IDENTICO al pattern DashboardData.cs del progetto Mercedes
	/// </summary>
	public static class ClusterDriveModeData
	{
		#region Asset Paths - IDENTICO Mercedes Pattern

		/// <summary>
		/// Path del prefab principale - IDENTICO al pattern Mercedes
		/// </summary>
		public const string CLUSTER_DRIVE_MODE_PREFAB_PATH = "ClusterDriveMode/ClusterDriveModePrefab";

		#endregion

		#region State Names - IDENTICO Mercedes Pattern

		/// <summary>
		/// Nomi stati FSM - IDENTICI al pattern Mercedes
		/// </summary>
		public const string ECO_MODE_STATE = "EcoModeState";
		public const string COMFORT_MODE_STATE = "ComfortModeState";
		public const string SPORT_MODE_STATE = "SportModeState";
		public const string WELCOME_STATE = "WelcomeState";

		#endregion

		#region Debug Keys - IDENTICO Mercedes Pattern

		/// <summary>
		/// Tasti debug per transizioni - IDENTICI al Mercedes
		/// </summary>
		public static readonly KeyCode DEBUG_ECO_KEY = KeyCode.F1;
		public static readonly KeyCode DEBUG_COMFORT_KEY = KeyCode.F2;
		public static readonly KeyCode DEBUG_SPORT_KEY = KeyCode.F3;
		public static readonly KeyCode DEBUG_WELCOME_KEY = KeyCode.F4;

		#endregion

		#region UI Layout Constants

		/// <summary>
		/// Costanti layout UI
		/// </summary>
		public const float MODE_INDICATOR_SIZE = 80f;
		public const float MODE_INDICATOR_SPACING = 100f;
		public const float ANIMATION_DURATION = 0.3f;

		// Posizioni UI (Screen Space)
		public static readonly Vector2 MODE_INDICATORS_POSITION = new Vector2(100, -100);
		public static readonly Vector2 VEHICLE_INFO_POSITION = new Vector2(-100, -100);

		#endregion

		#region Audi Brand Colors

		/// <summary>
		/// Palette colori Audi per modalità guida
		/// </summary>
		public static readonly Color ECO_COLOR = new Color(0.2f, 0.8f, 0.2f, 1f);        // Verde Eco
		public static readonly Color COMFORT_COLOR = new Color(0.2f, 0.5f, 0.8f, 1f);    // Blu Comfort  
		public static readonly Color SPORT_COLOR = new Color(0.9f, 0.1f, 0.1f, 1f);      // Rosso Sport
		public static readonly Color INACTIVE_COLOR = new Color(0.3f, 0.3f, 0.3f, 0.5f); // Grigio Inactive
		public static readonly Color TEXT_COLOR = new Color(0.9f, 0.9f, 0.9f, 1f);       // Bianco Testi

		#endregion

		#region UI Text Labels

		/// <summary>
		/// Etichette testo per modalità
		/// </summary>
		public const string ECO_MODE_TEXT = "ECO";
		public const string COMFORT_MODE_TEXT = "COMFORT";
		public const string SPORT_MODE_TEXT = "SPORT";

		#endregion

		#region Validation Helper

		/// <summary>
		/// Verifica se uno stato è valido - IDENTICO al pattern Mercedes
		/// </summary>
		public static bool IsValidState(string stateName)
		{
			return stateName == ECO_MODE_STATE ||
				   stateName == COMFORT_MODE_STATE ||
				   stateName == SPORT_MODE_STATE ||
				   stateName == WELCOME_STATE;
		}

		#endregion
	}
}